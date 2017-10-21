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
                if(_inVideo != value)
                    Reset();
                _inVideo = value;
            }
        }
        
        public double Correct => Model.detection_certainty;

        public List<Point2d> Landmarks { get; private set; }
        public Vec6d PoseVec { get; private set; }
        public double[] Transform { get; private set; } = new double[3];
        public double[] EularRotation { get; private set; } = new double[3];
        public double[] RodriguesRotation { get; private set; } = new double[3];

        private bool needFocusUpdate = true;
        public float FocalX { get; set; }
        public float FocalY { get; set; }
        public float CenterX { get; set; }
        public float CenterY { get; set; }

        public double SizeFactor { get; set; } = 0.5;

        public CLNF Model { get; private set; }
        public FaceModelParameters Parameters { get; private set; }

        public LandmarkDetectorWrap(string modelRoot = "./")
        {
            var argument = new StringList { modelRoot };
            Parameters = new FaceModelParameters(argument);
        }

        public bool DetectROI(Mat mat, Rect roi)
        {
            CheckFocus(mat);
            bool ret = false;
            MatOfByte buff;
            if (mat.Channels() == 1)
            {
                buff = new MatOfByte(mat);
            }
            else
            {
                buff = new MatOfByte();
                Cv2.CvtColor(mat, buff, ColorConversionCodes.BGR2GRAY);
            }

            Rect2d r = new Rect2d(roi.X, roi.Y, roi.Width, roi.Height);

            if (_inVideo)
            {
                ret = LandmarkDetector.DetectLandmarksInVideo(buff.ToSwig(), r.ToSwig(), Model.ToSwig(), Parameters.ToSwig());
            }
            else
            {
                ret = LandmarkDetector.DetectLandmarksInImage(buff.ToSwig(), r.ToSwig(), Model.ToSwig(), Parameters.ToSwig());
            }

            UpdateLandmark();

            buff.Dispose();
            buff = null;
            return ret;
        }

        public bool DetectImage(Mat mat)
        {
            CheckFocus(mat);
            bool ret = false;
            MatOfByte buff;
            if (mat.Channels() == 1)
            {
                buff = new MatOfByte(mat);
            }
            else
            {
                buff = new MatOfByte();
                Cv2.CvtColor(mat, buff, ColorConversionCodes.BGR2GRAY);
            }

            if (_inVideo)
            {
                ret = LandmarkDetector.DetectLandmarksInVideo(buff.ToSwig(), Model.ToSwig(), Parameters.ToSwig());
            }
            else
            {
                ret = LandmarkDetector.DetectLandmarksInImage(buff.ToSwig(), Model.ToSwig(), Parameters.ToSwig());
            }

            UpdateLandmark();

            buff.Dispose();
            buff = null;
            return ret;
        }

        private void CheckFocus(Mat mat)
        {
            if (needFocusUpdate)
            {
                UpdateFocus(mat);
                needFocusUpdate = false;
            }
        }

        public void Draw(Mat mat)
        {
            foreach (var pt in Landmarks)
            {
                mat.DrawMarker((int)pt.X, (int)pt.Y, Scalar.Red, MarkerStyle.CircleAndCross, 10, LineTypes.AntiAlias, 1);
                LandmarkDetector.DrawBox(mat.ToSwig(), PoseVec.ToSwig(), Scalar.Magenta.ToSwig(), 1, FocalX, FocalY, CenterX, CenterY);
            }
        }

        public void Load()
        {
            Model = new CLNF(Parameters.model_location);
        }

        public void Reset()
        {
            if (Model != null)
            {
                Model.Reset();
            }
        }

        public void UpdateFocus(Mat view)
        {
            CenterX = view.Width / 2.0f;
            CenterY = view.Height / 2.0f;

            var fx = 500 * (view.Width / 640.0);
            var fy = 500 * (view.Height / 480.0);
            FocalX = (float)(fx + fy) / 2;
            FocalY = FocalX;
        }

        private void UpdateLandmark()
        {
            Landmarks = new List<Point2d>();
            using (var nativeList = LandmarkDetector.CalculateLandmarks(Model))
            {
                foreach (var item in nativeList)
                {
                    var pt = item.ToPoint2d();
                    Landmarks.Add(pt);
                }
            }

            var nativeTrans = LandmarkDetector.GetCorrectedPoseWorld(Model.ToSwig(), FocalX, FocalY, CenterX, CenterY);
            var trans = nativeTrans.ToVec6d();
            PoseVec = trans;

            Transform[0] = trans[0];
            Transform[1] = trans[1];
            Transform[2] = trans[2];

            EularRotation[0] = trans[3];
            EularRotation[1] = trans[4];
            EularRotation[2] = trans[5];

            var nativeRod = LandmarkDetector.Euler2AxisAngle(new Vec3d(trans[3], trans[4], trans[5]).ToSwig());
            var rod = nativeRod.ToVec3d();

            RodriguesRotation[0] = rod[0];
            RodriguesRotation[1] = rod[1];
            RodriguesRotation[2] = rod[2];
        }

        public void Dispose()
        {
            if (Model != null)
            {
                Model.Dispose();
                Model = null;
            }

            if (Parameters != null)
            {
                Parameters.Dispose();
                Parameters = null;
            }
        }
    }
}
