using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PSMapa.ViewModels;
using Xamarin.Forms.Maps;
using System.Timers;
using WorkingWithMaps.ViewModels;
using Plugin.Geolocator;
using Xamarin.Essentials;
using System.Threading;
using System.Net.Http;
using PSMapa.BindingModel;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace PSMapa
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlayingPage : ContentPage
    {
        public double TempLat = 37.422;
        public double TempLong = -122.084;
        public double PhoneLocLat;
        public double PhoneLocLong;
        public Stopwatch Clock;
        public PlayingPageViewModel PlayContext;


        public PlayingPage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            Clock = new Stopwatch();
            PlayContext = new PlayingPageViewModel();
            BindingContext = PlayContext;
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(54.3520500, 18.6463700), Distance.FromMiles(5)));
            MessagingCenter.Subscribe<MapsList, string[]>(this, "this", (s, a) =>
            {
                PlayContext.DownlaodMap(a[0]);
                MapNametxt.Text = a[1];
                MapsListBtn.IsEnabled = false;
            });
        }

        protected override void OnAppearing()
        {
            SendResultBtn.IsEnabled = false;
        }

        public void OnMapClicked(object sender, MapClickedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"MapClick: {e.Position.Latitude}, {e.Position.Longitude}");
        }

        private void LeaveGame_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        public void StartBtn_Clicked(object sender, EventArgs e)
        {
            Clock.Start();

            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                Clocktxt.Text = Clock.Elapsed.ToString();
                return true;
            });

            Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                GeoLocalisation_Clicked(sender, e);

                if (PlayContext._locations.Count == 0 )
                {
                    return false;
                }

                return true;
            });

        }


        public async void GeoLocalisation_Clicked(object sender, EventArgs e)
        {

            if (PlayContext._locations.Count == 0)
            {
                Clock.Stop();
                SendResultBtn.IsEnabled = true;
            }

            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High);
                var location = await Geolocation.GetLocationAsync(request);
                if (location != null)
                {

                    PhoneLocLat = Math.Round(location.Latitude, 3, MidpointRounding.ToEven);
                    PhoneLocLong = Math.Round(location.Longitude, 3, MidpointRounding.ToEven);

                        foreach (var variable in PlayContext._locations)
                        {
                            if (PhoneLocLat == Math.Round(variable.Position.Latitude, 3, MidpointRounding.ToEven) && PhoneLocLong == Math.Round(variable.Position.Longitude, 3, MidpointRounding.ToEven))
                            {
                                PlayContext._locations.Remove(variable);
                                break;
                            }
                        }
                }
            }
            catch 
            {
                await DisplayAlert("Geo localization", "error", "OK");
            }
        }

        private void MapslistBtn(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MapsList());
        }

        private void SendResultBtn_Clicked(object s, EventArgs e)
        {
            RaceResult raceresult = new RaceResult();
            raceresult.Time = Clocktxt.Text;
            raceresult.RaceId = Int32.Parse(PlayContext.MapID);
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (message, cert, chain, errors) => { return true; };
            HttpClient client = new HttpClient(clientHandler);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Tokens.UserToken);
            Uri uri = new Uri("https://xamaracing.azurewebsites.net//v1/RaceResults");

            var content = JsonConvert.SerializeObject(raceresult);
            var data = new StringContent(content, Encoding.UTF8, "application/json");
            var response = client.PostAsync(uri, data).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            //Clocktxt.Text = "Clock";
            SendResultBtn.IsEnabled = false;
            Clock.Reset();
            MapNametxt.Text = "Map name";
        }
    }
}