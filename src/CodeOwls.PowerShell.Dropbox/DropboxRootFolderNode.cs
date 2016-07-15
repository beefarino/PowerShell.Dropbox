using Dropbox.Api;

namespace CodeOwls.PowerShell.Dropbox
{
    public class DropboxRootFolderNode : DropboxFolderNode
    {
        public DropboxRootFolderNode( DropboxClient client ) : base( client, null, null, null )
        {

        }
    }
}