﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Readr7.Controls;
using GalaSoft.MvvmLight;
using Readr7.Resources.DesignData;

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
            }
        }

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

        public static string ConfigView = "/ConfigView.xaml";
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