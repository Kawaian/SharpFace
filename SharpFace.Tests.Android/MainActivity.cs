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

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Run LandmarkDetector tests
            SharpFace.Android.Native.Init();

            TestBase t =
                //new LandmarkTestVid();
                new LandmarkWrapperTest();
            var ret = t.Run();

            // Console.Write($"======== Test Finished {ret} =======");
            // Console.Read();
        }
    }
}

