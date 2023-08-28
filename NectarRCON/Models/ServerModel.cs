using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NectarRCON.Interfaces;
using NectarRCON.Rcon;
using NectarRCON.Services;
using NectarRCON.ViewModels;
using NectarRCON.Windows;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Mvvm.Contracts;

namespace NectarRCON.Models
{
    public partial class ServerModel : ObservableObject
    {
        private readonly IServerPasswordService _passwordService;

        private readonly IServerInformationService _informationService;
        private readonly IConnectingDialogService _connectingDialogService;
        private readonly IRconConnectionInfoService _connectionInfoService;
        private readonly INavigationService _navigationService;
        private readonly IServerPasswordService _serverPasswordService;

        private readonly ILanguageService _languageService;
        private readonly ServerInformation _information;

        private readonly ServersPageViewModel _viewModel;

        private readonly IRconConnection _rconConnection;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _address;

#pragma warning disable CS8618 // CS8618 你说的对,但是我已经初始化了
        public ServerModel(ServerInformation information, ServersPageViewModel viewModel)
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        {
            _passwordService = App.GetService<IServerPasswordService>();
            _informationService = App.GetService<IServerInformationService>();
            _languageService = App.GetService<ILanguageService>();
            _connectingDialogService = App.GetService<IConnectingDialogService>();
            _rconConnection = App.GetService<IRconConnection>(typeof(RconSingleConnection));
            _connectionInfoService = App.GetService<IRconConnectionInfoService>();
            _navigationService = App.GetService<INavigationService>();
            _serverPasswordService = App.GetService<IServerPasswordService>();
            _information = information;
            _viewModel = viewModel;

            Name = information.Name;
            Address = $"{information.Address}:{information.Port}";
        }

        private void EditPass()
        {
            _passwordService.Select(_information);
            EditPasswordWindow window = new EditPasswordWindow();
            window.ShowDialog();
        }

        [RelayCommand]
        public void EditPassword()
        {
            EditPass();
        }

        [RelayCommand]
        public void Edit()
        {
            _passwordService.Select(_information);
            EditServerWindow window = new EditServerWindow();
            window.ShowDialog();
            _viewModel.Refresh();
        }

        [RelayCommand]
        public void Delete()
        {
            var result = MessageBox.Show(_languageService.GetKey("ui.server_page.confirm"), _languageService.GetKey("ui.server_page.menu.delete"), MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _informationService.RemoveServer(Name);
                _informationService.Save();
                _passwordService.Remove(_information);
                _passwordService.Save();
                _viewModel.Refresh();
            }
        }

        [RelayCommand]
        public async void Connect()
        {
            if (_rconConnection.IsConnecting())
                return;
            try
            {
                _connectingDialogService.Show();
                if (_rconConnection.IsConnected())
                {
                    _rconConnection.Close();
                }
                var server = _passwordService.Get(_information);
                if (server == null)
                {
                    EditPass();
                }
                else if (server.Password == null && !server.IsEmpty)
                {
                    EditPass();
                }
                _connectionInfoService.Clear();
                _connectionInfoService.AddInformation(Name);
                await Task.Run(_rconConnection.Connect);
                _serverPasswordService.Select(_information);
                if (_rconConnection.IsConnected())
                    _navigationService.Navigate(0);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(_languageService.GetKey("text.server.connect.fail.text")
                    .Replace("\\n", "\n")
                    .Replace("%s", ex.Message), _languageService.GetKey("text.error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (AuthenticationException)
            {
                MessageBox.Show(_languageService.GetKey("text.server.connect.auth_fail")
                .Replace("\\n", "\n"), _languageService.GetKey("text.error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _connectingDialogService.Close();
            }
        }
    }
}
