using System;
using GalaSoft.MvvmLight;
using Readr7.Controls;
using Readr7.Model;
using Readr7.Resources.DesignData;
using System.Windows;
using Gi7.Utils;

namespace Readr7.Services
{
    public class ViewModelLocator
    {
        public static GoogleReaderService GoogleReaderService { get; private set; }
        public static INavigationService NavigationService { get; private set; }

        static ViewModelLocator()
        {
            if (!ViewModelBase.IsInDesignModeStatic)
            {
                GoogleReaderService = new GoogleReaderService();
                NavigationService = new NavigationService();

                GoogleReaderService.Authenticated += (s,e) => {
                    if(e.IsAuthenticated == false && !NavigationService.CurrentUri().Contains(MainViewUrl)){
                        NavigationService.NavigateTo(ViewModelLocator.MainViewUrl);
                    }
                };
                GoogleReaderService.ConnectionError += (s,e) => MessageBox.Show("Server unreachable.");
                GoogleReaderService.Unauthorized += (s,e) => MessageBox.Show("Wrong credentials.");
                GoogleReaderService.Loading += (s, e) => GlobalLoading.Instance.IsLoading = e.IsLoading;
            }   
        }

        public static string MainViewUrl = "/MainPage.xaml";
        public Object MainViewModel
        {
            get
            {
                if (ViewModelBase.IsInDesignModeStatic)
                {
                    return new MainViewDataModel();
                }
                else
                {
                    return new MainViewModel(GoogleReaderService, NavigationService, ConfigViewModel as ConfigViewModel);
                }
            }
        }

        public LoginPanelViewModel LoginPanelViewModel
        {
            get
            {
                return new LoginPanelViewModel(GoogleReaderService);
            }
        }

        public static string AboutUrl = "/AboutView.xaml";
        public Object AboutViewModel
        {
            get
            {
                if (ViewModelBase.IsInDesignModeStatic)
                {
                    return null;
                }
                else
                {
                    return new AboutViewModel();
                }
            }
        }

        public static string ConfigUrl = "/ConfigView.xaml";
        public static Object _configViewModel;
        public Object ConfigViewModel
        {
            get
            {
                if (ViewModelBase.IsInDesignModeStatic)
                {
                    return null;
                }
                else
                {
                    if(_configViewModel == null)
                        _configViewModel = new ConfigViewModel(GoogleReaderService);
                    return _configViewModel;
                }
            }
        }
    }
}
