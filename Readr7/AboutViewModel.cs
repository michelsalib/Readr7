using System;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Readr7.Controls;
using Readr7.Model;
using Readr7.Model.Entites;
using Readr7.Services;

namespace Readr7
{
    public class AboutViewModel : ViewModelBase
    {
        public RelayCommand<String> NavigateCommand { get; private set; }

        public AboutViewModel()
        {
            NavigateCommand = new RelayCommand<string>(s =>
            {
                new WebBrowserTask()
                {
                    Uri = new Uri(s)
                }.Show();
            });
        }
    }
}
