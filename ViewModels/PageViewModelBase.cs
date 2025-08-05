using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ModPane.ViewModels
{
    public abstract partial class PageViewModelBase : ViewModelBase
    {
        [ObservableProperty]
        private Control _headerContent;

        [ObservableProperty]
        private Control _mainContent;

        [ObservableProperty]
        private Control? _sidePanelContent;

        [ObservableProperty]
        private bool _isSidePanelVisible;

        public PageViewModelBase(Control headerContent, Control mainContent, Control? sidePanelContent = null)
        {
            HeaderContent = headerContent;
            MainContent = mainContent;
            SidePanelContent = sidePanelContent;
            IsSidePanelVisible = false; // Default to hidden
        }
        
    }


}
