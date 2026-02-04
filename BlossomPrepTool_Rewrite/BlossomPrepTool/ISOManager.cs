using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace BlossomPrepTool
{
    public class ISODownloadProgressEventArgs : EventArgs
    {
        public long BytesDownloaded { get; set; }
        public long TotalBytes { get; set; }
        public int ProgressPercentage => TotalBytes > 0 ? (int)((BytesDownloaded * 100) / TotalBytes) : 0;
    }

    /// <summary>
    /// Manages ISO download, caching, and SHA256 verification
    /// </summary>
    public class ISOManager
    {
        private const string ISOUrl = "https://cdn.blossomos.org/iso/BlossomOS-2026.01.16-x86_64.iso";
        private const string SHA256Url = ISOUrl + ".sha256";
        private const string CacheDirName = "BlossomOS";

        private string _cacheDirectory;
        private CancellationTokenSource _cancellationTokenSource;

        public event EventHandler<ISODownloadProgressEventArgs> DownloadProgress;
        public event EventHandler<EventArgs> DownloadCompleted;
        public event EventHandler<EventArgs> DownloadCancelled;

        public ISOManager()
        {
            _cacheDirectory = Path.Combine(Path.GetTempPath(), CacheDirName);
        }

        public string GetCachePath()
        {
            return _cacheDirectory;
        }

        public string GetISOPath()
        {
            return Path.Combine(_cacheDirectory, "BlossomOS.iso");
        }

        public async Task<bool> CheckCachedISO()
        {
            var isoPath = GetISOPath();

            if (!File.Exists(isoPath))
                return false;

            try
            {
                // Verify SHA256
                var expectedSha256 = await GetExpectedSHA256();
                if (string.IsNullOrEmpty(expectedSha256))
                    return false;

                var actualSha256 = await ComputeFileSHA256(isoPath);
                return actualSha256.Equals(expectedSha256, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                // If verification fails, delete the file
                try { File.Delete(isoPath); } catch { }
                return false;
            }
        }

        public async Task<string> DownloadISO()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var isoPath = GetISOPath();

            try
            {
                // Ensure cache directory exists
                Directory.CreateDirectory(_cacheDirectory);

                // Delete any existing incomplete file
                if (File.Exists(isoPath))
                    File.Delete(isoPath);

                using (var client = new WebClient())
                {
                    client.DownloadProgressChanged += (s, e) =>
                    {
                        if (_cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            client.CancelAsync();
                            return;
                        }

                        DownloadProgress?.Invoke(this, new ISODownloadProgressEventArgs
                        {
                            BytesDownloaded = e.BytesReceived,
                            TotalBytes = e.TotalBytesToReceive
                        });
                    };

                    client.DownloadFileCompleted += (s, e) =>
                    {
                        if (e.Cancelled)
                        {
                            DownloadCancelled?.Invoke(this, EventArgs.Empty);
                        }
                        else if (e.Error != null)
                        {
                            // Delete incomplete file
                            try { File.Delete(isoPath); } catch { }
                        }
                        else
                        {
                            DownloadCompleted?.Invoke(this, EventArgs.Empty);
                        }
                    };

                    await client.DownloadFileTaskAsync(ISOUrl, isoPath);
                }

                // Verify after download
                var verified = await CheckCachedISO();
                if (!verified)
                    throw new Exception("ISO verification failed after download");

                return isoPath;
            }
            catch
            {
                // Clean up on error
                try { File.Delete(isoPath); } catch { }
                throw;
            }
        }

        public void CancelDownload()
        {
            _cancellationTokenSource?.Cancel();
        }

        private async Task<string> GetExpectedSHA256()
        {
            try
            {
                using (var client = new WebClient())
                {
                    var content = await Task.Run(() => client.DownloadString(SHA256Url));
                    var parts = content.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    return parts.Length > 0 ? parts[0] : null;
                }
            }
            catch
            {
                return null;
            }
        }

        private async Task<string> ComputeFileSHA256(string filePath)
        {
            return await Task.Run(() =>
            {
                using (var sha256 = SHA256.Create())
                using (var fileStream = File.OpenRead(filePath))
                {
                    var hash = sha256.ComputeHash(fileStream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            });
        }
    }
}
