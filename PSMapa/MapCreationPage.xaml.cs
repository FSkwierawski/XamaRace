using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Input;
using WorkingWithMaps.ViewModels;




namespace PSMapa
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapCreationPage : ContentPage
    {

        public PinItemsSourcePageViewModel context;
        public Geocoder geoCoder;

        public MapCreationPage()
        {
            InitializeComponent();
            context = new PinItemsSourcePageViewModel();
            geoCoder = new Geocoder();
            BindingContext = context;
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(54.3520500, 18.6463700), Distance.FromMiles(5)));          
        }

        public async void OnMapClicked(object sender, MapClickedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"MapClick: {e.Position.Latitude}, {e.Position.Longitude}");
            //DisplayAlert("New pin coordinates", $"MapClick: {e.Position.Latitude}, {e.Position.Longitude}", "OK");
            await DisplayAlert("New checkpoint", $"{e.Position.Latitude}, {e.Position.Longitude}", "OK");
            string address = "";
            var PossibleAddress = await geoCoder.GetAddressesForPositionAsync(e.Position);
            foreach (var a in PossibleAddress)
                {
                address += a;
                }
            context._pinCreatedCount++;
            context._locations.Add(new Location(
                $"Pin {context._pinCreatedCount}",
                $"{address}",
                new Position(e.Position.Latitude, e.Position.Longitude)));

        }

        private void UploadMapBtn_Clicked(object sender, EventArgs e)
        {
            if (context._locations.Count != 0)
            {

                if (CreatedMapName.Text.Length != 0)
                {
                    try
                    {
                        context.MapName = CreatedMapName.Text;
                        context.UploadMapAsync();
                        CreatedMapName.Text = "";
                        context._locations.Clear();
                        DisplayAlert("Upload Succesfull", "", "OK");
                    }
                    catch
                    {
                        DisplayAlert("Upload map", "Error", "OK");
                    }
                }
                else
                {
                    DisplayAlert("Map name is empty", "Insert map name", "OK");
                }
            }
            else
            {
                DisplayAlert("Checkpoint list is empty", "Add some checkpoints", "OK");

            }
        }
    }
}