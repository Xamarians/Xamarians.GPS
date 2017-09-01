using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarians.GPS;
using Xamarin.Forms;

namespace GPSTestApp
{
	public partial class MainPage : ContentPage
	{
        Label location;
        Label locationOnPositionChanged;
        Position currentPosition;
        public MainPage()
		{
            location = new Label() { HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center };
            locationOnPositionChanged = new Label() { Text = "Changed Position - getting.... ", TextColor = Color.Red, HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center };
            var btnGetLocation = new Button()
            {
                Text = "Get Location",
                BackgroundColor = Color.Blue,
                TextColor = Color.White,
                WidthRequest = 300,
                HorizontalOptions = LayoutOptions.Center
            };
            btnGetLocation.Clicked += BtnGetLocation_Clicked;

            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Children =
                    {
                        location,
                        btnGetLocation,
                        new BoxView() { HeightRequest=1, BackgroundColor=Color.FromHex("#999999"), HorizontalOptions=LayoutOptions.FillAndExpand},
                        locationOnPositionChanged,
                        new BoxView() { HeightRequest=1, BackgroundColor=Color.FromHex("#999999"), HorizontalOptions=LayoutOptions.FillAndExpand},
                    }
            };
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            GPSService.Instance.StartListening(1000, 5);
            GPSService.Instance.PositionChanged += Instance_PositionChanged;
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            GPSService.Instance.StopListening();
            GPSService.Instance.PositionChanged -= Instance_PositionChanged;
        }

        private async void BtnGetLocation_Clicked(object sender, EventArgs e)
        {
            var isGpsEnable = GPSService.Instance.IsGeolocationEnabled;
            if (isGpsEnable)
            {
                currentPosition = await GetCurrentLocation();
                location.Text = currentPosition.Latitude == 0 && currentPosition.Longitude == 0 ?
                    "Location not available" : $"Coordinates: {currentPosition.Latitude}, {currentPosition.Longitude}";
            }
            else
            {
                GPSService.Instance.CheckAndEnableGPS("Please enable GPS to detect your current location.");
                currentPosition = await GetCurrentLocation();
                location.Text = currentPosition.Latitude == 0 && currentPosition.Longitude == 0 ?
                    "Location not available" : $"Coordinates: {currentPosition.Latitude}, {currentPosition.Longitude}";
            }
        }

        private async Task<Position> GetCurrentLocation()
        {
            GPSService.Instance.DesiredAccuracy = 100;
            var postion = await GPSService.Instance.GetPositionAsync(CancellationToken.None);
            if (postion != null)
                return postion;
            else
                return new Position() { Latitude = 0, Longitude = 0 };
        }

        bool isEvenCount;
        private void Instance_PositionChanged(object sender, PositionEventArgs e)
        {
            isEvenCount = !isEvenCount;
            //App.Current.MainPage.DisplayAlert("Warning", "Position Changed", "", "Ok");

            if (e.Position != null)
            {
                locationOnPositionChanged.TextColor = isEvenCount ? Color.Blue : Color.Red;
                locationOnPositionChanged.Text = e.Position.Latitude == 0 && e.Position.Longitude == 0 ?
                  "Location not available" : $"Changed  Position: {e.Position.Latitude}, {e.Position.Longitude}";
            }
            else
            {
                App.Current.MainPage.DisplayAlert("Error", "Failed to get current location. Make sure you have turned on location service on the device.", "", "Ok");
            }
        }
    }
}
	