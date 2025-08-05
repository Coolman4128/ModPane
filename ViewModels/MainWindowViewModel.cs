using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
            Pages = new ObservableCollection<PageViewModelBase>
            {
                new HomeViewModel(new TextBlock { Text = "Home Header" }, 
                                   new TextBlock { Text = "Home Main Content" }, 
                                   new TextBlock { Text = "Home Side Panel" }),
                // Add other page view models here
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

