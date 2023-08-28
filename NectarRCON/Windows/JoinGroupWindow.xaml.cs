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
using Wpf.Ui.Controls;

namespace NectarRCON.Windows
{
    /// <summary>
    /// JoinGroupWindow.xaml 的交互逻辑
    /// </summary>
    public partial class JoinGroupWindow : UiWindow
    {
        public string? SelectedServer = null;
        public JoinGroupWindow()
        {
            InitializeComponent();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void AddBlackList(string value)
        {
            ((JoinGroupWindowViewModel)DataContext).BlackList?.Add(value);
            ((JoinGroupWindowViewModel)DataContext).ServerCollectionView?.Refresh();
        }

        private void ServersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0)
            {
                SelectedServer = e.AddedItems[0]?.ToString();
                Close();
            }
        }
    }
}
