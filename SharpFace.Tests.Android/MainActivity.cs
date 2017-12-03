using Android.App;
using Android.Widget;
using Android.OS;

using Debug = System.Diagnostics.Debug;

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
            Debug.WriteLine("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF");
            SharpFace.Android.Native.Init();

            Debug.WriteLine("UUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUU");
            SharpFace.NativeTest.Test();
            Debug.WriteLine("CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC");

            Button button = FindViewById<Button>(Resource.Id.button1);
            Debug.WriteLine("KKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKK");
            
            TestBase t = new LandmarkWrapperTest(1, "/storage/emulated/0/openface_model/");
            //t.Start();
            Debug.WriteLine("UUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUU");
        }
    }
}

