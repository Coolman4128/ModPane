using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModPan.ViewModels;
using ModPane.Models;
using ModPane.ViewModels;
using ModPane.Views;
using Modpane.ViewModels;

namespace ModPane.ViewModels
{
    public partial class AddConnectionDialogViewModel : ObservableValidator
    {
        [ObservableProperty]
        [Required(ErrorMessage = "Connection name is required")]
        [MinLength(1, ErrorMessage = "Connection name cannot be empty")]
        private string _connectionName = string.Empty;

        [ObservableProperty]
        private string _selectedConnectionType = "TCP";

        [ObservableProperty]
        private string _ipAddress = "192.168.1.1";

        [ObservableProperty]
        private int _port = 502;

        [ObservableProperty]
        private string _selectedSerialPort = string.Empty;

        [ObservableProperty]
        private int _baudRate = 9600;

        [ObservableProperty]
        private string _selectedParity = "None";

        [ObservableProperty]
        private bool _stopBits = true; // True = 1 stop bit, False = 2 stop bits

        [ObservableProperty]
        private int _timeout = 1000;

        public ObservableCollection<string> ConnectionTypes { get; } = new() { "TCP", "RTU" };
        public ObservableCollection<string> AvailableSerialPorts => SerialManager.AvailablePorts;
        public ObservableCollection<string> ParityOptions { get; } = new() { "None", "Even", "Odd" };
        public ObservableCollection<int> CommonBaudRates { get; } = new() { 9600, 19200, 38400, 57600, 115200 };

        public bool IsTcpSelected => SelectedConnectionType == "TCP";
        public bool IsRtuSelected => SelectedConnectionType == "RTU";
        public bool HasAvailableSerialPorts => AvailableSerialPorts.Count > 0;

        public ConnectionViewModel? ResultConnection { get; private set; }
        public bool DialogResult { get; private set; }

        public AddConnectionDialogViewModel()
        {
            // Subscribe to changes in available serial ports
            SerialManager.AvailablePorts.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(HasAvailableSerialPorts));
            };
        }

        partial void OnSelectedConnectionTypeChanged(string value)
        {
            OnPropertyChanged(nameof(IsTcpSelected));
            OnPropertyChanged(nameof(IsRtuSelected));
            OnPropertyChanged(nameof(HasAvailableSerialPorts));
            
            // Set default serial port if switching to RTU and none selected
            if (value == "RTU" && string.IsNullOrEmpty(SelectedSerialPort) && AvailableSerialPorts.Count > 0)
            {
                SelectedSerialPort = AvailableSerialPorts[0];
            }
        }

        [RelayCommand]
        public void Ok()
        {
            if (!ValidateInput())
                return;

            try
            {
                ResultConnection = CreateConnection();
                DialogResult = true;
                _dialog?.Close(true);
            }
            catch (Exception)
            {
                // TODO: Show error message to user
                // For now, just return without setting DialogResult
                return;
            }
        }

        [RelayCommand]
        public void Cancel()
        {
            DialogResult = false;
            ResultConnection = null;
            _dialog?.Close(false);
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(ConnectionName))
                return false;

            if (SelectedConnectionType == "TCP")
            {
                return ValidateTcpSettings();
            }
            else if (SelectedConnectionType == "RTU")
            {
                return ValidateRtuSettings();
            }

            return false;
        }

        private bool ValidateTcpSettings()
        {
            // Validate IP address
            string pattern = @"^(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)"
                           + @"(\.(25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)){3}$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(IpAddress, pattern))
                return false;

            // Validate port
            if (Port <= 0 || Port > 65535)
                return false;

            return true;
        }

        private bool ValidateRtuSettings()
        {
            // Check if there are any available serial ports
            if (!HasAvailableSerialPorts)
                return false;

            // Validate serial port
            if (string.IsNullOrEmpty(SelectedSerialPort))
                return false;

            // Validate baud rate
            if (BaudRate <= 0 || BaudRate > 115200)
                return false;

            return true;
        }

        private ConnectionsPageViewModel? _parentViewModel;
        private AddConnectionDialog? _dialog;

        public void SetParentViewModel(ConnectionsPageViewModel parent)
        {
            _parentViewModel = parent;
        }

        public void SetDialog(AddConnectionDialog dialog)
        {
            _dialog = dialog;
        }

        private ConnectionViewModel CreateConnection()
        {
            if (_parentViewModel == null)
                throw new InvalidOperationException("Parent view model must be set before creating connection");

            if (SelectedConnectionType == "TCP")
            {
                return new ConnectionTCPViewModel(_parentViewModel, ConnectionName, IpAddress, Port)
                {
                    Timeout = Timeout
                };
            }
            else // RTU
            {
                var connection = new ConnectionRTUViewModel(_parentViewModel, ConnectionName, SelectedSerialPort, BaudRate)
                {
                    Timeout = Timeout,
                    StopBits = StopBits
                };

                // Set parity
                connection.Parity = SelectedParity switch
                {
                    "Even" => true,
                    "Odd" => false,
                    _ => null // None
                };

                return connection;
            }
        }
    }
}
