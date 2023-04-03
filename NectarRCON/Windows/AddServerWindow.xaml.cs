using NectarRCON.ViewModels;
using System.Windows;

namespace NectarRCON.Windows
{
    /// <summary>
    /// AddServerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddServerWindow
    {
        public AddServerWindowViewModel ViewModel
        {
            get;
        }
        public AddServerWindow(AddServerWindowViewModel viewModel)
        {
            ViewModel = viewModel;
            viewModel.SetWindow(this);
            InitializeComponent();
            DataContext = this;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
            => Close();
    }
}
