
using Foundation;
using Tesseract;
using Tesseract.iOS;
using TinyIoC;
using UIKit;
using XLabs.Ioc;
using XLabs.Ioc.TinyIOC;
using XLabs.Platform.Device;

namespace AdamsOCR.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            var container = TinyIoCContainer.Current;
            container.Register<IDevice>(AppleDevice.CurrentDevice);
            container.Register<ITesseractApi>((cont, parameters) =>
            {
                return new TesseractApi();
            });
            Resolver.SetResolver(new TinyResolver(container));

            Stormlion.ImageCropper.iOS.Platform.Init();
            Xamarians.CropImage.iOS.CropImageServiceIOS.Initialize();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
