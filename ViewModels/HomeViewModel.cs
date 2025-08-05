using Avalonia.Controls;
using ModPane.ViewModels;

namespace ModPane.ViewModels
{
    public class HomeViewModel : PageViewModelBase
    {
        public HomeViewModel(Control headerContent, Control mainContent, Control? sidePanelContent = null)
            : base(headerContent, mainContent, sidePanelContent)
        {
        }
    }
}
