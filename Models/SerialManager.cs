using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace ModPane.Models;

public static class SerialManager
{
    public static ObservableCollection<string> InUsePorts { get; } = new ObservableCollection<string>();
    public static ObservableCollection<string> AvailablePorts { get; } = new ObservableCollection<string>();

    public static Dictionary<string, System.IO.Ports.SerialPort> OpenPorts { get; } = new Dictionary<string, System.IO.Ports.SerialPort>();

    public static Dictionary<string, SemaphoreSlim> PortLocks { get; } = new Dictionary<string, SemaphoreSlim>();

    public static void Init()
    {
        Task.Run(() => RefreshPortsLoop());
    }

    public static async Task RefreshPortsLoop()
    {
        while (true)
        {
            var ports = System.IO.Ports.SerialPort.GetPortNames();
            AvailablePorts.Clear();
            foreach (var port in ports)
            {
                AvailablePorts.Add(port);
            }
            foreach (var inUsePort in InUsePorts.ToList())
            {
                if (!AvailablePorts.Contains(inUsePort))
                {
                    ClosePort(inUsePort);
                }
            }

            await Task.Delay(1000); // Refresh every second
        }
    }

    public static bool OpenPort(string portName, int baudRate, bool? parity, bool stopbits)
    {
        if (!AvailablePorts.Contains(portName))
        {
            throw new System.IO.IOException($"Port {portName} is not available.");
        }

        if (OpenPorts.ContainsKey(portName))
        {
            return true; // Port is already open
        }

        var serialPort = new System.IO.Ports.SerialPort(portName, baudRate)
        {
            Parity = parity.HasValue ? (parity.Value ? System.IO.Ports.Parity.Even : System.IO.Ports.Parity.Odd) : System.IO.Ports.Parity.None,
            StopBits = stopbits ? System.IO.Ports.StopBits.One : System.IO.Ports.StopBits.Two
        };

        try
        {
            serialPort.Open();
            OpenPorts[portName] = serialPort;
            InUsePorts.Add(portName);
            return true;
        }
        catch
        {
            return false;
        }

    }

    public static bool ClosePort(string portName)
    {
        if (!OpenPorts.ContainsKey(portName))
        {
            return false; // Port is not open
        }

        try
        {
            OpenPorts[portName].Close();
            OpenPorts.Remove(portName);
            InUsePorts.Remove(portName);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static async Task<int> SendDataAsync(string portName, byte[] data, int timeout = 1000, CancellationToken cancellationToken = default)
    {
        if (!OpenPorts.ContainsKey(portName))
        {
            throw new System.IO.IOException($"Port {portName} is not open.");
        }

        var serialPort = OpenPorts[portName];
        if (serialPort.IsOpen)
        {
            if (PortLocks.TryGetValue(portName, out var lockObj))
            {
                await lockObj.WaitAsync(cancellationToken);
            }
            else
            {
                lockObj = new SemaphoreSlim(1, 1);
                PortLocks[portName] = lockObj;
                await lockObj.WaitAsync(cancellationToken);
            }
            var dataLength = await Task.Run(() =>
            {
                try
                {
                    serialPort.WriteTimeout = timeout;
                    serialPort.Write(data, 0, data.Length);
                    return data.Length; // Return number of bytes sent
                }
                catch (System.IO.IOException ex)
                {
                    throw new System.IO.IOException($"Error sending data on port {portName}: {ex.Message}", ex);
                }
            }, cancellationToken);
            lockObj.Release();
            return dataLength;
        }
        else
        {
            throw new System.IO.IOException($"Port {portName} is not open.");
        }
    }

    public static async Task<byte[]> ReceiveDataAsync(string portName, int bufferSize, int timeout = 1000, CancellationToken cancellationToken = default)
    {
        if (!OpenPorts.ContainsKey(portName))
        {
            throw new System.IO.IOException($"Port {portName} is not open.");
        }

        var serialPort = OpenPorts[portName];
        if (serialPort.IsOpen)
        {
            if (PortLocks.TryGetValue(portName, out var lockObj))
            {
                await lockObj.WaitAsync(cancellationToken);
            }
            else
            {
                lockObj = new SemaphoreSlim(1, 1);
                PortLocks[portName] = lockObj;
                await lockObj.WaitAsync(cancellationToken);
            }

            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;

            try
            {
                serialPort.ReadTimeout = timeout;
                bytesRead = await Task.Run(() => serialPort.Read(buffer, 0, bufferSize), cancellationToken);
            }
            catch (TimeoutException)
            {
                throw new TimeoutException($"Read operation timed out on port {portName} after {timeout} milliseconds.");
            }
            catch (System.IO.IOException ex)
            {
                throw new System.IO.IOException($"Error receiving data on port {portName}: {ex.Message}", ex);
            }
            finally
            {
                lockObj.Release();
            }

            return buffer.Take(bytesRead).ToArray();
        }
        else
        {
            throw new System.IO.IOException($"Port {portName} is not open.");
        }
    }
}