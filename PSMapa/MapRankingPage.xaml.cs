using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PSMapa.BindingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PSMapa
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapRankingPage : ContentPage
    {
        public List<RaceResultGet> jsonlist = new List<RaceResultGet>();
        public string content;
        public TimeSpan[] timeSpan = new TimeSpan[100];
        public int numerator = 0;
        public List<MapsListView> Maps = new List<MapsListView>();
        public class MapsListView
        {
            public MapsListView(int? id, string time, Color favoriteColor, string userName)
            {
                this.Time = time;
                this.FavoriteColor = favoriteColor;
                this.IdValue = id;
                this.UserName = userName;
            }
            public string UserName { get; set; }
            public int? IdValue { get; set; }

            public string Time { private set; get; }

            public Color FavoriteColor { private set; get; }
        };

        public void DownloadMapsFromServer(string MapId)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient(clientHandler);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Tokens.UserToken);
            UriBuilder address = new UriBuilder("https", "xamaracing.azurewebsites.net");
            //44353, "v1/RaceMaps/" + MapId.ToString() + "/RaceResults");
            address.Path = "v1/RaceMaps/" + MapId.ToString() + "/RaceResults";
            Uri Racemap = new Uri(address.Uri.ToString());
            //Uri FirstUri = new Uri("https://10.0.2.2:44353/v1/RaceMaps/");
            //Uri SecondUri = new Uri(FirstUri, MapId);
            //string ThirdUri = "/RaceResults";
            //Uri Racemap = new Uri(SecondUri, ThirdUri);
            content = client.GetStringAsync(Racemap).Result;
            JArray jMapResults = JArray.Parse(content);
            foreach (var item in jMapResults)
            {
                int minutesspanned = (int)item.SelectToken("raceResult.time.minutes");
                int secondsspanned = (int)item.SelectToken("raceResult.time.seconds");
                int milliseconsspanned = (int)item.SelectToken("raceResult.time.milliseconds");
                timeSpan[numerator] = new TimeSpan(0, 0, minutesspanned, secondsspanned, milliseconsspanned);
                Maps.Add(new MapsListView((int?)item.SelectToken("raceResult.raceid"), timeSpan[numerator].ToString(), Color.Red, (string)item.SelectToken("userName")));
                numerator++;
            }
            jsonlist = JsonConvert.DeserializeObject<List<RaceResultGet>>(content);
        }


        public MapRankingPage(string MapId)
        {
            DownloadMapsFromServer(MapId);

            Label header = new Label
            {
                Text = "ListView",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center
            };

            // Define some data.
            //List<MapsListView> Maps = new List<MapsListView>();


            //foreach (RaceResultGet item in jsonlist)
            //{
            //    Maps.Add(new MapsListView(item.RaceId, item.seconds, Color.Red));
            //}


            // Create the ListView.
            ListView listView = new ListView
            {
                // Source of data items.
                ItemsSource = Maps,

                // Define template for displaying each item.
                // (Argument of DataTemplate constructor is called for 
                //      each item; it must return a Cell derivative.)
                ItemTemplate = new DataTemplate(() =>
                {

                    Label NameLabel = new Label();
                    NameLabel.SetBinding(Label.TextProperty, "UserName");
                        //new Binding("IdValue", BindingMode.OneWay, null, null, "User Id: {0}"));

                    // Create views with bindings for displaying each property.
                    Label TimeLabel = new Label();
                    TimeLabel.SetBinding(Label.TextProperty, "Time");

                    BoxView boxView = new BoxView();
                    boxView.SetBinding(BoxView.ColorProperty, "FavoriteColor");

                    // Return an assembled ViewCell.
                    return new ViewCell
                    {
                        View = new StackLayout
                        {

                            Padding = new Thickness(0, 5),
                            Orientation = StackOrientation.Horizontal,
                            Children =
                            {

                                    boxView,
                                    new StackLayout
                                    {
                                        VerticalOptions = LayoutOptions.Center,
                                        Spacing = 0,
                                        Children =
                                        {
                                            NameLabel,
                                            TimeLabel,
                                        }
                                    },
                            }
                        }
                    };
                })
            };

            // Accomodate iPhone status bar.
            //this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
            listView.HasUnevenRows = true;
            //listView.ItemTapped += ListView_ItemTapped;
            // Build the page.
            this.Content = new StackLayout
            {
                Children =
                {
                    header,
                    listView
                }
            };
        }

        //private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        //{
        //    var SelectedItem = e.Item as MapsListView;
        //    string[] RaceMapArgs = new string[2];
        //    RaceMapArgs[0] = SelectedItem.IdValue.ToString();
        //    RaceMapArgs[1] = SelectedItem.Name;
        //    MessagingCenter.Send(this, "this", RaceMapArgs);
        //    Navigation.PopAsync();
        //}
    }
}