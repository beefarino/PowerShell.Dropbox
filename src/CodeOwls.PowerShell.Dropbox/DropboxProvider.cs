using System;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Threading;
using System.Threading.Tasks;
using CodeOwls.PowerShell.Paths.Processors;
using CodeOwls.PowerShell.Provider;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using Dropbox.Api.Sharing;

namespace CodeOwls.PowerShell.Dropbox
{
    [CmdletProvider( "Dropbox", ProviderCapabilities.Filter)]
    public class DropboxProvider : Provider.Provider
    {
        protected override PSDriveInfo NewDrive(PSDriveInfo drive)
        {
            if (drive is DropboxDrive) return drive;
            AuthenticationData data = null;
            var param = this.DynamicParameters as NewDriveParameters;
            if (!String.IsNullOrWhiteSpace(param?.AccessToken))
            {
                data = AuthenticationData.FromSecuredData(param.AccessToken);
            }
            else
            {
                data = Authenticate();
            }

            var root = $"[{data.UserId}]";
            if (! String.IsNullOrEmpty(drive.Root)) 
            {
                root += $"\\{drive.Root}";
            }

            var newInfo = new PSDriveInfo(
                drive.Name,
                drive.Provider,
                root,
                drive.Description,
                drive.Credential,
                drive.DisplayRoot
            );
            
            return new DropboxDrive( newInfo, data );
        }

        public class NewDriveParameters
        {
            [Parameter]
            public string AccessToken { get; set; }
        }

        protected override object NewDriveDynamicParameters()
        {
            return new NewDriveParameters();
        }

        protected override IPathResolver PathResolver
        {
            get
            {
                var drives = this.SessionState.Drive.GetAllForProvider(this.ProviderInfo.Name);
                EngineIdleManager.RegisterForNextEngineIdle(this.SessionState);
                return new DropboxPathResolver(drives);
            }
        }

        AuthenticationData Authenticate()
        {
            var completion = new TaskCompletionSource<AuthenticationData>();

            var thread = new Thread(() =>
            {
                try
                {
                    var app = System.Windows.Application.Current;
                    if (null == app)
                    {
                        app = new System.Windows.Application();
                    }
                    var login = new AuthenticationDialog( AuthenticationData.AppKey);
                    app.Run(login);
                    if (login.Result)
                    {
                        completion.TrySetResult(new AuthenticationData(login.AccessToken, login.UserId));
                    }
                    else
                    {
                        completion.TrySetCanceled();
                    }
                }
                catch (Exception e)
                {
                    completion.TrySetException(e);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            var result = completion.Task.Result;

            return result;
        }
    }
}