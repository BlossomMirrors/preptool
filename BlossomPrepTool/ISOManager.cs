using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace BlossomPrepTool
{
    public class ISODownloadProgressEventArgs : EventArgs
    {
        public long BytesDownloaded { get; set; }
        public long TotalBytes { get; set; }
        public int ProgressPercentage => TotalBytes > 0 ? (int)((BytesDownloaded * 100) / TotalBytes) : 0;
    }

    public class ISOMetadata
    {
        public string name { get; set; }
        public string sha256 { get; set; }
    }

    /// <summary>
    /// Manages ISO download, caching, and SHA256 verification
    /// </summary>
    public class ISOManager
    {
        private const string MetadataUrl = "https://cdn.blossomos.org/iso/isodata.json";
        private const string BaseUrl = "https://cdn.blossomos.org/iso/";
        private const string CacheDirName = "BlossomOS";

        private string _cacheDirectory;
        private CancellationTokenSource _cancellationTokenSource;
        private ManualResetEventSlim _pauseEvent = new ManualResetEventSlim(true);
        private volatile bool _isPaused;
        private ISOMetadata _metadata;

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
                // Fetch metadata first
                await FetchMetadata();
                if (_metadata == null || string.IsNullOrEmpty(_metadata.sha256))
                    return false;

                var actualSha256 = await ComputeFileSHA256(isoPath);
                return actualSha256.Equals(_metadata.sha256, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                // If verification fails, delete the file
                try { File.Delete(isoPath); } catch { }
                return false;
            }
        }

        private async Task FetchMetadata()
        {
            if (_metadata != null)
                return;

            try
            {
                using (var client = new HttpClient())
                {
                    var json = await client.GetStringAsync(MetadataUrl);
                    
                    // Simple JSON parsing for the specific structure
                    var nameMatch = Regex.Match(json, @"""name""\s*:\s*""([^""]+)""");
                    var sha256Match = Regex.Match(json, @"""sha256""\s*:\s*""([^""]+)""");
                    
                    if (nameMatch.Success && sha256Match.Success)
                    {
                        _metadata = new ISOMetadata
                        {
                            name = nameMatch.Groups[1].Value,
                            sha256 = sha256Match.Groups[1].Value
                        };
                    }
                }
            }
            catch
            {
                _metadata = null;
                throw;
            }
        }

        public async Task<string> DownloadISO()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var isoPath = GetISOPath();

            try
            {
                // Fetch metadata first
                await FetchMetadata();
                if (_metadata == null || string.IsNullOrEmpty(_metadata.name))
                    throw new Exception("Failed to fetch ISO metadata");

                var isoUrl = BaseUrl + _metadata.name;

                // Ensure cache directory exists
                Directory.CreateDirectory(_cacheDirectory);

                // Delete any existing incomplete file
                if (File.Exists(isoPath))
                    File.Delete(isoPath);

                using (var httpClient = new HttpClient())
                using (var response = await httpClient.GetAsync(isoUrl, HttpCompletionOption.ResponseHeadersRead, _cancellationTokenSource.Token))
                {
                    response.EnsureSuccessStatusCode();
                    var totalBytes = response.Content.Headers.ContentLength ?? 0;

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(isoPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, true))
                    {
                        var buffer = new byte[81920];
                        long totalRead = 0;
                        int bytesRead;

                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, _cancellationTokenSource.Token)) > 0)
                        {
                            _pauseEvent.Wait(_cancellationTokenSource.Token);

                            await fileStream.WriteAsync(buffer, 0, bytesRead, _cancellationTokenSource.Token);
                            totalRead += bytesRead;

                            DownloadProgress?.Invoke(this, new ISODownloadProgressEventArgs
                            {
                                BytesDownloaded = totalRead,
                                TotalBytes = totalBytes
                            });
                        }
                    }
                }

                if (_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    DownloadCancelled?.Invoke(this, EventArgs.Empty);
                    throw new OperationCanceledException();
                }

                DownloadCompleted?.Invoke(this, EventArgs.Empty);

                // Verify after download
                var verified = await CheckCachedISO();
                if (!verified)
                    throw new Exception("ISO verification failed after download");

                return isoPath;
            }
            catch (OperationCanceledException)
            {
                try { File.Delete(isoPath); } catch { }
                DownloadCancelled?.Invoke(this, EventArgs.Empty);
                throw;
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
            _pauseEvent.Set();
            _isPaused = false;
        }

        public void PauseDownload()
        {
            if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
                return;

            _isPaused = true;
            _pauseEvent.Reset();
        }

        public void ResumeDownload()
        {
            _isPaused = false;
            _pauseEvent.Set();
        }

        public bool IsPaused => _isPaused;

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
