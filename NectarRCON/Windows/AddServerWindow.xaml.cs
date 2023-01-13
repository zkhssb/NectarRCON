using NectarRCON.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
