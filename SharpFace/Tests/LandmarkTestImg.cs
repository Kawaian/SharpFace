using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFace.Tests
{
    public class LandmarkTestImg : TestBase
    {


        public override int Run()
        {
            //            //Convert arguments to more convenient vector form
            //            vector<string> arguments = get_arguments(argc, argv);
            var arguments = new StringList { "./" };

            //            // Search paths
            //            boost::filesystem::path config_path = boost::filesystem::path(CONFIG_DIR);
            //            boost::filesystem::path parent_path = boost::filesystem::path(arguments[0]).parent_path();
            var configPath = "some/config/path";
            var parentPath = arguments[0];


            //            // Some initial parameters that can be overriden from command line
            //            vector<string> files, output_images, output_landmark_locations, output_pose_locations;

            StringList files = new StringList(), 
                       outputImages = new StringList(), 
                       outputLandmarkLocations = new StringList(),
                       outputPoseLocations = new StringList();

            //            // Bounding boxes for a face in each image (optional)
            //            vector<cv::Rect_<double>> bounding_boxes;
            CVDoubleRectList boundingBoxes = new CVDoubleRectList();

            //            LandmarkDetector::get_image_input_output_params(files, output_landmark_locations, output_pose_locations, output_images, bounding_boxes, arguments);
            LandmarkDetector.get_image_input_output_params(files, outputLandmarkLocations, outputPoseLocations, outputImages, boundingBoxes, arguments);

            //            LandmarkDetector::FaceModelParameters det_parameters(arguments);
            FaceModelParameters detParameters = new FaceModelParameters(arguments);

            //            // No need to validate detections, as we're not doing tracking
            //            det_parameters.validate_detections = false;
            detParameters.validate_detections = false;

            //            // Grab camera parameters if provided (only used for pose and eye gaze and are quite important for accurate estimates)
            //            float fx = 0, fy = 0, cx = 0, cy = 0;
            //            int device = -1;
            //            LandmarkDetector::get_camera_params(device, fx, fy, cx, cy, arguments);
            float fx = 0, fy = 0, cx = 0, cy = 0;
            int device = -1;
            LandmarkDetector.get_camera_params(out device, out fx, out fy, out cx, out cy, arguments);

            //            // If cx (optical axis centre) is undefined will use the image size/2 as an estimate
            //            bool cx_undefined = false;
            //            bool fx_undefined = false;
            //            if (cx == 0 || cy == 0)
            //            {
            //                cx_undefined = true;
            //            }
            //            if (fx == 0 || fy == 0)
            //            {
            //                fx_undefined = true;
            //            }

            bool cx_undefined = false;
            bool fx_undefined = false;

            if (cx == 0 || cy == 0)
                cx_undefined = true;
            if (fx == 0 || fy == 0)
                fx_undefined = true;


            //            // The modules that are being used for tracking
            //            cout << "Loading the model" << endl;
            //            LandmarkDetector::CLNF clnf_model(det_parameters.model_location);
            //            cout << "Model loaded" << endl;

            INFO_STREAM("Loading the model");
            CLNF clnfModel = new CLNF(detParameters.model_location);
            INFO_STREAM("Model loaded");

            // TODO: dlib::frontal_face_detector ??? NANI ???

            //            cv::CascadeClassifier classifier(det_parameters.face_detector_location);
            //            dlib::frontal_face_detector face_detector_hog = dlib::get_frontal_face_detector();
            CascadeClassifier classifier = new CascadeClassifier(detParameters.face_detector_location);
            SWIGTYPE_p_dlib__frontal_face_detector faceDetectorHog;

            //            // Loading the AU prediction models
            //            string au_loc = "AU_predictors/AU_all_static.txt";
            string auLoc = "AU_predictors/AU_all_static.txt";

            //            boost::filesystem::path au_loc_path = boost::filesystem::path(au_loc);
            

            //            if (boost::filesystem::exists(au_loc_path))
            //            {
            //                au_loc = au_loc_path.string();
            //            }
            //            else if (boost::filesystem::exists(parent_path / au_loc_path))
            //            {
            //                au_loc = (parent_path / au_loc_path).string();
            //            }
            //            else if (boost::filesystem::exists(config_path / au_loc_path))
            //            {
            //                au_loc = (config_path / au_loc_path).string();
            //            }
            //            else
            //            {
            //                cout << "Can't find AU prediction files, exiting" << endl;
            //                return 1;
            //            }

            //            // Used for image masking for AUs
            //            string tri_loc;
            //            boost::filesystem::path tri_loc_path = boost::filesystem::path("model/tris_68_full.txt");
            //            if (boost::filesystem::exists(tri_loc_path))
            //            {
            //                tri_loc = tri_loc_path.string();
            //            }
            //            else if (boost::filesystem::exists(parent_path / tri_loc_path))
            //            {
            //                tri_loc = (parent_path / tri_loc_path).string();
            //            }
            //            else if (boost::filesystem::exists(config_path / tri_loc_path))
            //            {
            //                tri_loc = (config_path / tri_loc_path).string();
            //            }
            //            else
            //            {
            //                cout << "Can't find triangulation files, exiting" << endl;
            //                return 1;
            //            }

            //            FaceAnalysis::FaceAnalyser face_analyser(vector<cv::Vec3d>(), 0.7, 112, 112, au_loc, tri_loc);

            //            bool visualise = !det_parameters.quiet_mode;

            //            // Do some image loading
            //            for (size_t i = 0; i < files.size(); i++)
            //            {
            //                string file = files.at(i);

            //                // Loading image
            //                cv::Mat read_image = cv::imread(file, -1);

            //                if (read_image.empty())
            //                {
            //                    cout << "Could not read the input image" << endl;
            //                    return 1;
            //                }

            //                // Making sure the image is in uchar grayscale
            //                cv::Mat_<uchar> grayscale_image;
            //                convert_to_grayscale(read_image, grayscale_image);


            //                // If optical centers are not defined just use center of image
            //                if (cx_undefined)
            //                {
            //                    cx = grayscale_image.cols / 2.0f;
            //                    cy = grayscale_image.rows / 2.0f;
            //                }
            //                // Use a rough guess-timate of focal length
            //                if (fx_undefined)
            //                {
            //                    fx = 500 * (grayscale_image.cols / 640.0);
            //                    fy = 500 * (grayscale_image.rows / 480.0);

            //                    fx = (fx + fy) / 2.0;
            //                    fy = fx;
            //                }


            //                // if no pose defined we just use a face detector
            //                if (bounding_boxes.empty())
            //                {

            //                    // Detect faces in an image
            //                    vector<cv::Rect_<double>> face_detections;

            //                    if (det_parameters.curr_face_detector == LandmarkDetector::FaceModelParameters::HOG_SVM_DETECTOR)
            //                    {
            //                        vector<double> confidences;
            //                        LandmarkDetector::DetectFacesHOG(face_detections, grayscale_image, face_detector_hog, confidences);
            //                    }
            //                    else
            //                    {
            //                        LandmarkDetector::DetectFaces(face_detections, grayscale_image, classifier);
            //                    }

            //                    // Detect landmarks around detected faces
            //                    int face_det = 0;
            //                    // perform landmark detection for every face detected
            //                    for (size_t face = 0; face < face_detections.size(); ++face)
            //                    {
            //                        // if there are multiple detections go through them
            //                        bool success = LandmarkDetector::DetectLandmarksInImage(grayscale_image, face_detections[face], clnf_model, det_parameters);

            //                        // Estimate head pose and eye gaze				
            //                        cv::Vec6d headPose = LandmarkDetector::GetCorrectedPoseWorld(clnf_model, fx, fy, cx, cy);

            //                        // Gaze tracking, absolute gaze direction
            //                        cv::Point3f gazeDirection0(0, 0, -1);
            //            cv::Point3f gazeDirection1(0, 0, -1);

            //            if (success && det_parameters.track_gaze)
            //            {
            //                FaceAnalysis::EstimateGaze(clnf_model, gazeDirection0, fx, fy, cx, cy, true);
            //                FaceAnalysis::EstimateGaze(clnf_model, gazeDirection1, fx, fy, cx, cy, false);

            //            }

            //            auto ActionUnits = face_analyser.PredictStaticAUs(read_image, clnf_model, false);

            //            // Writing out the detected landmarks (in an OS independent manner)
            //            if (!output_landmark_locations.empty())
            //            {
            //                char name[100];
            //                // append detection number (in case multiple faces are detected)
            //                sprintf(name, "_det_%d", face_det);

            //                // Construct the output filename
            //                boost::filesystem::path slash("/");
            //                std::string preferredSlash = slash.make_preferred().string();

            //                boost::filesystem::path out_feat_path(output_landmark_locations.at(i));
            //                boost::filesystem::path dir = out_feat_path.parent_path();
            //                boost::filesystem::path fname = out_feat_path.filename().replace_extension("");
            //                boost::filesystem::path ext = out_feat_path.extension();
            //                string outfeatures = dir.string() + preferredSlash + fname.string() + string(name) + ext.string();
            //                write_out_landmarks(outfeatures, clnf_model, headPose, gazeDirection0, gazeDirection1, ActionUnits.first, ActionUnits.second);
            //            }

            //            if (!output_pose_locations.empty())
            //            {
            //                char name[100];
            //                // append detection number (in case multiple faces are detected)
            //                sprintf(name, "_det_%d", face_det);

            //                // Construct the output filename
            //                boost::filesystem::path slash("/");
            //                std::string preferredSlash = slash.make_preferred().string();

            //                boost::filesystem::path out_pose_path(output_pose_locations.at(i));
            //                boost::filesystem::path dir = out_pose_path.parent_path();
            //                boost::filesystem::path fname = out_pose_path.filename().replace_extension("");
            //                boost::filesystem::path ext = out_pose_path.extension();
            //                string outfeatures = dir.string() + preferredSlash + fname.string() + string(name) + ext.string();
            //                write_out_pose_landmarks(outfeatures, clnf_model.GetShape(fx, fy, cx, cy), headPose, gazeDirection0, gazeDirection1);

            //            }

            //            if (det_parameters.track_gaze)
            //            {
            //                cv::Vec6d pose_estimate_to_draw = LandmarkDetector::GetCorrectedPoseWorld(clnf_model, fx, fy, cx, cy);

            //                // Draw it in reddish if uncertain, blueish if certain
            //                LandmarkDetector::DrawBox(read_image, pose_estimate_to_draw, cv::Scalar(255.0, 0, 0), 3, fx, fy, cx, cy);
            //                FaceAnalysis::DrawGaze(read_image, clnf_model, gazeDirection0, gazeDirection1, fx, fy, cx, cy);
            //            }

            //            // displaying detected landmarks
            //            cv::Mat display_image;
            //            create_display_image(read_image, display_image, clnf_model);

            //            if (visualise && success)
            //            {
            //                imshow("colour", display_image);
            //                cv::waitKey(1);
            //            }

            //            // Saving the display images (in an OS independent manner)
            //            if (!output_images.empty() && success)
            //            {
            //                string outimage = output_images.at(i);
            //                if (!outimage.empty())
            //                {
            //                    char name[100];
            //                    sprintf(name, "_det_%d", face_det);

            //                    boost::filesystem::path slash("/");
            //                    std::string preferredSlash = slash.make_preferred().string();

            //                    // append detection number
            //                    boost::filesystem::path out_feat_path(outimage);
            //                    boost::filesystem::path dir = out_feat_path.parent_path();
            //                    boost::filesystem::path fname = out_feat_path.filename().replace_extension("");
            //                    boost::filesystem::path ext = out_feat_path.extension();
            //                    outimage = dir.string() + preferredSlash + fname.string() + string(name) + ext.string();
            //                    create_directory_from_file(outimage);
            //                    bool write_success = cv::imwrite(outimage, display_image);

            //                    if (!write_success)
            //                    {
            //                        cout << "Could not output a processed image" << endl;
            //                        return 1;
            //                    }

            //                }

            //            }

            //            if (success)
            //            {
            //                face_det++;
            //            }

            //        }
            //    }
            //		else
            //		{
            //			// Have provided bounding boxes
            //			LandmarkDetector::DetectLandmarksInImage(grayscale_image, bounding_boxes[i], clnf_model, det_parameters);

            //			// Estimate head pose and eye gaze				
            //			cv::Vec6d headPose = LandmarkDetector::GetCorrectedPoseWorld(clnf_model, fx, fy, cx, cy);

            //    // Gaze tracking, absolute gaze direction
            //    cv::Point3f gazeDirection0(0, 0, -1);
            //    cv::Point3f gazeDirection1(0, 0, -1);

            //			if (det_parameters.track_gaze)
            //			{
            //				FaceAnalysis::EstimateGaze(clnf_model, gazeDirection0, fx, fy, cx, cy, true);
            //				FaceAnalysis::EstimateGaze(clnf_model, gazeDirection1, fx, fy, cx, cy, false);
            //			}

            //auto ActionUnits = face_analyser.PredictStaticAUs(read_image, clnf_model, false);

            //			// Writing out the detected landmarks
            //			if(!output_landmark_locations.empty())
            //			{
            //				string outfeatures = output_landmark_locations.at(i);

            //                write_out_landmarks(outfeatures, clnf_model, headPose, gazeDirection0, gazeDirection1, ActionUnits.first, ActionUnits.second);
            //			}

            //			// Writing out the detected landmarks
            //			if (!output_pose_locations.empty())
            //			{
            //				string outfeatures = output_pose_locations.at(i);

            //                write_out_pose_landmarks(outfeatures, clnf_model.GetShape(fx, fy, cx, cy), headPose, gazeDirection0, gazeDirection1);
            //			}

            //			// displaying detected stuff
            //			cv::Mat display_image;

            //			if (det_parameters.track_gaze)
            //			{
            //				cv::Vec6d pose_estimate_to_draw = LandmarkDetector::GetCorrectedPoseWorld(clnf_model, fx, fy, cx, cy);

            //// Draw it in reddish if uncertain, blueish if certain
            //LandmarkDetector::DrawBox(read_image, pose_estimate_to_draw, cv::Scalar(255.0, 0, 0), 3, fx, fy, cx, cy);
            //				FaceAnalysis::DrawGaze(read_image, clnf_model, gazeDirection0, gazeDirection1, fx, fy, cx, cy);
            //			}


            //            create_display_image(read_image, display_image, clnf_model);

            //			if(visualise)
            //			{

            //                imshow("colour", display_image);
            //cv::waitKey(1);
            //			}

            //			if(!output_images.empty())
            //			{
            //				string outimage = output_images.at(i);
            //				if(!outimage.empty())
            //				{

            //                    create_directory_from_file(outimage);
            //bool write_success = imwrite(outimage, display_image);	

            //					if (!write_success)
            //					{
            //						cout << "Could not output a processed image" << endl;
            //						return 1;
            //					}
            //				}
            //			}
            //		}				

            //	}

            return 0;
        }
    }
}
