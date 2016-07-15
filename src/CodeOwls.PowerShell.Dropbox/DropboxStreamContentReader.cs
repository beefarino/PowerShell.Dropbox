using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation.Provider;

namespace CodeOwls.PowerShell.Dropbox
{
    class StreamContentWriter : IContentWriter
    {
        private readonly Stream _stream;

        public StreamContentWriter( Stream stream )
        {
            _stream = stream;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        public IList Write(IList content)
        {
            var buffer = new List<byte>();
            foreach (byte b in content)
            {
                buffer.Add(b);   
            }

            _stream.Write(buffer.ToArray(), 0, buffer.Count);
            return buffer;
        }

        public void Seek(long offset, SeekOrigin origin)
        {
            _stream.Seek(offset, origin);
        }

        public void Close()
        {
            _stream.Flush();
            _stream.Close();
        }

        public event EventHandler<Stream> WriteComplete;

        protected virtual void OnWriteComplete(Stream e)
        {
            WriteComplete?.Invoke(this, e);
        }
    }
    internal class StreamContentReader : IContentReader
    {
        private readonly Stream _stream;

        public StreamContentReader(Stream stream)
        {
            _stream = stream;
        }

        public void Dispose()
        {
            _stream.Dispose();
        }

        public IList Read(long readCount)
        {
            var actualReadCount = readCount > 0 ? readCount : _stream.Length;

            ArrayList list = new ArrayList();

            var buffer = new byte[actualReadCount];
            var thisCount = _stream.Read(buffer, 0, (int) actualReadCount);
            if (thisCount != actualReadCount)
            {
                var thisBuffer = new byte[thisCount];
                Array.Copy( buffer, thisBuffer, thisCount );
                buffer = thisBuffer;
            }
            list.AddRange( buffer );
            
            return list;
        }

        public void Close()
        {
            _stream.Close();
        }

        public void Seek(long offset, SeekOrigin origin)
        {
            _stream.Seek(offset, origin);
        }
    }
}