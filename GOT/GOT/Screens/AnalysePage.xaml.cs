using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using GOT.API;
using System.Diagnostics;
using System.IO;
using GOT.Models;

namespace GOT.Screens
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnalysePage : ContentPage
    {
        // Initilizing the Api Service
        ComputerVisionAPI api = new ComputerVisionAPI();
        public MediaFile file { get; set; }
        public AnalysePage()
        {
            InitializeComponent();
            takeBtn.Clicked += TakePicAsync;
        }

        private async Task MoveToResultPage(Recognitionresult result)
        {
            await Navigation.PushAsync(new ResultPage(result));
        }

        private async void TakePicAsync(object sender, EventArgs e)
        {
            // We will be using the Media Plugin
            // Checking if the camera is available
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }
            // Taking a photo
            file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.jpg"
            });

            if (file == null)
                return;
            
            // Displaying the photo
            pic.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                return stream;
            });

            // An ugly way to control the activity indicator since i won't use MVVM
            // I'm sure you can do better !!
            ind.IsVisible = true;
            ind.IsRunning = true;
            ind.IsEnabled = true;
            scanLayout.IsVisible = false;

            var str = file.GetStream();
            
            // Staring the Scan here
            var res = await api.Scan(str);

            file.Dispose();

            ind.IsVisible = false;
            ind.IsRunning = false;
            ind.IsEnabled = false;
            scanLayout.IsVisible = true;

            // Checks if the Api managed to understand your ugly handwriting
            if (res.status.Equals("Succeeded"))
            {
                // If True you will be redirected to the results page
                await MoveToResultPage(res.recognitionResult);
            }

        }

    }
}