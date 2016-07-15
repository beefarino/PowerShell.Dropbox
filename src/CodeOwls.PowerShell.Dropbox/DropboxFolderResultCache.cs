using System.Collections.Generic;
using Dropbox.Api;
using Dropbox.Api.Files;

namespace CodeOwls.PowerShell.Dropbox
{
    static class DropboxFolderResultCache
    {
        static Dictionary<string, ListFolderResult> Cache = new Dictionary<string, ListFolderResult>();

        static DropboxFolderResultCache()
        {
            EngineIdleManager.OnEngineIdle += (sender, args) => Clear();
        }

        public static ListFolderResult GetFolderChildren(DropboxClient client, string path)
        {
            var lowerPath = path.ToLowerInvariant();
            if (Cache.ContainsKey(lowerPath))
            {
                return Cache[path];
            }

            var result = client.Files.ListFolderAsync(path).Result;
            Cache[lowerPath] = result;
            return result;
        }

        static void Clear()
        {
            Cache = new Dictionary<string, ListFolderResult>();
        }
    }
}