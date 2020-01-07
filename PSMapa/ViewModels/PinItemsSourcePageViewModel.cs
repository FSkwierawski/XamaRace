using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using WorkingWithMaps.ViewModels;
using System;
using PSMapa;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using PSMapa.BindingModel;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace WorkingWithMaps.ViewModels
{

      public class PinItemsSourcePageViewModel
    {


        public int _pinCreatedCount = 0;
        public ObservableCollection<Location> _locations;
        public string MapName = "";


        public IEnumerable Locations => _locations;

        //public ICommand AddLocationCommand { get; }
        public ICommand RemoveLocationCommand { get; }
        public ICommand ClearLocationsCommand { get; }
        //public ICommand UpdateLocationsCommand { get; }
        //public ICommand ReplaceLocationCommand { get; }
        public ICommand UploadMapCommand { get; set; }

        public PinItemsSourcePageViewModel()
        {
             _locations = new ObservableCollection<Location>()
            {

            };

            RemoveLocationCommand = new Command(RemoveLocation);
            ClearLocationsCommand = new Command(ClearLocations);
            UploadMapCommand = new Command(UploadMapAsync);
        }

        public async void UploadMapAsync()
        {
            var CheckPoints = new List<RaceCheckpoint>();
            int LatLongTemp = 0;

            RaceMapBindingModel raceMap = new RaceMapBindingModel();

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient(clientHandler);

            foreach (Location variable in _locations)
            {
                RaceCheckpoint checkpoint = new RaceCheckpoint();
                checkpoint.Latitude = variable.Position.Latitude;
                checkpoint.Longitude = variable.Position.Longitude;
                checkpoint.NumberInOrder = LatLongTemp + 1;
                CheckPoints.Add(checkpoint);
                LatLongTemp += 1;
            }
            raceMap.Name = MapName;
            raceMap.Description = "TestOpis";
            raceMap.RaceCheckpoints = CheckPoints;
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (message, cert, chain, errors) => { return true; };
            var url = "https://10.0.2.2:44353/v1/RaceMaps";
            var JSONcontent = JsonConvert.SerializeObject(raceMap);
            var data = new StringContent(JSONcontent, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Tokens.UserToken);
            var response = await client.PostAsync(url, data);
            string result = response.Content.ReadAsStringAsync().Result;
        }

        void ClearLocations()
        {
            _locations.Clear();
            _pinCreatedCount = 0;
        }

        void RemoveLocation()
        {
            if (_locations.Any())
            {
                _locations.Remove(_locations.Last());
                _pinCreatedCount -= 1;
            }
        }
      }
}
