///////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2017, Carnegie Mellon University and University of Cambridge,
// all rights reserved.
//
// ACADEMIC OR NON-PROFIT ORGANIZATION NONCOMMERCIAL RESEARCH USE ONLY
//
// BY USING OR DOWNLOADING THE SOFTWARE, YOU ARE AGREEING TO THE TERMS OF THIS LICENSE AGREEMENT.  
// IF YOU DO NOT AGREE WITH THESE TERMS, YOU MAY NOT USE OR DOWNLOAD THE SOFTWARE.
//
// License can be found in OpenFace-license.txt

//     * Any publications arising from the use of this software, including but
//       not limited to academic journal and conference publications, technical
//       reports and manuals, must cite at least one of the following works:
//
//       OpenFace: an open source facial behavior analysis toolkit
//       Tadas Baltrušaitis, Peter Robinson, and Louis-Philippe Morency
//       in IEEE Winter Conference on Applications of Computer Vision, 2016  
//
//       Rendering of Eyes for Eye-Shape Registration and Gaze Estimation
//       Erroll Wood, Tadas Baltrušaitis, Xucong Zhang, Yusuke Sugano, Peter Robinson, and Andreas Bulling 
//       in IEEE International. Conference on Computer Vision (ICCV),  2015 
//
//       Cross-dataset learning and person-speci?c normalisation for automatic Action Unit detection
//       Tadas Baltrušaitis, Marwa Mahmoud, and Peter Robinson 
//       in Facial Expression Recognition and Analysis Challenge, 
//       IEEE International Conference on Automatic Face and Gesture Recognition, 2015 
//
//       Constrained Local Neural Fields for robust facial landmark detection in the wild.
//       Tadas Baltrušaitis, Peter Robinson, and Louis-Philippe Morency. 
//       in IEEE Int. Conference on Computer Vision Workshops, 300 Faces in-the-Wild Challenge, 2013.    
//
///////////////////////////////////////////////////////////////////////////////
// FaceTrackingVid.cpp : Defines the entry point for the console application for tracking faces in videos.
// Libraries for landmark detection (includes CLNF and CLM modules)

// #include "stdafx.h"
#include "LandmarkCoreIncludes.h"

#include <cstdint>
#include <cstdio>
#include <iostream>
#include <fstream>
#include <sstream>
#include <vector>

// OpenCV includes
#include <opencv2/videoio/videoio.hpp>  // Video write
#include <opencv2/videoio/videoio_c.h>  // Video write
#include <opencv2/imgproc.hpp>
#include <opencv2/highgui/highgui.hpp>

// Boost includes
#include <filesystem.hpp>
#include <filesystem/fstream.hpp>

using namespace std;

#define INFO_STREAM( stream ) \
std::cout << stream << std::endl

#define WARN_STREAM( stream ) \
std::cout << "Warning: " << stream << std::endl

#define ERROR_STREAM( stream ) \
std::cout << "Error: " << stream << std::endl

static void printErrorAndAbort(const std::string & error)
{
	std::cout << error << std::endl;
	abort();
}

#define FATAL_STREAM( stream ) \
printErrorAndAbort( std::string( "Fatal error: " ) + stream )

vector<string> get_arguments(int argc, char **argv)
{
	vector<string> arguments;

	for (int i = 0; i < argc; ++i)
	{
		arguments.push_back(string(argv[i]));
	}
	return arguments;
}

// Some globals for tracking timing information for visualisation
double fps_tracker = -1.0;
int64_t t0 = 0;

// Visualising the results
void visualise_tracking(cv::Mat& captured_image, const LandmarkDetector::CLNF& face_model, const LandmarkDetector::FaceModelParameters& det_parameters, cv::Point3f gazeDirection0, cv::Point3f gazeDirection1, int frame_count, double fx, double fy, double cx, double cy)
{

	// Drawing the facial landmarks on the face and the bounding box around it if tracking is successful and initialised
	double detection_certainty = face_model.detection_certainty;
	bool detection_success = face_model.detection_success;

	double visualisation_boundary = 0.2;

	// Only draw if the reliability is reasonable, the value is slightly ad-hoc
	if (detection_certainty < visualisation_boundary)
	{
		LandmarkDetector::Draw(captured_image, face_model);

		double vis_certainty = detection_certainty;
		if (vis_certainty > 1)
			vis_certainty = 1;
		if (vis_certainty < -1)
			vis_certainty = -1;

		vis_certainty = (vis_certainty + 1) / (visualisation_boundary + 1);

		// A rough heuristic for box around the face width
		int thickness = (int)std::ceil(2.0* ((double)captured_image.cols) / 640.0);

		cv::Vec6d pose_estimate_to_draw = LandmarkDetector::GetCorrectedPoseWorld(face_model, fx, fy, cx, cy);

		// Draw it in reddish if uncertain, blueish if certain
		LandmarkDetector::DrawBox(captured_image, pose_estimate_to_draw, cv::Scalar((1 - vis_certainty)*255.0, 0, vis_certainty * 255), thickness, fx, fy, cx, cy);

		if (det_parameters.track_gaze && detection_success && face_model.eye_model)
		{
			//FaceAnalysis::DrawGaze(captured_image, face_model, gazeDirection0, gazeDirection1, fx, fy, cx, cy);
		}
	}

	// Work out the framerate
	if (frame_count % 10 == 0)
	{
		double t1 = cv::getTickCount();
		fps_tracker = 10.0 / (double(t1 - t0) / cv::getTickFrequency());
		t0 = t1;
	}

	// Write out the framerate on the image before displaying it
	char fpsC[255];
	std::sprintf(fpsC, "%d", (int)fps_tracker);
	string fpsSt("FPS:");
	fpsSt += fpsC;
	cv::putText(captured_image, fpsSt, cv::Point(10, 20), CV_FONT_HERSHEY_SIMPLEX, 0.5, CV_RGB(255, 0, 0));

	if (!det_parameters.quiet_mode)
	{
		cv::namedWindow("tracking_result", 1);
		cv::imshow("tracking_result", captured_image);
	}
}

int main(int argc, char **argv)
{
	vector<string> arguments = get_arguments(argc, argv);

	// Some initial parameters that can be overriden from command line	
	vector<string> files, output_video_files, out_dummy;

	// By default try webcam 0
	int device = 0;

	LandmarkDetector::FaceModelParameters det_parameters(arguments);

	// Get the input output file parameters

	// Indicates that rotation should be with respect to world or camera coordinates
	bool u;
	string output_codec;
	LandmarkDetector::get_video_input_output_params(files, out_dummy, output_video_files, u, output_codec, arguments);

	// The modules that are being used for tracking
	LandmarkDetector::CLNF clnf_model(det_parameters.model_location);

	// Grab camera parameters, if they are not defined (approximate values will be used)
	float fx = 0, fy = 0, cx = 0, cy = 0;
	// Get camera parameters
	LandmarkDetector::get_camera_params(device, fx, fy, cx, cy, arguments);

	// If cx (optical axis centre) is undefined will use the image size/2 as an estimate
	bool cx_undefined = false;
	bool fx_undefined = false;
	if (cx == 0 || cy == 0)
	{
		cx_undefined = true;
	}
	if (fx == 0 || fy == 0)
	{
		fx_undefined = true;
	}

	// If multiple video files are tracked, use this to indicate if we are done
	bool done = false;
	int f_n = -1;

	det_parameters.track_gaze = true;

	while (!done) // this is not a for loop as we might also be reading from a webcam
	{

		string current_file;

		// We might specify multiple video files as arguments
		if (files.size() > 0)
		{
			f_n++;
			current_file = files[f_n];
		}
		else
		{
			// If we want to write out from webcam
			f_n = 0;
		}

		// Do some grabbing
		cv::VideoCapture video_capture;
		if (current_file.size() > 0)
		{
			if (!boost::filesystem::exists(current_file))
			{
				FATAL_STREAM("File does not exist");
				return 1;
			}

			current_file = boost::filesystem::path(current_file).generic_string();

			INFO_STREAM("Attempting to read from file: " << current_file);
			video_capture = cv::VideoCapture(current_file);
		}
		else
		{
			INFO_STREAM("Attempting to capture from device: " << device);
			video_capture = cv::VideoCapture(device);

			// Read a first frame often empty in camera
			cv::Mat captured_image;
			video_capture >> captured_image;
		}

		if (!video_capture.isOpened())
		{
			FATAL_STREAM("Failed to open video source");
			return 1;
		}
		else INFO_STREAM("Device or file opened");

		cv::Mat captured_image;
		video_capture >> captured_image;

		// If optical centers are not defined just use center of image
		if (cx_undefined)
		{
			cx = captured_image.cols / 2.0f;
			cy = captured_image.rows / 2.0f;
		}
		// Use a rough guess-timate of focal length
		if (fx_undefined)
		{
			fx = 500 * (captured_image.cols / 640.0);
			fy = 500 * (captured_image.rows / 480.0);

			fx = (fx + fy) / 2.0;
			fy = fx;
		}

		int frame_count = 0;

		// saving the videos
		cv::VideoWriter writerFace;
		if (!output_video_files.empty())
		{
			try
			{
				writerFace = cv::VideoWriter(output_video_files[f_n], CV_FOURCC(output_codec[0], output_codec[1], output_codec[2], output_codec[3]), 30, captured_image.size(), true);
			}
			catch (cv::Exception e)
			{
				WARN_STREAM("Could not open VideoWriter, OUTPUT FILE WILL NOT BE WRITTEN. Currently using codec " << output_codec << ", try using an other one (-oc option)");
			}
		}

		// Use for timestamping if using a webcam
		int64 t_initial = cv::getTickCount();

		INFO_STREAM("Starting tracking");
		while (!captured_image.empty())
		{

			// Reading the images
			cv::Mat_<uchar> grayscale_image;

			if (captured_image.channels() == 3)
			{
				cv::cvtColor(captured_image, grayscale_image, CV_BGR2GRAY);
			}
			else
			{
				grayscale_image = captured_image.clone();
			}

			// The actual facial landmark detection / tracking
			bool detection_success = LandmarkDetector::DetectLandmarksInVideo(grayscale_image, clnf_model, det_parameters);

			// Visualising the results
			// Drawing the facial landmarks on the face and the bounding box around it if tracking is successful and initialised
			double detection_certainty = clnf_model.detection_certainty;

			// Gaze tracking, absolute gaze direction
			cv::Point3f gazeDirection0(0, 0, -1);
			cv::Point3f gazeDirection1(0, 0, -1);

			if (det_parameters.track_gaze && detection_success && clnf_model.eye_model)
			{
				//FaceAnalysis::EstimateGaze(clnf_model, gazeDirection0, fx, fy, cx, cy, true);
				//FaceAnalysis::EstimateGaze(clnf_model, gazeDirection1, fx, fy, cx, cy, false);
			}

			visualise_tracking(captured_image, clnf_model, det_parameters, gazeDirection0, gazeDirection1, frame_count, fx, fy, cx, cy);

			// output the tracked video
			if (!output_video_files.empty())
			{
				writerFace << captured_image;
			}


			video_capture >> captured_image;

			// detect key presses
			char character_press = cv::waitKey(1);

			// restart the tracker
			if (character_press == 'r')
			{
				clnf_model.Reset();
			}
			// quit the application
			else if (character_press == 'q')
			{
				return(0);
			}

			// Update the frame count
			frame_count++;

		}

		frame_count = 0;

		// Reset the model, for the next video
		clnf_model.Reset();

		// break out of the loop if done with all the files (or using a webcam)
		if (f_n == files.size() - 1 || files.empty())
		{
			done = true;
		}
	}

	return 0;
}