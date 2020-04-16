using System;
using System.IO;

namespace GodelTech.Microservices.Core.DataLayer.Utils
{
    public class DbStreamAdapter : Stream
    {
        private bool _alreadyDisposed;

        private readonly Stream _innerStream;
        private readonly IDisposable _resourceOwner;

        public override bool CanRead => _innerStream.CanRead;
        public override bool CanSeek => _innerStream.CanSeek;
        public override bool CanWrite => _innerStream.CanWrite;
        public override long Length => _innerStream.Length;
        public override long Position
        {
            get => _innerStream.Position;
            set => _innerStream.Position = value;
        }

        public DbStreamAdapter(Stream innerStream, IDisposable resourceOwner)
        {
            _innerStream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
            _resourceOwner = resourceOwner ?? throw new ArgumentNullException(nameof(resourceOwner));
        }

        public override void Flush()
        {
            _innerStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _innerStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _innerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _innerStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _innerStream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_alreadyDisposed)
                return;

            _resourceOwner.Dispose();
            _alreadyDisposed = true;
        }
    }
}