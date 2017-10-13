using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFace.Tests
{
    public abstract class TestBase
    {
        protected void INFO_STREAM(string msg)
        {
            Debug.WriteLine(msg);
        }

        protected void FATAL_STREAM(string msg)
        {
            Debug.WriteLine("[ERROR] " + msg);
        }

        protected void WARN_STREAM(string msg)
        {
            Debug.WriteLine("[WARN] " + msg);
        }

        protected int CV_FOURCC(char c1, char c2, char c3, char c4)
        {
            return (((c1) & 255) + (((c2) & 255) << 8) + (((c3) & 255) << 16) + (((c4) & 255) << 24));
        }

        public abstract int Run();
    }
}
