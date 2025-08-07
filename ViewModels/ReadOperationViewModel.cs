using System;
using CommunityToolkit.Mvvm.ComponentModel;
using ModPan.ViewModels;

namespace ModPane.ViewModels;

public partial class ReadOperationViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _registerType = "Holding"; // Default, valid options are Holding, Input, Coil, Discrete

    [ObservableProperty]
    private int _startingAddress = 0; // Starting address for the read operation

    [ObservableProperty]
    private int _quantity = 1; // Default to reading 1 register

    [ObservableProperty]
    private ConnectionViewModel _parent;

    [ObservableProperty]
    private int _attemptPollingRate = 1000; // Default polling rate in milliseconds

    public ReadOperationViewModel(ConnectionViewModel parent, string registerType = "Holding", int startingAddress = 0, int quantity = 1)
    {
        Parent = parent;
        RegisterType = registerType;
        StartingAddress = startingAddress;
        Quantity = quantity;
        if (!Validate())
        {
            throw new ArgumentException("Invalid read operation parameters.", nameof(registerType));
        }
    }

    public bool Validate()
    {
        if (RegisterType != "Holding" && RegisterType != "Input" && RegisterType != "Coil" && RegisterType != "Discrete")
        {
            return false;
        }
        if (RegisterType == "Holding" || RegisterType == "Input")
        {
            if (Quantity < 1 || Quantity > 125)
            {
                return false; // Modbus limits the number of registers to read in one operation
            }
        }
        else if (RegisterType == "Coil" || RegisterType == "Discrete")
        {
            if (Quantity < 1 || Quantity > 2000)
            {
                return false; // Modbus limits the number of coils/discrete inputs to read in one operation
            }
        }
        if (StartingAddress < 0 || StartingAddress > 65535)
        {
            return false; // Starting address must be within valid Modbus range
        }
        return true;
    }
}