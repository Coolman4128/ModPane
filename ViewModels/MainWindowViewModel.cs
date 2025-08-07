using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModPane.Views;
using System.Collections.ObjectModel;

namespace ModPane.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private PageViewModelBase _currentPage;

        public ObservableCollection<PageViewModelBase> Pages { get; }

        public MainWindowViewModel()
        {
            var ConnectionsPage = new ConnectionsPageViewModel(new Control(), new Control());
            var ConnectionPageView = new ConnectionsPageView{
                DataContext = ConnectionsPage
            };
            var ConnectionHeaderView = new ConnectionsHeaderView{
                DataContext = ConnectionsPage
            };
            ConnectionsPage.MainContent = ConnectionPageView;
            ConnectionsPage.HeaderContent = ConnectionHeaderView;
            Pages = new ObservableCollection<PageViewModelBase>
            {
                ConnectionsPage
            };

            _currentPage = Pages[0];
        }

        [RelayCommand]
        private void NavigateTo(PageViewModelBase page)
        {
            CurrentPage = page;
        }
    }
}

