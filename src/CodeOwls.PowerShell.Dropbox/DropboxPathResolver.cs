using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using CodeOwls.PowerShell.Paths.Processors;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.PowerShell.Dropbox
{
    public class DropboxPathResolver : PSProviderPathResolver<DropboxDrive>
    {
        public DropboxPathResolver(IEnumerable<PSDriveInfo> drives) : base(drives)
        {
        }

        protected override IPathNode Root { get { return new DropboxRootFolderNode( ActiveDrive.Client );  } }
    }
}