using NectarRCON.ViewModels;

namespace NectarRCON.Views.Pages
{
    /// <summary>
    /// ServersPage.xaml 的交互逻辑
    /// </summary>
    public partial class ServersPage
    {
        public ServersPageViewModel ViewModel
        {
            get;
        }
        public ServersPage()
        {
            ViewModel = App.GetService<ServersPageViewModel>();
            InitializeComponent();
            DataContext = this;
        }
    }
}
