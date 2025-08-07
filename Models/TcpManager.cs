using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ModPane.Models;

public static class TcpManager
{
    public static ObservableCollection<string> InUseConnections { get; } = new ObservableCollection<string>();

    public static Dictionary<string, TcpClient> OpenConnections { get; } = new Dictionary<string, TcpClient>();

    public static Dictionary<string, SemaphoreSlim> ConnectionLocks { get; } = new Dictionary<string, SemaphoreSlim>();

    public static void Init()
    {
        // TCP connections don't need continuous refresh like serial ports
        // They are managed on-demand
    }

    private static string GetConnectionKey(IPAddress ipAddress, int port)
    {
        return $"{ipAddress}:{port}";
    }

    private static string GetConnectionKey(string ipAddress, int port)
    {
        return $"{ipAddress}:{port}";
    }

    public static async Task<bool> OpenConnectionAsync(IPAddress ipAddress, int port, int timeoutMs = 5000)
    {
        var connectionKey = GetConnectionKey(ipAddress, port);

        if (OpenConnections.ContainsKey(connectionKey))
        {
            return true; // Connection is already open
        }

        var tcpClient = new TcpClient();
        
        try
        {
            using var cancellationTokenSource = new CancellationTokenSource(timeoutMs);
            await tcpClient.ConnectAsync(ipAddress, port, cancellationTokenSource.Token);
            
            OpenConnections[connectionKey] = tcpClient;
            InUseConnections.Add(connectionKey);
            
            // Initialize the semaphore for this connection
            ConnectionLocks[connectionKey] = new SemaphoreSlim(1, 1);
            
            return true;
        }
        catch
        {
            tcpClient?.Close();
            return false;
        }
    }

    public static async Task<bool> OpenConnectionAsync(string ipAddress, int port, int timeoutMs = 5000)
    {
        if (!IPAddress.TryParse(ipAddress, out var parsedIp))
        {
            throw new ArgumentException($"Invalid IP address: {ipAddress}");
        }

        return await OpenConnectionAsync(parsedIp, port, timeoutMs);
    }

    public static bool CloseConnection(IPAddress ipAddress, int port)
    {
        var connectionKey = GetConnectionKey(ipAddress, port);
        return CloseConnection(connectionKey);
    }

    public static bool CloseConnection(string ipAddress, int port)
    {
        var connectionKey = GetConnectionKey(ipAddress, port);
        return CloseConnection(connectionKey);
    }

    public static bool CloseConnection(string connectionKey)
    {
        if (!OpenConnections.ContainsKey(connectionKey))
        {
            return false; // Connection is not open
        }

        try
        {
            OpenConnections[connectionKey].Close();
            OpenConnections.Remove(connectionKey);
            InUseConnections.Remove(connectionKey);
            
            // Clean up the semaphore
            if (ConnectionLocks.TryGetValue(connectionKey, out var semaphore))
            {
                semaphore.Dispose();
                ConnectionLocks.Remove(connectionKey);
            }
            
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static async Task<int> SendDataAsync(IPAddress ipAddress, int port, byte[] data, int timeout = 1000, CancellationToken cancellationToken = default)
    {
        var connectionKey = GetConnectionKey(ipAddress, port);
        return await SendDataAsync(connectionKey, data, timeout, cancellationToken);
    }

    public static async Task<int> SendDataAsync(string ipAddress, int port, byte[] data, int timeout = 1000, CancellationToken cancellationToken = default)
    {
        var connectionKey = GetConnectionKey(ipAddress, port);
        return await SendDataAsync(connectionKey, data, timeout, cancellationToken);
    }

    public static async Task<int> SendDataAsync(string connectionKey, byte[] data, int timeout = 1000, CancellationToken cancellationToken = default)
    {
        if (!OpenConnections.ContainsKey(connectionKey))
        {
            throw new InvalidOperationException($"Connection {connectionKey} is not open.");
        }

        var tcpClient = OpenConnections[connectionKey];
        if (!tcpClient.Connected)
        {
            throw new InvalidOperationException($"Connection {connectionKey} is not connected.");
        }

        if (!ConnectionLocks.TryGetValue(connectionKey, out var lockObj))
        {
            lockObj = new SemaphoreSlim(1, 1);
            ConnectionLocks[connectionKey] = lockObj;
        }

        await lockObj.WaitAsync(cancellationToken);

        try
        {
            var stream = tcpClient.GetStream();
            using var timeoutCts = new CancellationTokenSource(timeout);
            using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            await stream.WriteAsync(data, 0, data.Length, combinedCts.Token);
            return data.Length; // Return number of bytes sent
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException($"Send operation timed out on connection {connectionKey} after {timeout} milliseconds.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error sending data on connection {connectionKey}: {ex.Message}", ex);
        }
        finally
        {
            lockObj.Release();
        }
    }

    public static async Task<byte[]> ReceiveDataAsync(IPAddress ipAddress, int port, int bufferSize, int timeout = 1000, CancellationToken cancellationToken = default)
    {
        var connectionKey = GetConnectionKey(ipAddress, port);
        return await ReceiveDataAsync(connectionKey, bufferSize, timeout, cancellationToken);
    }

    public static async Task<byte[]> ReceiveDataAsync(string ipAddress, int port, int bufferSize, int timeout = 1000, CancellationToken cancellationToken = default)
    {
        var connectionKey = GetConnectionKey(ipAddress, port);
        return await ReceiveDataAsync(connectionKey, bufferSize, timeout, cancellationToken);
    }

    public static async Task<byte[]> ReceiveDataAsync(string connectionKey, int bufferSize, int timeout = 1000, CancellationToken cancellationToken = default)
    {
        if (!OpenConnections.ContainsKey(connectionKey))
        {
            throw new InvalidOperationException($"Connection {connectionKey} is not open.");
        }

        var tcpClient = OpenConnections[connectionKey];
        if (!tcpClient.Connected)
        {
            throw new InvalidOperationException($"Connection {connectionKey} is not connected.");
        }

        if (!ConnectionLocks.TryGetValue(connectionKey, out var lockObj))
        {
            lockObj = new SemaphoreSlim(1, 1);
            ConnectionLocks[connectionKey] = lockObj;
        }

        await lockObj.WaitAsync(cancellationToken);

        try
        {
            var stream = tcpClient.GetStream();
            var buffer = new byte[bufferSize];
            
            using var timeoutCts = new CancellationTokenSource(timeout);
            using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            int bytesRead = await stream.ReadAsync(buffer, 0, bufferSize, combinedCts.Token);
            return buffer.Take(bytesRead).ToArray();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException($"Read operation timed out on connection {connectionKey} after {timeout} milliseconds.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error receiving data on connection {connectionKey}: {ex.Message}", ex);
        }
        finally
        {
            lockObj.Release();
        }
    }

    public static bool IsConnectionOpen(IPAddress ipAddress, int port)
    {
        var connectionKey = GetConnectionKey(ipAddress, port);
        return IsConnectionOpen(connectionKey);
    }

    public static bool IsConnectionOpen(string ipAddress, int port)
    {
        var connectionKey = GetConnectionKey(ipAddress, port);
        return IsConnectionOpen(connectionKey);
    }

    public static bool IsConnectionOpen(string connectionKey)
    {
        return OpenConnections.ContainsKey(connectionKey) && OpenConnections[connectionKey].Connected;
    }

    public static void CloseAllConnections()
    {
        foreach (var connectionKey in OpenConnections.Keys.ToList())
        {
            CloseConnection(connectionKey);
        }
    }
}