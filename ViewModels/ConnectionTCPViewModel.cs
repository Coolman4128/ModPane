using System;
using System.Net.Sockets;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModPan.ViewModels;
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

    public ConnectionTCPViewModel(ConnectionsPageViewModel parent, string name, string ipAddress, int port)
        : base(parent, name, "TCP")
    {
        if (!IpAddressValidation(ipAddress) || !PortValidation(port))
        {
            throw new ArgumentException("Invalid IP address or port format.", nameof(ipAddress));
        }
        IpAddress = ipAddress;
        Port = port;
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