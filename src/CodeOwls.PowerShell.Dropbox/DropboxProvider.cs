using System;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Threading;
using System.Threading.Tasks;
using CodeOwls.PowerShell.Paths.Processors;
using CodeOwls.PowerShell.Provider;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;

namespace CodeOwls.PowerShell.Dropbox
{
    [CmdletProvider( "Dropbox", ProviderCapabilities.Filter)]
    public class DropboxProvider : Provider.Provider
    {
        DropboxDrive Drive {  get { return this.PSDriveInfo as DropboxDrive; } }

        protected override PSDriveInfo NewDrive(PSDriveInfo drive)
        {
            if (drive is DropboxDrive) return drive;

            var data = Authenticate();

            return new DropboxDrive( drive, data );
        }

        protected override IPathResolver PathResolver
        {
            get
            {
                EngineIdleManager.RegisterForNextEngineIdle(this.SessionState);
                return new DropboxPathResolver(Drive);
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