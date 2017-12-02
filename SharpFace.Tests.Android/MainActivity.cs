using Android.App;
using Android.Widget;
using Android.OS;

namespace SharpFace.Tests.Android
{
    [Activity(Label = "SharpFace.Tests.Android", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.Main);

            ImageView imgView = FindViewById<ImageView>(Resource.Id.imageView1);
            OpenCvSharp.Android.NativeBinding.Init(this, this, imgView);
            SharpFace.Android.Native.Init();

            Button button = FindViewById<Button>(Resource.Id.button1);

            TestBase t = new LandmarkWrapperTest();
            t.Start();
        }
    }
}

