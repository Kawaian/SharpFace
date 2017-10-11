// Swig interface file to generate C# wrappers for LandmarkDetector
%module LandmarkDetector

%include <typemaps.i>
%include <std_vector.i>
%include <std_pair.i>
%include <std_map.i>
%include <std_string.i>


// Lazy @MattMatt2000, need to review that :P
// "Nah, I'll do it the next day"
%include "modules/std_string_ref.i"


// Brokenu :P
/*
// Primitive data types passed as references
%apply int& 		OUTPUT {int& resultPointer};
%apply bool& 		OUTPUT {bool& resultPointer};
%apply float& 		OUTPUT {float& resultPointer};
%apply double& 		OUTPUT {double& resultPointer};

%apply int* 		OUTPUT {int* resultPointer};
%apply bool* 		OUTPUT {bool* resultPointer};
%apply float* 		OUTPUT {float* resultPointer};
%apply double* 		OUTPUT {double* resultPointer};
*/

%{
// ------------- C++ API ------------- //

#include "CCNF_patch_expert.h"
#include "LandmarkDetectionValidator.h"
#include "LandmarkDetectorFunc.h"
#include "LandmarkDetectorModel.h"
#include "LandmarkDetectorParameters.h"
#include "LandmarkDetectorUtils.h"
#include "Patch_experts.h"
#include "PAW.h"
#include "PDM.h"
#include "SVR_patch_expert.h"

using namespace LandmarkDetector;
%}

%include "CCNF_patch_expert.h"
%include "LandmarkDetectionValidator.h"
%include "LandmarkDetectorFunc.h"
%include "LandmarkDetectorModel.h"
%include "LandmarkDetectorParameters.h"
%include "LandmarkDetectorUtils.h"
%include "Patch_experts.h"
%include "PDM.h"
%include "SVR_patch_expert.h"

// Passed by value

%template(IntList)                 std::vector<int>;
%template(FloatList)               std::vector<float>;
%template(DoubleList)              std::vector<double>;
%template(StringList)              std::vector<std::string>;

%template(IntList2N)               std::vector<std::vector<int>>;
%template(FloatList2N)             std::vector<std::vector<float>>;

%template(FloatList3N)             std::vector<std::vector<std::vector<float>>>;

%template(CVPointPair)             std::pair<cv::Point, cv::Point>;
%template(CVPointList)             std::vector<cv::Point>;
%template(CVPointPairList)        std::vector<std::pair<cv::Point, cv::Point>>;

%template(CVPoint2DPair)           std::pair<cv::Point2d, cv::Point2d>;
%template(CVPoint2DList)           std::vector<cv::Point2d>;
%template(CVPoint2DPairList)      std::vector<std::pair<cv::Point2d, cv::Point2d>>;

%template(CVDoubleRectList)        std::vector<cv::Rect_<double>>;

%template(FloatMatList)            std::vector<cv::Mat_<float>>;
%template(DoubleMatList)           std::vector<cv::Mat_<double>>;
%template(IntMatList)              std::vector<cv::Mat_<int>>;

%template(Int_IntMatMap)           std::map<int, cv::Mat_<int>>;
%template(Int_DoubleMatMap)        std::map<int, cv::Mat_<double>>;

%template(Float_FloatMatMap)       std::map<float, cv::Mat_<float>>;
%template(Double_DoubleMatMap)     std::map<double, cv::Mat_<double>>;

%template(IntMatList2N)            std::vector<std::vector<cv::Mat_<int>>>;
%template(DoubleMatList2N)         std::vector<std::vector<cv::Mat_<double>>>;
%template(FloatMatList2N)          std::vector<std::vector<cv::Mat_<float>>>;
