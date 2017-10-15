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

#ifndef __PDM_h_
#define __PDM_h_

// OpenCV includes
#include <opencv2/core/core.hpp>

#include "LandmarkDetectorParameters.h"


#ifdef SWIG
#define OPENFACE_API
#endif

namespace LandmarkDetector
{
//===========================================================================
// A linear 3D Point Distribution Model (constructed using Non-Rigid structure from motion or PCA)
// Only describes the model but does not contain an instance of it (no local or global parameters are stored here)
// Contains the utility functions to help manipulate the model

class OPENFACE_API PDM{
	public:
    
		// The 3D mean shape vector of the PDM [x1,..,xn,y1,...yn,z1,...,zn]
		cv::Mat_<double> mean_shape;	
  
		// Principal components or variation bases of the model, 
		cv::Mat_<double> princ_comp;	

		// Eigenvalues (variances) corresponding to the bases
		cv::Mat_<double> eigen_values;	

		PDM(){;}
		
		// A copy constructor
		PDM(const PDM& other);
			
		void Read(string location);

		// Number of vertices
		inline int NumberOfPoints() const {return mean_shape.rows/3;}
		
		// Listing the number of modes of variation
		inline int NumberOfModes() const {return princ_comp.cols;}

		void Clamp(cv::Mat_<float>& params_local, cv::Vec6d& params_global, const FaceModelParameters& params);

		// Compute shape in object space (3D)
		void CalcShape3D(cv::Mat_<double>& out_shape, const cv::Mat_<double>& params_local) const;

		// Compute shape in image space (2D)
		void CalcShape2D(cv::Mat_<double>& out_shape, const cv::Mat_<double>& params_local, const cv::Vec6d& params_global) const;
    
		// provided the bounding box of a face and the local parameters (with optional rotation), generates the global parameters that can generate the face with the provided bounding box
		void CalcParams(cv::Vec6d& out_params_global, const cv::Rect_<double>& bounding_box, const cv::Mat_<double>& params_local, const cv::Vec3d rotation = cv::Vec3d(0.0));

		// Provided the landmark location compute global and local parameters best fitting it (can provide optional rotation for potentially better results)
		void CalcParams(cv::Vec6d& out_params_global, const cv::Mat_<double>& out_params_local, const cv::Mat_<double>& landmark_locations, const cv::Vec3d rotation = cv::Vec3d(0.0));

		// provided the model parameters, compute the bounding box of a face
		void CalcBoundingBox(cv::Rect& out_bounding_box, const cv::Vec6d& params_global, const cv::Mat_<double>& params_local);

		// Helpers for computing Jacobians, and Jacobians with the weight matrix
		void ComputeRigidJacobian(const cv::Mat_<float>& params_local, const cv::Vec6d& params_global, cv::Mat_<float> &Jacob, const cv::Mat_<float> W, cv::Mat_<float> &Jacob_t_w);
		void ComputeJacobian(const cv::Mat_<float>& params_local, const cv::Vec6d& params_global, cv::Mat_<float> &Jacobian, const cv::Mat_<float> W, cv::Mat_<float> &Jacob_t_w);

		// Given the current parameters, and the computed delta_p compute the updated parameters
		void UpdateModelParameters(const cv::Mat_<float>& delta_p, cv::Mat_<float>& params_local, cv::Vec6d& params_global);

  };
  //===========================================================================
}
#endif
