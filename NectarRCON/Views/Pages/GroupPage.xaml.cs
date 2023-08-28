using System.Windows.Controls;
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
            if (sender is ListView listView)
            {
                listView.SelectedIndex = -1;
            }
        }
    }
}
