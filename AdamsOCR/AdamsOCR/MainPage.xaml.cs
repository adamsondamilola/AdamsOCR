using Stormlion.ImageCropper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;
using Xamarians.CropImage;
using Xamarin.Essentials;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services.Media;

namespace AdamsOCR
{
    public partial class MainPage : ContentPage
    {
        public readonly ITesseractApi _tesseractApi;
        public readonly IDevice _device;

        string PhotoPath { get; set; }

        public MainPage()
        {
            InitializeComponent();
            _tesseractApi = Resolver.Resolve<ITesseractApi>();
            _device = Resolver.Resolve<IDevice>();
        }

        private async void ScanImage_Clicked(object sender, EventArgs e)
        {
            ScanImage.Text = "Loading...";
            ScanImage.IsEnabled = false;

            if (!_tesseractApi.Initialized)
                await _tesseractApi.Init("eng");

            await TakePhotoAsync();

            var photo = PhotoPath;
            //MemoryStream photo = new MemoryStream(byteArray);
            if (photo != null)
            {

                // ImageCropperFunc();
                var cropResult = await CropImageService.Instance.CropImage(PhotoPath, CropRatioType.None);


                _takenImage.Source = cropResult.FilePath; //ImageSource.FromStream(() => photo.Source);

                photo = cropResult.FilePath;
                var _photo = await TakePic(photo);
                var imageBytes = new byte[_photo.Source.Length];

                _photo.Source.Position = 0;
                _photo.Source.Read(imageBytes, 0, (int)_photo.Source.Length);
                _photo.Source.Position = 0;

                var tessResult = await _tesseractApi.SetImage(imageBytes);
                if (tessResult)
                {
                    textFromImage.Text = _tesseractApi.Text;
                }
            }

            ScanImage.Text = "Scan Image";
            ScanImage.IsEnabled = true;

        }

        async Task TakePhotoAsync()
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();

                await LoadPhotoAsync(photo);
                Console.WriteLine($"CapturePhotoAsync COMPLETED: {PhotoPath}");
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature is not supported on the device
            }
            catch (PermissionException pEx)
            {
                // Permissions not granted
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
            }
        }

        async Task LoadPhotoAsync(FileResult photo)
        {
            // canceled
            if (photo == null)
            {
                PhotoPath = null;
                return;
            }
            // save the file into local storage
            var newFile = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
            using (var stream = await photo.OpenReadAsync())
            using (var newStream = File.OpenWrite(newFile))
                await stream.CopyToAsync(newStream);

            PhotoPath = newFile;
        }

        private async Task<MediaFile> TakePic(string imagePath)
        {
            byte[] bytes = File.ReadAllBytes(imagePath);
            var mediaFile = new MediaFile(imagePath, () => { return new MemoryStream(bytes); });

            return mediaFile;
        }
    }
}
