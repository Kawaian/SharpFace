using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using OpenCvSharp;

namespace SharpFace.Tests
{
    public class LandmarkTestVid : TestBase
    {
        public double SizeFactor = 1;
        public Size Size = new Size(320, 240);

        // Some globals for tracking timing information for visualisation
        double fps_tracker = -1.0;
        long t0 = 0;

        unsafe void visualise_tracking(Mat captured_image, ref CLNF face_model, ref FaceModelParameters det_parameters, int frame_count, double fx, double fy, double cx, double cy)
        {
            //Drawing the facial landmarks on the face and the bounding box around it if tracking is successful and initialised
            double detection_certainty = face_model.detection_certainty;
            bool detection_success = face_model.detection_success;

            double visualisation_boundary = 0.2;

            // Only draw if the reliability is reasonable, the value is slightly ad-hoc
            if (detection_certainty < visualisation_boundary)
            {
                LandmarkDetector.Draw(new SWIGTYPE_p_cv__Mat(captured_image.CvPtr), face_model);

                double vis_certainty = detection_certainty;
                if (vis_certainty > 1)
                    vis_certainty = 1;
                if (vis_certainty < -1)
                    vis_certainty = -1;

                vis_certainty = (vis_certainty + 1) / (visualisation_boundary + 1);

                //A rough heuristic for box around the face width

                int thickness = (int)Math.Ceiling(2.0 * ((double)captured_image.Cols) / 640.0);

                Vec6d pose_estimate_to_draw = *((Vec6d*)LandmarkDetector.GetCorrectedPoseWorld(new SWIGTYPE_p_CLNF(CLNF.getCPtr(face_model)), fx, fy, cx, cy).Pointer);

                //Draw it in reddish if uncertain, blueish if certain
                var color = new Scalar((1 - vis_certainty) * 255.0, 0, vis_certainty * 255);
                LandmarkDetector.DrawBox(new SWIGTYPE_p_cv__Mat(captured_image.CvPtr), new SWIGTYPE_p_cv__Vec6d(new IntPtr(&pose_estimate_to_draw)), new SWIGTYPE_p_cv__Scalar(new IntPtr(&(color))), thickness, (float)fx, (float)fy, (float)cx, (float)cy);
            }

            //Work out the framerate
            if (frame_count % 10 == 0)
            {
                double t1 = Cv2.GetTickCount();
                fps_tracker = 10.0 / ((double)(t1 - t0) / Cv2.GetTickFrequency());
                t0 = (long)t1;
            }

            using (Mat resize = captured_image.Resize(new Size(Size.Width * SizeFactor, Size.Height * SizeFactor)))
            {
                string fpsSt = "FPS: " + Math.Round(fps_tracker).ToString("0.0");
                Cv2.PutText(captured_image, fpsSt, new Point(10, 20), HersheyFonts.HersheyPlain, 0.5, new Scalar(0, 255, 0), 2, LineTypes.AntiAlias);
                Cv2.ImShow("tracking_result", resize);
            }
        }

        public override int Run()
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

            //// Do some grabbing
            INFO_STREAM("Attempting to capture from device: " + device);
            using (VideoCapture video_capture = new VideoCapture(device))
            {
                using (Mat dummy = new Mat())
                    video_capture.Read(dummy);

                if (!video_capture.IsOpened())
                {
                    FATAL_STREAM("Failed to open video source");
                    return 1;
                }
                else INFO_STREAM("Device or file opened");

                int frame_count = 0;
                Mat captured_image = new Mat();
                video_capture.Read(captured_image);
                Size = new Size(captured_image.Width / SizeFactor, captured_image.Height / SizeFactor);
                using (var resized_image = captured_image.Resize(Size))
                {
                    // If optical centers are not defined just use center of image
                    if (cx_undefined)
                    {
                        cx = resized_image.Cols / 2.0f;
                        cy = resized_image.Rows / 2.0f;
                    }
                    // Use a rough guess-timate of focal length
                    if (fx_undefined)
                    {
                        fx = (float)(500 * (resized_image.Cols / 640.0));
                        fy = (float)(500 * (resized_image.Rows / 480.0));

                        fx = (float)((fx + fy) / 2.0);
                        fy = fx;
                    }
                }

                // Use for timestamping if using a webcam
                long t_initial = Cv2.GetTickCount();

                INFO_STREAM("Starting tracking");
                while (video_capture.Read(captured_image))
                {
                    using (var resized_image = captured_image.Resize(Size))
                    {
                        // Reading the images
                        MatOfByte grayscale_image = new MatOfByte();

                        if (resized_image.Channels() == 3)
                        {
                            Cv2.CvtColor(resized_image, grayscale_image, ColorConversionCodes.BGR2GRAY);
                        }
                        else
                        {
                            grayscale_image = (MatOfByte)resized_image.Clone();
                        }

                        // The actual facial landmark detection / tracking
                        bool detection_success = LandmarkDetector.DetectLandmarksInVideo(new SWIGTYPE_p_cv__Mat_T_uchar_t(grayscale_image.CvPtr), new SWIGTYPE_p_CLNF(CLNF.getCPtr(clnf_model)), new SWIGTYPE_p_FaceModelParameters(FaceModelParameters.getCPtr(det_parameters)));

                        // Visualising the results
                        // Drawing the facial landmarks on the face and the bounding box around it if tracking is successful and initialised
                        double detection_certainty = clnf_model.detection_certainty;

                        visualise_tracking(resized_image, ref clnf_model, ref det_parameters, frame_count, fx, fy, cx, cy);

                        // detect key presses
                        char character_press = (char)Cv2.WaitKey(1);

                        // restart the tracker
                        if (character_press == 'r')
                        {
                            clnf_model.Reset();
                        }
                        else if (character_press == 'q')
                        {
                            return 0;
                        }

                        // Update the frame count
                        frame_count++;

                        grayscale_image.Dispose();
                        grayscale_image = null;
                    }
                }
            }

            return 0;
        }
    }
}
