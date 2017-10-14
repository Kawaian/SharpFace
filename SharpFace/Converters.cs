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
        public unsafe static Point2d ToPoint2d(this SWIGTYPE_p_cv__Point2d point)
        {
            return *(Point2d*)(point.Pointer);
        }
    }
}
