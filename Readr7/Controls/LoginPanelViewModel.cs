using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Readr7.Services;

namespace Readr7.Controls
{
    public class LoginPanelViewModel : ViewModelBase
    {
        private String _username;
        public String Username
        {
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    _username = value;
                    RaisePropertyChanged("Username");
                }
            }
        }

        private String _password;
        public String Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    RaisePropertyChanged("Password");
                }
            }
        }

        public RelayCommand LoginCommand { get; private set; }

        public LoginPanelViewModel(GoogleReaderService googleReaderService)
        {
            LoginCommand = new RelayCommand(() =>
            {
                googleReaderService.Authenticate(Username, Password);
            });
        }
    }
}
