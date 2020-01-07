using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using WorkingWithMaps.ViewModels;
using System;
using PSMapa;
using System.Diagnostics;
using System.ComponentModel;
using System.Net.Http;
using PSMapa.BindingModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace PSMapa.ViewModels
{
    public class PlayingPageViewModel
    {
        public RaceMapBindingModel RaceMap = new RaceMapBindingModel();
        public string MapID;
        public int _pinCreatedCount = 0;
        public ObservableCollection<Location> _locations;

        public IEnumerable Locations => _locations;
        public ICommand DownloadMapCommand { get; }


        public PlayingPageViewModel()
        {
            _locations = new ObservableCollection<Location>();
        }

        public async void DownlaodMap(string id)
        {
            MapID = id;
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient(clientHandler);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Tokens.UserToken);
            Uri FirstRaceMapsURI = new Uri("https://10.0.2.2:44353/v1/RaceMaps/");
            Uri ConcatRaceMapsURI = new Uri(FirstRaceMapsURI, id);
            var content = await client.GetStringAsync(ConcatRaceMapsURI);
            RaceMap = JsonConvert.DeserializeObject<RaceMapBindingModel>(content);
            
            
            foreach (var item in RaceMap.RaceCheckpoints)
            {
                Geocoder geoCoder = new Geocoder();
                var address = "";
                var PossibleAddress = await geoCoder.GetAddressesForPositionAsync(new Position(item.Latitude, item.Longitude));
                foreach (var a in PossibleAddress)
                {
                    address += a;
                }
                _locations.Add(new Location($"{item.NumberInOrder}", $"{address}", new Position(item.Latitude, item.Longitude)));
            }

        }

    }
}
