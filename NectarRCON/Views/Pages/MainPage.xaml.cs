using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace NectarRCON.Views.Pages
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public void CloseCommandInputBoxPopup()
        {
            CommandInputBox.IsSuggestionListOpen = false;
        }
         
    }
}
