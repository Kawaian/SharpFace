using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace SharpFace
{
    public class Test
    {
        public void Invoke()
        {
            LandmarkDetector.CalculateBox(null, 0, 0, 0, 0);
        }
    }
}