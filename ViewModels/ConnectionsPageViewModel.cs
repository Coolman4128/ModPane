using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModPan.ViewModels;
using Modpane.ViewModels;
using ModPane.Views;
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
                switch (value)
                {
                    case ConnectionTCPViewModel tcp:
                        SidePanelContent = new ConnectionTCPSettingsView();
                        SidePanelContent.DataContext = tcp;
                        break;
                    case ConnectionRTUViewModel rtu:
                        SidePanelContent = new ConnectionRTUSettingsView();
                        SidePanelContent.DataContext = rtu;
                        break;
                    default:
                        SidePanelContent = null;
                        break;
                }
                IsSidePanelVisible = true;
                return;
            }
        }

        [RelayCommand]
        public async Task AddConnectionCommand()
        {
            var dialogViewModel = new AddConnectionDialogViewModel();
            dialogViewModel.SetParentViewModel(this);
            
            var dialog = new AddConnectionDialog(dialogViewModel);
            dialogViewModel.SetDialog(dialog);
            
            // Try to get the main window from the view hierarchy
            Window? mainWindow = null;
            if (MainContent is Control control)
            {
                mainWindow = control.FindLogicalAncestorOfType<Window>();
            }
            
            // Fallback to application main window if we can't find it through the hierarchy
            if (mainWindow == null && Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                mainWindow = desktop.MainWindow;
            }
            
            bool? result = null;
            if (mainWindow != null)
            {
                result = await dialog.ShowDialog<bool?>(mainWindow);
            }
            else
            {
                dialog.Show();
                // For now, we'll assume they clicked OK if no parent window
                result = true;
            }
            
            if (result == true && dialogViewModel.ResultConnection != null)
            {
                Connections.Add(dialogViewModel.ResultConnection);
            }
        }

        [RelayCommand]
        public void EditConnectionCommand(ConnectionViewModel connection)
        {
            SelectedSettingsConnection = connection;
            IsSidePanelVisible = true;
        }

        [RelayCommand]
        public void DeleteConnectionCommand(ConnectionViewModel connection)
        {
            Connections.Remove(connection);
            if (SelectedSettingsConnection == connection)
            {
                SelectedSettingsConnection = null;
            }
            if (SelectedConnection == connection)
            {
                SelectedConnection = null;
            }

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
        public void ClearConfigCommand()
        {

        }

        [RelayCommand]
        public void CloseSidePanelCommand()
        {
            SelectedSettingsConnection = null;
            IsSidePanelVisible = false;
        }

    }
}