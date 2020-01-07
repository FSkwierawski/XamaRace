using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PSMapa
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();

            NavigationPage.SetHasBackButton(this, false);
        }

        private void Logout_Clicked(object sender, EventArgs e)
        {
            Navigation.PopToRootAsync();
        }

        private void MapCreationPage_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MapCreationPage());
        }

        private void PlayingPage_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PlayingPage());
        }
    }
}