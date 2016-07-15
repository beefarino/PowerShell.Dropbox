using System.Collections.Generic;
using System.IO;
using System.Management.Automation.Provider;
using CodeOwls.PowerShell.Paths;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;
using Dropbox.Api;
using Dropbox.Api.Files;

namespace CodeOwls.PowerShell.Dropbox
{
    public class DropboxFileNode : PathNodeBase, IGetItemContent, ISetItemContent
    {
        private readonly DropboxClient _client;
        private readonly Metadata _metadata;

        public DropboxFileNode(DropboxClient client, Metadata metadata, string name, string path)
        {
            _client = client;
            _metadata = metadata;
            Name = name;
            Path = path;
        }

        public override IPathValue GetNodeValue()
        {
            return new LeafPathValue( _metadata, Name );
        }

        public override string Name { get; }

        string Path { get; }

        public IContentReader GetContentReader(IProviderContext providerContext)
        {
            var download = _client.Files.DownloadAsync(Path).Result;
            var stream = download.GetContentAsStreamAsync().Result;
            return new StreamContentReader(stream);
        }

        public object GetContentReaderDynamicParameters(IProviderContext providerContext)
        {
            return null;
        }

        public IContentWriter GetContentWriter(IProviderContext providerContext)
        {
            var writer = new StreamContentWriter( new MemoryStream() );
            writer.WriteComplete += (sender, stream) =>
            {
                stream.Position = 0L;
                var result = _client.Files.UploadAsync(Path, WriteMode.Overwrite.Instance, false, null, false, stream).Result;
            };
            return writer;

        }

        public object GetContentWriterDynamicParameters(IProviderContext providerContext)
        {
            return null;
        }
    }
}