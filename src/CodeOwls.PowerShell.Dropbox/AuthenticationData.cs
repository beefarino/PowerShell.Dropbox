using System.Diagnostics;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace CodeOwls.PowerShell.Dropbox
{
    class AuthenticationData
    {
        public string AccessToken { get; }
        public string UserId { get; }

        public static string AppKey = "tyye144bh9zf59y";
        public static string AppSecret = "3y3amtu2n9v31j4";

        public AuthenticationData( string accessToken, string userId )
        {
            AccessToken = accessToken;
            UserId = userId;
        }

        SecureString Encrypt()
        {
            var s = new SecureString();
            AccessToken.ToList().ForEach(c=>s.AppendChar(c));
            s.AppendChar('|');
            UserId.ToList().ForEach(c => s.AppendChar(c));
            return s;
        }
    }
}
