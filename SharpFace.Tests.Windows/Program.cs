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
            LandmarkTestVid t = new LandmarkTestVid();
            t.Run();
            Console.Write("testfin");
            Console.ReadLine();
        }
    }
}
