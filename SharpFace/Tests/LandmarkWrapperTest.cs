using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFace.Tests
{
    public class LandmarkWrapperTest : TestBase
    {
        LandmarkDetectorWrap wrap = new LandmarkDetectorWrap();
        VideoCapture capture;

        public LandmarkWrapperTest()
        {
            wrap.Load();
            wrap.InVideo = true;

            capture = new VideoCapture(0);
        }

        public override int Run()
        {
            if (!capture.IsOpened())
                return -1;

            Mat read = new Mat();
            while (capture.Read(read))
            {
                if (!read.Empty())
                {
                    wrap.DetectImage(read);
                    wrap.Draw(read);
                    Cv2.ImShow("test", read);
                }

                char c = (char)Cv2.WaitKey(1);
                switch (c)
                {
                    case 'r':
                        wrap.Reset();
                        break;
                    case 'q':
                        return 0;
                }

            }
            return 0;
        }
    }
}
