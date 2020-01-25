using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;
using System.Net.Http;
using Newtonsoft.Json;
using PSMapa.BindingModel;
using Newtonsoft.Json.Linq;

namespace PSMapa
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page1 : ContentPage
    {
        public Page1()
        {
            InitializeComponent();
        }
        private async void Login_Clicked(object s, EventArgs e)
        {
            LoginUser user = new LoginUser();
            user.UserName = EntryUserLogin.Text;
            user.Password = EntryUserPassword.Text;

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (message, cert, chain, errors) => { return true; };
            HttpClient client = new HttpClient(clientHandler);

            Uri uriaddress = new Uri("https://xamaracing.azurewebsites.net/v1/Accounts/Authenticate");
            var content = JsonConvert.SerializeObject(user);
            var data = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(uriaddress, data);
            string result = response.Content.ReadAsStringAsync().Result;
            JObject jToken = JObject.Parse(result);
            string accesstoken = (string)jToken.SelectToken("accessToken");
            Tokens.UserToken = accesstoken;
            if (response.IsSuccessStatusCode)
            {
               Navigation.PushAsync(new MenuPage());
            }
            //if (EntryUserLogin.Text == "filip" && EntryUserPassword.Text == "filip1")
            //{
            //Navigation.PushAsync(new MenuPage());
            //}
        }

        private void SignIn_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Page2());

        }


    }
}