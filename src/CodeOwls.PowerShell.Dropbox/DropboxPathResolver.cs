using CodeOwls.PowerShell.Paths.Processors;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.PowerShell.Dropbox
{
    public class DropboxPathResolver : PathResolverBase
    {
        private readonly DropboxDrive _drive;

        public DropboxPathResolver(DropboxDrive drive)
        {
            _drive = drive;
        }

        protected override IPathNode Root { get { return new DropboxRootFolderNode( _drive.Client );  } }
    }
}