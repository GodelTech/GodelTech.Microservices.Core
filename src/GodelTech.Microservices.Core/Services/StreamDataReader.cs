using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.Exceptions;

namespace GodelTech.Microservices.Core.Services
{
    public class StreamDataReader : IStreamDataReader
    {
        private const int BufferSize = 10 * 1024;

        public async Task<byte[]> ReadFromStreamAsync(Stream stream, int maxFileSize)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            using (var tokenSource = new CancellationTokenSource())
            using (var memoryStream = new MemoryStream())
            {
                var buffer = new byte[BufferSize];
                var totalBytes = 0;

                while (true)
                {
                    var read = await stream.ReadAsync(buffer, 0, buffer.Length, tokenSource.Token);

                    if (read <= 0)
                        break;

                    totalBytes = totalBytes + read;

                    if (totalBytes > maxFileSize)
                    {
                        tokenSource.Cancel();

                        throw new FileTooLargeExceptionException($"File size must be less than {maxFileSize} MB");
                    }

                    await memoryStream.WriteAsync(buffer, 0, read, tokenSource.Token);
                }

                memoryStream.Flush();

                return memoryStream.ToArray();
            }
        }
    }
}