using Android.App;
using Android.Widget;
using Android.OS;

using Debug = System.Diagnostics.Debug;
using System.Threading.Tasks;

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
            SharpFace.NativeTest.Test();

            Button button = FindViewById<Button>(Resource.Id.button1);

            Task.Factory.StartNew(() => 
            {
                button.Text = "Loading";
                TestBase t = new LandmarkWrapperTest(1, "/storage/emulated/0/openface_model/");
                t.Start();
                button.Text = "Processing";
            });
        }
    }
}

