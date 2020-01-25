using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PSMapa.BindingModel;
using System.Net.Http;
using Newtonsoft.Json;
namespace PSMapa
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page2 : ContentPage
    {
        public RegisterUser Registration = new RegisterUser();

        public Page2()
        {
            InitializeComponent();
            HttpClientHandler clientHandler = new HttpClientHandler();
        }

        public async void SignIn_Clicked(object s, EventArgs e)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (message, cert, chain, errors) => { return true; };
            HttpClient client = new HttpClient(clientHandler);

            Registration.UserName = SingInUserLogin.Text;
            Registration.Password = SinginUserPassword.Text;
            Registration.ConfirmPassword = SinginUserConfirmPassword.Text;
            if (SingInUserLogin.Text.Length == 0 || SinginUserPassword.Text.Length == 0 || SinginUserConfirmPassword.Text.Length == 0)
            {
                await DisplayAlert("Error", "All fields must be filled", "OK");
            }
            else
            {
                try
                {
                    Uri uriaddress = new Uri("https://xamaracing.azurewebsites.net/v1/Accounts/Register");
                    var content = JsonConvert.SerializeObject(Registration);
                    var data = new StringContent(content, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(uriaddress, data);
                    string result = response.Content.ReadAsStringAsync().Result;
                }
                catch
                {
                    await DisplayAlert("Error", "Connection not established", "OK");
                }
            }
        }
    }
}