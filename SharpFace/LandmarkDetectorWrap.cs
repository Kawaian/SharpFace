using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFace
{
    public class LandmarkDetectorWrap : IDisposable
    {
        private bool _inVideo = false;
        public bool InVideo
        {
            get => _inVideo;
            set
            {
                Reset();
                _inVideo = value;
            }
        }

        public CLNF Model { get; private set; }

        public LandmarkDetectorWrap()
        {

        }

        public CLNF DetectFace(Mat mat)
        {
            return Model;
        }

        public CLNF DetectImage(Mat mat)
        {
            if (_inVideo)
            {
                
            }
            else
            {

            }

            return Model;
        }

        public void Draw(Mat mat)
        {
            var list = LandmarkDetector.CalculateLandmarks(Model);
            foreach (var item in list)
            {
                var pt = item.ToPoint2d();
            }
        }

        public void Load()
        {
            Model = new CLNF();
        }

        public void Reset()
        {
            if (Model != null)
            {
                Model.Reset();
            }
        }

        public void Dispose()
        {
            if(Model != null)
            {
                Model.Dispose();
                Model = null;
            }
        }
    }
}
