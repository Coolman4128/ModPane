using System.Collections.ObjectModel;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModPan.ViewModels;
using Tmds.DBus.Protocol;

namespace ModPane.ViewModels
{
    public partial class ConnectionsPageViewModel : PageViewModelBase
    {

        public ObservableCollection<ConnectionViewModel> Connections { get; } = new ObservableCollection<ConnectionViewModel>();

        [ObservableProperty]
        private ConnectionViewModel? _selectedSettingsConnection;

        [ObservableProperty]
        private ConnectionViewModel? _selectedConnection;

        public ConnectionsPageViewModel(Control mainView, Control headerView) : base(mainView, headerView)
        {

        }

        partial void OnSelectedSettingsConnectionChanged(ConnectionViewModel? value)
        {
            if (value == null)
            {
                IsSidePanelVisible = true;
                SidePanelContent = null;
                return;
            }
            else
            {
                // Create new side panel content here at some point
                IsSidePanelVisible = true;
                return;
            }
        }

        [RelayCommand]
        public void AddConnectionCommand()
        {
            
        }

        [RelayCommand]
        public void EditConnectionCommand()
        {

        }

        [RelayCommand]
        public void DeleteConnectionCommand()
        {

        }

        [RelayCommand]
        public void SaveConfigCommand()
        {

        }

        [RelayCommand]
        public void LoadConfigCommand()
        {

        }

        [RelayCommand]
        public void AddTcpConnectionCommand()
        {

        }

        [RelayCommand]
        public void AddRtuConnectionCommand()
        {

        }

        [RelayCommand]
        public void ClearConfigCommand()
        {

        }

        [RelayCommand]
        public void CloseSidePanelCommand()
        {
            SelectedSettingsConnection = null;
        }

    }
}