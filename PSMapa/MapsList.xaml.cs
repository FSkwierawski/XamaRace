using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PSMapa.BindingModel;
using PSMapa.ViewModels;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace PSMapa
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    partial class MapsList : ContentPage
    {
        public List<RaceMapBindingModelWithID> jsonlist = new List<RaceMapBindingModelWithID>();
        public string content;
        public class MapsListView
        {
            public MapsListView(string name, string description, Color favoriteColor, int _Id)
            {
                this.Name = name;
                this.Description = description;
                this.FavoriteColor = favoriteColor;
                this.IdValue = _Id;
            }
            public int IdValue { get; set; }

            public string Name { private set; get; }

            public string Description { private set; get; }

            public Color FavoriteColor { private set; get; }
        };

        public void DownloadMapsFromServer()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient(clientHandler);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Tokens.UserToken);

            Uri Racemap = new Uri("https://xamaracing.azurewebsites.net/v1/RaceMaps/");
            content = client.GetStringAsync(Racemap).Result;
            jsonlist = JsonConvert.DeserializeObject<List<RaceMapBindingModelWithID>>(content);
        }


        public MapsList()
        {
            DownloadMapsFromServer();

            Label header = new Label
            {
                Text = "ListView",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center
            };

            // Define some data.
            List<MapsListView> Maps = new List<MapsListView>();


                foreach (RaceMapBindingModelWithID item in jsonlist)
                {
                Maps.Add(new MapsListView($"{item.Name}", $"{item.Description}", Color.Red, item.Id));
                }

            
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
                    // Create views with bindings for displaying each property.
                    Label nameLabel = new Label();
                    nameLabel.SetBinding(Label.TextProperty, "Name");

                    Label descriptionlabel = new Label();
                    descriptionlabel.SetBinding(Label.TextProperty, "Description");

                    Label IDlabel = new Label();
                    IDlabel.SetBinding(Label.TextProperty,
                        new Binding("IdValue", BindingMode.OneWay, null, null, "Id number: {0}"));

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
                                            nameLabel,
                                            descriptionlabel,
                                            IDlabel
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
            listView.ItemTapped += ListView_ItemTapped;
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

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var SelectedItem = e.Item as MapsListView;
            string[] RaceMapArgs = new string[2];
            RaceMapArgs[0] = SelectedItem.IdValue.ToString();
            RaceMapArgs[1] = SelectedItem.Name;
            MessagingCenter.Send(this, "this", RaceMapArgs);
            Navigation.PopAsync();
        }
    }
}