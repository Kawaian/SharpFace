using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFace.Tests.Windows
{
    class Program
    {
        static void Main(string[] args)
        {
            OpenCvSharp.Windows.NativeBindings.Init();
            SharpFace.Windows.Native.Init();

            TestBase t =
                //new LandmarkTestVid();
                new LandmarkWrapperTest();
            t.Run();

            Console.Write($"======== Test Finished =======");
            Console.Read();
        }
    }
}
