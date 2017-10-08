using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFace.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Windows.Native.Init();

            var t = new Test();
            t.Invoke();
            Console.Write("fin");
            Console.ReadLine();
        }
    }
}
