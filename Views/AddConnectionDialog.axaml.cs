using Avalonia.Controls;
using ModPane.ViewModels;

namespace ModPane.Views
{
    public partial class AddConnectionDialog : Window
    {
        public AddConnectionDialog()
        {
            InitializeComponent();
        }

        public AddConnectionDialog(AddConnectionDialogViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}
