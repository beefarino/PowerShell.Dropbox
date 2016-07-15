using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dropbox.Api;

namespace CodeOwls.PowerShell.Dropbox
{
    /// <summary>
    /// Interaction logic for AuthenticationDialog.xaml
    /// </summary>
    public partial class AuthenticationDialog
    {
        const string RedirectUri = "https://localhost/authorize";
        private string _state;

        public AuthenticationDialog( string appKey )
        {
            InitializeComponent();
            Dispatcher.BeginInvoke(new Action<string>(this.Start), appKey);
        }

        public string AccessToken { get; private set; }
        public string UserId { get; private set; }
        public bool Result { get; private set; }

        void Start(string appKey)
        {
            _state = Guid.NewGuid().ToString("N");
            var authUrl = DropboxOAuth2Helper.GetAuthorizeUri(
                OAuthResponseType.Token, 
                appKey, 
                new Uri(RedirectUri), 
                _state
            );

            Browser.Navigate(authUrl);
        }

        void BrowserNavigating(object sender, NavigatingCancelEventArgs args)
        {
            if (!args.Uri.ToString().StartsWith(RedirectUri, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            try
            {
                OAuth2Response response = DropboxOAuth2Helper.ParseTokenFragment(args.Uri);
                if (response.State != _state)
                {
                    return;
                }

                this.AccessToken = response.AccessToken;
                this.UserId = response.Uid;
                this.Result = true;
            }
            catch (ArgumentException)
            {
            }
            finally
            {
                args.Cancel = true;
                this.Close();
            }
        }

        void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
