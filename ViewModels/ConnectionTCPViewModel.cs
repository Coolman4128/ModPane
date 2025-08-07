using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModPan.ViewModels;
using ModPane.Models;
using ModPane.ViewModels;

namespace Modpane.ViewModels;

public partial class ConnectionTCPViewModel : ConnectionViewModel
{
    [ObservableProperty]
    private string _ipAddress = string.Empty;

    [ObservableProperty]
    private int _port;

    [ObservableProperty]
    private int _timeout = 1000; // Default to 1000ms

    public ObservableCollection<string> InUseConnections => TcpManager.InUseConnections;

    private string ConnectionKey => $"{IpAddress}:{Port}";

    public ConnectionTCPViewModel(ConnectionsPageViewModel parent, string name, string ipAddress, int port)
        : base(parent, name, "TCP")
    {
        if (!IpAddressValidation(ipAddress) || !PortValidation(port))
        {
            throw new ArgumentException("Invalid IP address or port format.", nameof(ipAddress));
        }
        IpAddress = ipAddress;
        Port = port;

        // Don't attempt to connect immediately in constructor to avoid UI thread blocking
        // Connection will be established when user explicitly connects
        IsConnected = false;
        TcpManager.InUseConnections.CollectionChanged += ConnectionChangedListener;
        
        // Check if connection already exists (in case it was opened elsewhere)
        IsConnected = TcpManager.IsConnectionOpen(IpAddress, Port);
    }

    private async Task<bool> OpenConnectionAsync()
    {
        try
        {
            return await TcpManager.OpenConnectionAsync(IpAddress, Port, Timeout);
        }
        catch
        {
            return false;
        }
    }

    public bool IpAddressValidation(string ipAddress)
    {
        string pattern = @"^(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)"
                       + @"(\.(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)){3}$";
        return System.Text.RegularExpressions.Regex.IsMatch(ipAddress, pattern);
    }

    public bool PortValidation(int port)
    {
        return port > 0 && port <= 65535;
    }

    public void ConnectionChangedListener(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (var item in e.NewItems!)
            {
                if (item is string newConnection)
                {
                    if (newConnection == ConnectionKey)
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
                    if (oldConnection == ConnectionKey)
                    {
                        IsConnected = false; // Set to false if the connection is removed
                    }
                }
            }
        }
    }

    [RelayCommand]
    public async Task ConnectCommand()
    {
        if (!IsConnected)
        {
            IsConnected = await TcpManager.OpenConnectionAsync(IpAddress, Port, Timeout);
        }
    }

    [RelayCommand]
    public void DisconnectCommand()
    {
        if (IsConnected)
        {
            TcpManager.CloseConnection(IpAddress, Port);
            IsConnected = false;
        }
    }
    
    [RelayCommand]
    public async Task TestConnectionCommand()
    {
        // Test connection by attempting to open and immediately close it
        var wasConnected = IsConnected;
        try
        {
            if (!wasConnected)
            {
                bool testResult = await TcpManager.OpenConnectionAsync(IpAddress, Port, Timeout);
                if (testResult)
                {
                    // Connection test successful, but close it if we weren't originally connected
                    TcpManager.CloseConnection(IpAddress, Port);
                }
                // TODO: Show user the test result (success/failure)
            }
            else
            {
                // Already connected, just verify the connection is still valid
                bool isStillConnected = TcpManager.IsConnectionOpen(IpAddress, Port);
                // TODO: Show user the test result
            }
        }
        catch
        {
            // TODO: Show user that the test failed
        }
    }

    [RelayCommand]
    public async Task ToggleConnectionCommand()
    {
        if (IsConnected)
        {
            DisconnectCommand();
        }
        else
        {
            await ConnectCommand();
        }
    }

    [RelayCommand]
    public void SaveChangesCommand()
    {
        // TODO: Implement saving changes to configuration
        // This might involve persisting the connection settings to a configuration file or database
    }

    public override async void Connect()
    {
        await ConnectCommand();
    }

    public override void Disconnect()
    {
        DisconnectCommand();
    }
    
}