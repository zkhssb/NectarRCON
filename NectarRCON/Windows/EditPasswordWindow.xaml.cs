using NectarRCON.ViewModels;
using System.Windows;

namespace NectarRCON.Windows
{
    /// <summary>
    /// EditPasswordWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditPasswordWindow
    {
        public EditPasswordWindow()
        {
            InitializeComponent();
            ((EditPasswordWindowViewModel)this.DataContext).SetWindow(this);
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
            => Close();
    }
}
