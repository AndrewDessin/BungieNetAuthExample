using System;
using System.ComponentModel;
using System.Diagnostics;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using AuthExample.DAL;
using AuthExample.Domain;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AuthExample
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int Success = 1;
        public MainPage()
        {
            InitializeComponent();
            Status.Text = "Not Logged In";
            BungieClient.Instance.PropertyChanged += BungieClient_PropertyChanged;
        }

        private async void BungieClient_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            if (e.PropertyName == nameof(BungieClient.AuthCode))
            {
                var result = await BungieClient.Instance.ObtainAccessToken();
                Status.Text = result?.ErrorCode == Success ? "Logged In" : "Not Logged In";
            }
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(BungieClient.AuthenticationCodeRequest));
        }

        private async void Print_Click(object sender, RoutedEventArgs e)
        {
            var result = await BungieClient.Instance.RunGetAsync<GamertagResponse>("Platform/User/GetBungieNetUser/");
            if (result?.ErrorCode == Success)
            {
                PsnText.Text = result.Response?.PsnId ?? "None";
                XboxText.Text = result.Response?.GamerTag ?? "None";
            }
            else
                PsnText.Text = "error";
        }
    }
}