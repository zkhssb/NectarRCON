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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace NectarRCON.Views.Pages
{
    /// <summary>
    /// GroupPage.xaml 的交互逻辑
    /// </summary>
    public partial class GroupPage : UiPage
    {
        public GroupPage()
        {
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender is ListView listView)
            {
                listView.SelectedIndex = -1;
            }
        }
    }
}
