using System;
using System.Collections.Generic;
using System.Linq;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;
using Dropbox.Api;
using Dropbox.Api.Files;

namespace CodeOwls.PowerShell.Dropbox
{
    public class DropboxFolderNode : PathNodeBase
    {
        private readonly DropboxClient _client;
        private readonly Metadata _metadata;

        public DropboxFolderNode(DropboxClient client, Metadata metadata, string name, string path )
        {
            _client = client;
            _metadata = metadata;
            Name = name ?? String.Empty;
            Path = path ?? String.Empty;
        }

        public override IEnumerable<IPathNode> GetNodeChildren(IProviderContext providerContext)
        {
            try
            {
                var children = DropboxFolderResultCache.GetFolderChildren(_client, Path);

                var folders =
                    children.Entries.Where(m => m.IsFolder)
                        .ToList()
                        .ConvertAll(m => new DropboxFolderNode(_client, m, m.Name, Path + "/" + m.Name));

                folders.Sort((a, b) => StringComparer.OrdinalIgnoreCase.Compare(a.Name, b.Name));

                var files =
                    children.Entries.Where(m => m.IsFile)
                        .ToList()
                        .ConvertAll(m => new DropboxFileNode(_client, m, m.Name, Path + "/" + m.Name));

                files.Sort((a, b) => StringComparer.OrdinalIgnoreCase.Compare(a.Name, b.Name));

                var pathNodes = new List<IPathNode>();
                pathNodes.AddRange(folders);
                pathNodes.AddRange(files);
                return pathNodes;
            }
            catch
            {
                return null;
            }
        }

        public override IPathValue GetNodeValue()
        {
            return new ContainerPathValue(_metadata, Name);
        }

        public override string Name { get; }

        
        string Path { get; }
    }
}