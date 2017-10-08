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
//
//     * Any publications arising from the use of this software, including but
//       not limited to academic journal and conference publications, technical
//       reports and manuals, must cite at least one of the following works:
//
//       OpenFace: an open source facial behavior analysis toolkit
//       Tadas Baltru�aitis, Peter Robinson, and Louis-Philippe Morency
//       in IEEE Winter Conference on Applications of Computer Vision, 2016  
//
//       Rendering of Eyes for Eye-Shape Registration and Gaze Estimation
//       Erroll Wood, Tadas Baltru�aitis, Xucong Zhang, Yusuke Sugano, Peter Robinson, and Andreas Bulling 
//       in IEEE International. Conference on Computer Vision (ICCV),  2015 
//
//       Cross-dataset learning and person-speci?c normalisation for automatic Action Unit detection
//       Tadas Baltru�aitis, Marwa Mahmoud, and Peter Robinson 
//       in Facial Expression Recognition and Analysis Challenge, 
//       IEEE International Conference on Automatic Face and Gesture Recognition, 2015 
//
//       Constrained Local Neural Fields for robust facial landmark detection in the wild.
//       Tadas Baltru�aitis, Peter Robinson, and Louis-Philippe Morency. 
//       in IEEE Int. Conference on Computer Vision Workshops, 300 Faces in-the-Wild Challenge, 2013.    
//
///////////////////////////////////////////////////////////////////////////////

//  Header for all external CLNF/CLM-Z/CLM methods of interest to the user
#ifndef __LANDMARK_DETECTOR_UTILS_h_
#define __LANDMARK_DETECTOR_UTILS_h_

// OpenCV includes
#include <opencv2/core/core.hpp>

#include "LandmarkDetectorModel.h"


#ifdef SWIG
#define OPENFACE_API
#endif

using namespace std;

namespace LandmarkDetector
{
	//===========================================================================	
	// Defining a set of useful utility functions to be used within CLNF


	//=============================================================================================
	// Helper functions for parsing the inputs
	//=============================================================================================
	OPENFACE_API void get_video_input_output_params(vector<string> &input_video_file, vector<string> &output_files,
		vector<string> &output_video_files, bool& world_coordinates_pose, string &output_codec, vector<string> &arguments);

    OPENFACE_API void get_camera_params(int &device, float &fx, float &fy, float &cx, float &cy, vector<string> &arguments);

    OPENFACE_API void get_image_input_output_params(vector<string> &input_image_files, vector<string> &output_feature_files, vector<string> &output_pose_files, vector<string> &output_image_files,
		vector<cv::Rect_<double>> &input_bounding_boxes, vector<string> &arguments);

	//===========================================================================
	// Fast patch expert response computation (linear model across a ROI) using normalised cross-correlation
	//===========================================================================
	// This is a modified version of openCV code that allows for precomputed dfts of templates and for precomputed dfts of an image
	// _img is the input img, _img_dft it's dft (optional), _integral_img the images integral image (optional), squared integral image (optional), 
	// templ is the template we are convolving with, templ_dfts it's dfts at varying windows sizes (optional),  _result - the output, method the type of convolution
    OPENFACE_API void matchTemplate_m( const cv::Mat_<float>& input_img, cv::Mat_<double>& img_dft, cv::Mat& _integral_img, cv::Mat& _integral_img_sq, const cv::Mat_<float>&  templ, map<int, cv::Mat_<double> >& templ_dfts, cv::Mat_<float>& result, int method );

	//===========================================================================
	// Point set and landmark manipulation functions
	//===========================================================================
	// Using Kabsch's algorithm for aligning shapes
	//This assumes that align_from and align_to are already mean normalised
    OPENFACE_API  cv::Matx22d AlignShapesKabsch2D(const cv::Mat_<double>& align_from, const cv::Mat_<double>& align_to );

	//=============================================================================
	// Basically Kabsch's algorithm but also allows the collection of points to be different in scale from each other
    OPENFACE_API cv::Matx22d AlignShapesWithScale(cv::Mat_<double>& src, cv::Mat_<double> dst);

	//===========================================================================
	// Visualisation functions
	//===========================================================================
	OPENFACE_API void Project(cv::Mat_<double>& dest, const cv::Mat_<double>& mesh, double fx, double fy, double cx, double cy);
	OPENFACE_API void DrawBox(cv::Mat image, cv::Vec6d pose, cv::Scalar color, int thickness, float fx, float fy, float cx, float cy);

	// Drawing face bounding box
	OPENFACE_API vector<std::pair<cv::Point2d, cv::Point2d>> CalculateBox(cv::Vec6d pose, float fx, float fy, float cx, float cy);
	OPENFACE_API void DrawBox(vector<pair<cv::Point, cv::Point>> lines, cv::Mat image, cv::Scalar color, int thickness);

	OPENFACE_API vector<cv::Point2d> CalculateLandmarks(const cv::Mat_<double>& shape2D, cv::Mat_<int>& visibilities);
	OPENFACE_API vector<cv::Point2d> CalculateLandmarks(CLNF& clnf_model);
	OPENFACE_API void DrawLandmarks(cv::Mat img, vector<cv::Point> landmarks);

	OPENFACE_API void Draw(cv::Mat img, const cv::Mat_<double>& shape2D, const cv::Mat_<int>& visibilities);
	OPENFACE_API void Draw(cv::Mat img, const cv::Mat_<double>& shape2D);
	OPENFACE_API void Draw(cv::Mat img, const CLNF& clnf_model);


	//===========================================================================
	// Angle representation conversion helpers
	//===========================================================================
    OPENFACE_API cv::Matx33d Euler2RotationMatrix(const cv::Vec3d& eulerAngles);

	// Using the XYZ convention R = Rx * Ry * Rz, left-handed positive sign
    OPENFACE_API cv::Vec3d RotationMatrix2Euler(const cv::Matx33d& rotation_matrix);

    OPENFACE_API cv::Vec3d Euler2AxisAngle(const cv::Vec3d& euler);

    OPENFACE_API cv::Vec3d AxisAngle2Euler(const cv::Vec3d& axis_angle);

    OPENFACE_API cv::Matx33d AxisAngle2RotationMatrix(const cv::Vec3d& axis_angle);

    OPENFACE_API cv::Vec3d RotationMatrix2AxisAngle(const cv::Matx33d& rotation_matrix);

	//============================================================================
	// Face detection helpers
	//============================================================================

	// Face detection using Haar cascade classifier
	OPENFACE_API bool DetectFaces(vector<cv::Rect_<double> >& o_regions, const cv::Mat_<uchar>& intensity);
	OPENFACE_API bool DetectFaces(vector<cv::Rect_<double> >& o_regions, const cv::Mat_<uchar>& intensity, cv::CascadeClassifier& classifier);
	// The preference point allows for disambiguation if multiple faces are present (pick the closest one), if it is not set the biggest face is chosen
    OPENFACE_API bool DetectSingleFace(cv::Rect_<double>& o_region, const cv::Mat_<uchar>& intensity, cv::CascadeClassifier& classifier, const cv::Point preference = cv::Point(-1,-1));

	// Face detection using HOG-SVM classifier
    OPENFACE_API bool DetectFacesHOG(vector<cv::Rect_<double> >& o_regions, const cv::Mat_<uchar>& intensity, std::vector<double>& confidences);
    OPENFACE_API bool DetectFacesHOG(vector<cv::Rect_<double> >& o_regions, const cv::Mat_<uchar>& intensity, dlib::frontal_face_detector& classifier, std::vector<double>& confidences);
	// The preference point allows for disambiguation if multiple faces are present (pick the closest one), if it is not set the biggest face is chosen
    OPENFACE_API bool DetectSingleFaceHOG(cv::Rect_<double>& o_region, const cv::Mat_<uchar>& intensity, dlib::frontal_face_detector& classifier, double& confidence, const cv::Point preference = cv::Point(-1,-1));

	//============================================================================
	// Matrix reading functionality
	//============================================================================

	// Reading a matrix written in a binary format
    OPENFACE_API void ReadMatBin(std::ifstream& stream, cv::Mat &output_mat);

	// Reading in a matrix from a stream
    OPENFACE_API void ReadMat(std::ifstream& stream, cv::Mat& output_matrix);

	// Skipping comments (lines starting with # symbol)
    OPENFACE_API void SkipComments(std::ifstream& stream);

}
#endif
