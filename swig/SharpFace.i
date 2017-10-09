// Swig interface file to generate C# wrappers for LandmarkDetector
%module LaandmrkDetector

%{
    // C++ API

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
%}

%include <typemaps.i>
%include <std_vector.i>
%include <std_pair.i>
%include <std_map.i>

%include "std_string.i"

%include "CCNF_patch_expert.h"
%include "LandmarkDetectionValidator.h"
%include "LandmarkDetectorFunc.h"
%include "LandmarkDetectorModel.h"
%include "LandmarkDetectorParameters.h"
%include "LandmarkDetectorUtils.h"
%include "Patch_experts.h"
%include "PDM.h"
%include "SVR_patch_expert.h"

%template(Int_List)                 std::vector<int>;
%template(Float_List)               std::vector<float>;
%template(Double_List)              std::vector<double>;
%template(String_List)              std::vector<std::string>;

%template(CVPoint_Pair)             std::pair<cv::Point, cv::Point>;
%template(CVPoint_List)             std::vector<cv::Point>;
%template(CVPoint_Pair_List)        std::vector<std::pair<cv::Point, cv::Point>>;

%template(CVPoint2D_Pair)           std::pair<cv::Point2d, cv::Point2d>;
%template(CVPoint2D_List)           std::vector<cv::Point2d>;
%template(CVPoint2D_Pair_List)      std::vector<std::pair<cv::Point2d, cv::Point2d>>;

%template(CVDoubleRect_List)        std::vector<cv::Rect_<double>>;

%template(Int_List2N)               std::vector<std::vector<int>>;
%template(Float_List2N)             std::vector<std::vector<float>>;

%template(FloatMat_List)            std::vector<cv::Mat<float>>;
%template(DoubleMat_List)           std::vector<cv::Mat<double>>;
%template(IntMat_List)              std::vector<cv::Mat<int>>;

%template(Int_DoubleMat_Map)        std::map<int, cv::Mat<int>>;
%template(Float_DoubleMat_Map)      std::map<float, cv::Mat<float>>;
%template(Double_DoubleMat_Map)     std::map<double, cv::Mat<double>>;

%template(IntMat_List2N)            std::vector<std::vector<cv::Mat<int>>>;
%template(DoubleMat_List2N)         std::vector<std::vector<cv::Mat<double>>>;
%template(FloatMat_List2N)          std::vector<std::vector<cv::Mat<float>>>;
