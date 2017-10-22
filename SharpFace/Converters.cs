using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFace
{
    public static class Converters
    {
        #region ToManaged

        public unsafe static Point2d ToPoint2d(this SWIGTYPE_p_cv__Point2d point)
        {
            return *(Point2d*)point.Pointer;
        }

        public unsafe static Vec6d ToVec6d(this SWIGTYPE_p_cv__Vec6d vec)
        {
            return *(Vec6d*)vec.Pointer;
        }

        public unsafe static Vec3d ToVec3d(this SWIGTYPE_p_cv__Vec3d vec)
        {
            return *(Vec3d*)vec.Pointer;
        }

        public unsafe static Rect2d ToRect2d(this SWIGTYPE_p_cv__Rect_T_double_t t)
        {
            return *(Rect2d*)t.Pointer;
        }

        #endregion ToManaged

        #region ToSwig

        public unsafe static SWIGTYPE_p_cv__Vec3d ToSwig(this Vec3d vec)
        {
            return new SWIGTYPE_p_cv__Vec3d((IntPtr)(void*)&vec);
        }

        public unsafe static SWIGTYPE_p_cv__Vec6d ToSwig(this Vec6d vec)
        {
            return new SWIGTYPE_p_cv__Vec6d((IntPtr)(void*)&vec);
        }

        public unsafe static SWIGTYPE_p_cv__Scalar ToSwig(this Scalar sc)
        {
            return new SWIGTYPE_p_cv__Scalar((IntPtr)(void*)&sc);
        }

        public unsafe static SWIGTYPE_p_cv__Rect_T_double_t ToSwig(this Rect2d obj)
        {
            return new SWIGTYPE_p_cv__Rect_T_double_t((IntPtr)(void*)&obj);
        }

        public static SWIGTYPE_p_cv__Mat ToSwig(this Mat mat)
        {
            return new SWIGTYPE_p_cv__Mat(mat.CvPtr);
        }

        public static SWIGTYPE_p_CLNF ToSwig(this CLNF clnf)
        {
            return new SWIGTYPE_p_CLNF(CLNF.getCPtr(clnf));
        }

        public static SWIGTYPE_p_FaceModelParameters ToSwig(this FaceModelParameters model)
        {
            return new SWIGTYPE_p_FaceModelParameters(FaceModelParameters.getCPtr(model));
        }

        public static SWIGTYPE_p_cv__Mat_T_uchar_t ToSwig(this MatOfByte obj)
        {
            return new SWIGTYPE_p_cv__Mat_T_uchar_t(obj.CvPtr);
        }

        #endregion ToSwig
    }
}
