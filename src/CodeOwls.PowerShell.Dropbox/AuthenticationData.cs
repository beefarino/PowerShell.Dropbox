using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CodeOwls.PowerShell.Dropbox
{
    class AuthenticationData
    {
        static readonly byte[] _entropy = { 115,176,255,214,172,82,90,67,185,113,136,68,211,32,50,14 };

        public string AccessToken { get; }
        public string UserId { get; }

        public static string AppKey = "tyye144bh9zf59y";
        public static string AppSecret = "3y3amtu2n9v31j4";

        public AuthenticationData( string accessToken, string userId )
        {
            AccessToken = accessToken;
            UserId = userId;
        }

        internal string Encrypt()
        {
            var plainText = Encoding.UTF8.GetBytes( AccessToken + "|" + UserId );
            
            var cipher = ProtectedData.Protect(plainText, _entropy, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(cipher);
        }

        public static AuthenticationData FromSecuredData(string cipher)
        {
            var plainText = ProtectedData.Unprotect(Convert.FromBase64String(cipher), _entropy, DataProtectionScope.CurrentUser);
            var data = Encoding.UTF8.GetString(plainText);
            var items = data.Split('|');
            return new AuthenticationData( items[0], items[1]);

        }
    }
}
