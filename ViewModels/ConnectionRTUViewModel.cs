using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModPan.ViewModels;
using ModPane.Models;

namespace ModPane.ViewModels;

public partial class ConnectionRTUViewModel : ConnectionViewModel
{
    [ObservableProperty]
    private string _serialPort = string.Empty;

    [ObservableProperty]
    private int _baudRate;

    [ObservableProperty]
    private bool? _parity = null; // Null == None, true == Even, false == Odd

    [ObservableProperty]
    private bool _stopBits = true; // True == 1 Stop Bit, False == 2 Stop Bits

    [ObservableProperty]
    private int _slaveAddress = 1; // Default slave address for RTU connections

    [ObservableProperty]
    private int _timeout = 1000; // Default to 1000ms

    public ObservableCollection<string> AvailableSerialPorts => SerialManager.AvailablePorts;

    public ConnectionRTUViewModel(ConnectionsPageViewModel parent, string name, string serialPort, int baudRate)
        : base(parent, name, "RTU")
    {
        if (!SerialPortValidation(serialPort) || !BaudRateValidation(baudRate))
        {
            throw new ArgumentException("Invalid serial port or baud rate format.", nameof(serialPort));
        }
        SerialPort = serialPort;
        BaudRate = baudRate;
        Parity = null; // Default to None
        StopBits = true; // Default to 1 Stop Bit

        // This opens the port on device creation, if multiple devices use the same port, they must share the same settings
        // TODO Tell the user somehow that the settings must match if they do not match
        IsConnected = SerialManager.OpenPort(serialPort, baudRate, Parity, StopBits);
        // TODO, if this connection attempt fails we need to show the user that it failed and give them the option to retry, change settings, or cancel
        SerialManager.InUsePorts.CollectionChanged += ConnectionChangedListener;
    }

    public bool SerialPortValidation(string serialPort)
    {
        var ports = System.IO.Ports.SerialPort.GetPortNames();
        if (ports.Length == 0)
        {
            return false; // No serial ports available
        }
        return ports.Contains(serialPort);
    }

    public bool BaudRateValidation(int baudRate)
    {
        return baudRate > 0 && baudRate <= 115200; // Common baud rates for RTU
    }

    public void ConnectionChangedListener(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (var item in e.NewItems!)
            {
                if (item is string newConnection)
                {
                    if (newConnection == SerialPort)
                    {
                        IsConnected = true; // Set to true if the connection is added
                    }
                }
            }
        }
        if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset)
        {
            foreach (var item in e.OldItems!)
            {
                if (item is string oldConnection)
                {
                    if (oldConnection == SerialPort)
                    {
                        IsConnected = false; // Set to false if the connection is removed
                    }
                }
            }
        }
    }

    [RelayCommand]
    public void ConnectCommand()
    {

    }

    [RelayCommand]
    public void DisconnectCommand()
    {

    }

    [RelayCommand]
    public void TestConnectionCommand()
    {

    }

    [RelayCommand]
    public void ToggleConnectionCommand()
    {

    }

    [RelayCommand]
    public void SaveChangesCommand()
    {

    }

}