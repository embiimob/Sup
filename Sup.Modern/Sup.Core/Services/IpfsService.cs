using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace Sup.Core.Services;

/// <summary>
/// IPFS service implementation for file storage and retrieval
/// </summary>
public class IpfsService : IIpfsService
{
    private readonly string _ipfsApiUrl;
    private readonly string _ipfsExecutablePath;
    private Process? _daemonProcess;
    private readonly HttpClient _httpClient;

    public IpfsService(string apiUrl = "http://127.0.0.1:5001", string? executablePath = null)
    {
        _ipfsApiUrl = apiUrl;
        _ipfsExecutablePath = executablePath ?? GetDefaultIpfsPath();
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_ipfsApiUrl)
        };
    }

    public async Task<bool> IsRunningAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/v0/version");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task StartDaemonAsync()
    {
        if (await IsRunningAsync())
        {
            Console.WriteLine("IPFS daemon is already running");
            return;
        }

        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = _ipfsExecutablePath,
                Arguments = "daemon",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            _daemonProcess = Process.Start(startInfo);

            if (_daemonProcess == null)
                throw new InvalidOperationException("Failed to start IPFS daemon");

            // Wait for daemon to be ready
            for (int i = 0; i < 30; i++)
            {
                await Task.Delay(1000);
                if (await IsRunningAsync())
                {
                    Console.WriteLine("IPFS daemon started successfully");
                    return;
                }
            }

            throw new TimeoutException("IPFS daemon did not start within 30 seconds");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to start IPFS daemon: {ex.Message}", ex);
        }
    }

    public async Task StopDaemonAsync()
    {
        try
        {
            if (_daemonProcess != null && !_daemonProcess.HasExited)
            {
                _daemonProcess.Kill();
                await _daemonProcess.WaitForExitAsync();
                _daemonProcess.Dispose();
                _daemonProcess = null;
                Console.WriteLine("IPFS daemon stopped");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error stopping IPFS daemon: {ex.Message}");
        }
    }

    public async Task<string> AddFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        if (!await IsRunningAsync())
            throw new InvalidOperationException("IPFS daemon is not running");

        try
        {
            using var fileStream = File.OpenRead(filePath);
            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(fileStream);
            
            content.Add(streamContent, "file", Path.GetFileName(filePath));

            var response = await _httpClient.PostAsync("/api/v0/add", content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<IpfsAddResponse>(responseBody);

            if (result?.Hash == null)
                throw new InvalidOperationException("IPFS add returned no hash");

            return result.Hash;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to add file to IPFS: {ex.Message}", ex);
        }
    }

    public async Task<string> AddDataAsync(byte[] data)
    {
        if (!await IsRunningAsync())
            throw new InvalidOperationException("IPFS daemon is not running");

        try
        {
            using var content = new MultipartFormDataContent();
            using var byteContent = new ByteArrayContent(data);
            
            content.Add(byteContent, "file", "data");

            var response = await _httpClient.PostAsync("/api/v0/add", content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<IpfsAddResponse>(responseBody);

            if (result?.Hash == null)
                throw new InvalidOperationException("IPFS add returned no hash");

            return result.Hash;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to add data to IPFS: {ex.Message}", ex);
        }
    }

    public async Task<byte[]> GetDataAsync(string cid)
    {
        if (!await IsRunningAsync())
            throw new InvalidOperationException("IPFS daemon is not running");

        try
        {
            var response = await _httpClient.PostAsync($"/api/v0/cat?arg={cid}", null);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get data from IPFS: {ex.Message}", ex);
        }
    }

    public async Task PinAsync(string cid)
    {
        if (!await IsRunningAsync())
            throw new InvalidOperationException("IPFS daemon is not running");

        try
        {
            var response = await _httpClient.PostAsync($"/api/v0/pin/add?arg={cid}", null);
            response.EnsureSuccessStatusCode();
            Console.WriteLine($"Pinned: {cid}");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to pin: {ex.Message}", ex);
        }
    }

    public async Task UnpinAsync(string cid)
    {
        if (!await IsRunningAsync())
            throw new InvalidOperationException("IPFS daemon is not running");

        try
        {
            var response = await _httpClient.PostAsync($"/api/v0/pin/rm?arg={cid}", null);
            response.EnsureSuccessStatusCode();
            Console.WriteLine($"Unpinned: {cid}");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to unpin: {ex.Message}", ex);
        }
    }

    private string GetDefaultIpfsPath()
    {
        // Determine platform-specific default path
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Path.Combine(AppContext.BaseDirectory, "ipfs", "ipfs.exe");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return Path.Combine(AppContext.BaseDirectory, "ipfs", "ipfs");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return Path.Combine(AppContext.BaseDirectory, "ipfs", "ipfs");
        }
        else
        {
            return "ipfs"; // Hope it's in PATH
        }
    }

    public void Dispose()
    {
        StopDaemonAsync().Wait();
        _httpClient?.Dispose();
    }

    #region Response Models

    private class IpfsAddResponse
    {
        public string? Hash { get; set; }
        public string? Name { get; set; }
        public long Size { get; set; }
    }

    #endregion
}
