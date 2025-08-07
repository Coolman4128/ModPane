using System;
using CommunityToolkit.Mvvm.ComponentModel;
using ModPane.ViewModels;

namespace ModPan.ViewModels;

public partial class ConnectionViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _modbusType = string.Empty; // This can be either "TCP" or "RTU"

    [ObservableProperty]
    private bool _isConnected;

    public ConnectionsPageViewModel parent { get; }

    public ConnectionViewModel(ConnectionsPageViewModel parent, string name, string modbusType)
    {
        this.parent = parent;
        Name = name;
        ModbusType = modbusType;
        IsConnected = false; // Default to not connected
    }

    public virtual void Connect()
    {
        throw new NotImplementedException("Connect method must be implemented in derived classes.");
    }

    public virtual void Disconnect()
    {
        throw new NotImplementedException("Disconnect method must be implemented in derived classes.");
    }
    

}