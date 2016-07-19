using System;
using System.Management.Automation;
using CodeOwls.PowerShell.Provider;
using Dropbox.Api;

namespace CodeOwls.PowerShell.Dropbox
{
    public class DropboxDrive : Drive
    {
        private readonly AuthenticationData _authenticationData;

        public DropboxClient Client { get; }

        public string UserId {  get { return _authenticationData.UserId; } }

        internal DropboxDrive(PSDriveInfo driveInfo, AuthenticationData authenticationData ) : base(driveInfo)
        {            
            _authenticationData = authenticationData;
            Client = new DropboxClient( _authenticationData.AccessToken );
        }

        public string GetSecuredAccessToken()
        {
            return _authenticationData.Encrypt();
        }
    }
}