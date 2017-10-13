using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using OpenCvSharp;
using LandmarkDetector;

namespace SharpFace.Tests
{
    public class LandmarkTestImg
    {
        private void INFO_STREAM(string msg)
        {
            Debug.WriteLine(msg);
        }

        private void FATAL_STREAM(string msg)
        {
            Debug.WriteLine("[ERROR] " + msg);
        }

        private void WARN_STREAM(string msg)
        {
            Debug.WriteLine("[WARN] " + msg);
        }

        private int CV_FOURCC(char c1, char c2, char c3, char c4)
        {
            return (((c1) & 255) + (((c2) & 255) << 8) + (((c3) & 255) << 16) + (((c4) & 255) << 24));
        }

        void visualise_tracking(ref Mat captured_image, ref CLNF face_model, ref FaceModelParameters det_parameters, Point3f gazeDirection0, Point3f gazeDirection1, int frame_count, double fx, double fy, double cx, double cy)
        {
            //Drawing the facial landmarks on the face and the bounding box around it if tracking is successful and initialised
            double detection_certainty = face_model.detection_certainty;
            bool detection_success = face_model.detection_success;

            double visualisation_boundary = 0.2;

            // Only draw if the reliability is reasonable, the value is slightly ad-hoc
            if (detection_certainty < visualisation_boundary)
            {
                LandmarkDetector.Draw(captured_image, face_model);

                double vis_certainty = detection_certainty;
                if (vis_certainty > 1)
                    vis_certainty = 1;
                if (vis_certainty < -1)

                    vis_certainty = -1;

                vis_certainty = (vis_certainty + 1) / (visualisation_boundary + 1);

                //A rough heuristic for box around the face width

                int thickness = (int)Math.Ceiling(2.0 * ((double)captured_image.Cols) / 640.0);

                Vec6d pose_estimate_to_draw = LandmarkDetector.GetCorrectedPoseWorld(face_model, fx, fy, cx, cy);

                //Draw it in reddish if uncertain, blueish if certain
                LandmarkDetector.DrawBox(captured_image, pose_estimate_to_draw, new Scalar((1 - vis_certainty) * 255.0, 0, vis_certainty * 255), thickness, fx, fy, cx, cy);

                if (det_parameters.track_gaze && detection_success && face_model.eye_model)
                {
                    //TODO: WTF
                    //FaceAnalysis.DrawGaze(captured_image, face_model, gazeDirection0, gazeDirection1, fx, fy, cx, cy);
                }
            }

            //  Work out the framerate
            // if (frame_count % 10 == 0)
            // {
            //  double t1 = cv::getTickCount();
            //fps_tracker = 10.0 / (double(t1 - t0) / cv::getTickFrequency());
            //  t0 = t1;
            // }

            //  Write out the framerate on the image before displaying it
            // char fpsC[255];
            //std::sprintf(fpsC, "%d", (int) fps_tracker);
            // string fpsSt("FPS:");
            //fpsSt += fpsC;
            // cv::putText(captured_image, fpsSt, cv::Point(10, 20), CV_FONT_HERSHEY_SIMPLEX, 0.5, CV_RGB(255, 0, 0));

            // if (!det_parameters.quiet_mode)
            // {
            //  cv::namedWindow("tracking_result", 1);
            //  cv::imshow("tracking_result", captured_image);
            // }
        }

        public int Run()
        {
            int device = 0;

            var argument = new StringList { "./" };
            FaceModelParameters det_parameters = new FaceModelParameters(argument);

            //vector<string> files, depth_directories, output_video_files, out_dummy;
            StringList files = new StringList(), output_video_files = new StringList(), out_dummy = new StringList();
            bool u;
            string output_codec;
            LandmarkDetector.get_video_input_output_params(files, out_dummy, output_video_files, out u, out output_codec, argument);

            CLNF clnf_model = new CLNF(det_parameters.model_location);

            float fx = 0, fy = 0, cx = 0, cy = 0;
            LandmarkDetector.get_camera_params(out device, out fx, out fy, out cx, out cy, argument);

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
                string current_file = "";

                //// We might specify multiple video files as arguments
                if (files.Count > 0)
                {
                    f_n++;
                    current_file = files[f_n];
                }
                else
                {
                    // If we want to write out from webcam
                    f_n = 0;
                }

                //// Do some grabbing
                VideoCapture video_capture;
                if (current_file.Length > 0)
                {
                    //if (!boost::filesystem::exists(current_file))
                    //{
                    //    FATAL_STREAM("File does not exist");
                    //    return 1;
                    //}

                    //current_file = boost::filesystem::path(current_file).generic_string();

                    //INFO_STREAM("Attempting to read from file: " << current_file);
                    video_capture = new VideoCapture(current_file);
                }
                else
                {
                    INFO_STREAM("Attempting to capture from device: " + device);
                    video_capture = new VideoCapture(device);

                    // Read a first frame often empty in camera
                    //cv::Mat captured_image;
                    //video_capture >> captured_image;
                    using (Mat dummy = new Mat())
                        video_capture.Read(dummy);
                }

                if (!video_capture.IsOpened())
                {
                    FATAL_STREAM("Failed to open video source");
                    return 1;
                }
                else INFO_STREAM("Device or file opened");

                Mat captured_image = new Mat();
                video_capture.Read(captured_image);

                // If optical centers are not defined just use center of image
                if (cx_undefined)
                {
                    cx = captured_image.Cols / 2.0f;
                    cy = captured_image.Rows / 2.0f;
                }
                // Use a rough guess-timate of focal length
                if (fx_undefined)
                {
                    fx = (float)(500 * (captured_image.Cols / 640.0));
                    fy = (float)(500 * (captured_image.Rows / 480.0));

                    fx = (float)((fx + fy) / 2.0);
                    fy = fx;
                }

                int frame_count = 0;

                // saving the videos
                VideoWriter writerFace;
                if (output_video_files.Count != 0)
                {
                    try
                    {
                        writerFace = new VideoWriter(output_video_files[f_n], CV_FOURCC(output_codec[0], output_codec[1], output_codec[2], output_codec[3]), 30, captured_image.Size(), true);
                    }
                    catch (Exception e)
                    {
                        WARN_STREAM("Could not open VideoWriter, OUTPUT FILE WILL NOT BE WRITTEN. Currently using codec " + output_codec + ", try using an other one (-oc option)");
                    }
                }

                // Use for timestamping if using a webcam
                long t_initial = Cv2.GetTickCount();

                INFO_STREAM("Starting tracking");
                while (!captured_image.Empty())
                {

                    // Reading the images
                    MatOfByte grayscale_image = new MatOfByte();

                    if (captured_image.Channels() == 3)
                    {
                        Cv2.CvtColor(captured_image, grayscale_image, ColorConversionCodes.BGR2GRAY);
                    }
                    else
                    {
                        grayscale_image = (MatOfByte)captured_image.Clone();
                    }

                    // The actual facial landmark detection / tracking
                    bool detection_success = LandmarkDetector.DetectLandmarksInVideo(new SWIGTYPE_p_cv__Mat_T_uchar_t(grayscale_image.CvPtr, true), new SWIGTYPE_p_CLNF(CLNF.getC0Ptr(clnf_model)), new SWIGTYPE_p_FaceModelParameters(FaceModelParameters.getCPtr(det_parameters)));

                    // Visualising the results
                    // Drawing the facial landmarks on the face and the bounding box around it if tracking is successful and initialised
                    double detection_certainty = clnf_model.detection_certainty;

                    // Gaze tracking, absolute gaze direction
                    Point3f gazeDirection0 = new Point3f(0, 0, -1);
                    Point3f gazeDirection1 = new Point3f(0, 0, -1);

                    if (det_parameters.track_gaze && detection_success && clnf_model.eye_model)
                    {
                        //TODO: WTF
                        //FaceAnalysis.EstimateGaze(clnf_model, gazeDirection0, fx, fy, cx, cy, true);
                        //FaceAnalysis.EstimateGaze(clnf_model, gazeDirection1, fx, fy, cx, cy, false);
                    }

                    visualise_tracking(captured_image, clnf_model, det_parameters, gazeDirection0, gazeDirection1, frame_count, fx, fy, cx, cy);

                    //    // output the tracked video
                    //    if (!output_video_files.empty())
                    //    {
                    //        writerFace << captured_image;
                    //    }


                    //    video_capture >> captured_image;

                    //    // detect key presses
                    //    char character_press = cv::waitKey(1);

                    //    // restart the tracker
                    //    if (character_press == 'r')
                    //    {
                    //        clnf_model.Reset();
                    //    }
                    //    // quit the application
                    //    else if (character_press == 'q')
                    //    {
                    //        return (0);
                    //    }

                    //    // Update the frame count
                    //    frame_count++;

                }

                //frame_count = 0;

                //// Reset the model, for the next video
                //clnf_model.Reset();

                //// break out of the loop if done with all the files (or using a webcam)
                //if (f_n == files.size() - 1 || files.empty())
                //{
                //    done = true;
                //}
            }
        }
    }
}
