using GalaSoft.MvvmLight;
using Readr7.Services;
using System.Linq;
using System.Collections.ObjectModel;
using Readr7.Model;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Tasks;
using System;
using GalaSoft.MvvmLight.Messaging;
using Readr7.Controls;

namespace Readr7
{
    public class MainViewModel : ViewModelBase
    {
        private bool _isAuthenticated;
        public bool IsAuthenticated
        {
            get { return _isAuthenticated; }
            set
            {
                {
                    if (_isAuthenticated != value)
                    {
                        _isAuthenticated = value;
                        RaisePropertyChanged();
                        RaisePropertyChanged("Title");
                    }
                }
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                {
                    if (_isLoading != value)
                    {
                        _isLoading = value;
                        RaisePropertyChanged();
                        RaisePropertyChanged("Title");
                    }
                }
            }
        }

        public String Title
        {
            get
            {
                if (!IsAuthenticated)
                    return "Login";
                else if (Feed != null && Feed.Items.Count == 0)
                    return "Reading list is empty";
                else if (Feed != null)
                    return Feed.Title.Substring(0, Feed.Title.IndexOf(" in Google Reader"));
                else
                    return "Loading...";
            }
        }

        private Feed _feed;
        public Feed Feed
        {
            get
            {
                return _feed;
            }
            set{
                if(_feed != value){
                    _feed = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged("Title");
                }
            }
        }

        private readonly GoogleReaderService _googleReaderService;
        private readonly ConfigViewModel _config;

        public RelayCommand<Item> FeedSelectedCommand { get; private set; }
        public RelayCommand<Item> ReadCommand { get; private set; }
        public RelayCommand<Item> UnreadCommand { get; private set; }
        public RelayCommand<ItemScrolledEvent> ItemScrolled { get; private set; }
        public RelayCommand<String> NavigateCommand { get; private set; }
        
        public MainViewModel(GoogleReaderService googleReaderService, INavigationService navigationService, ConfigViewModel config)
        {
            // init
            _googleReaderService = googleReaderService;
            _config = config;

            // commands
            FeedSelectedCommand = new RelayCommand<Item>(i =>
            {
                if (i != null)
                {
                    _read(i);
                    new WebBrowserTask()
                    {
                        Uri = new Uri(i.Url)
                    }.Show();
                }
            });
            ItemScrolled = new RelayCommand<ItemScrolledEvent>(o =>
            {
                var readItems = Feed.Items.TakeWhile(i => i != o.Item);
                foreach (var item in readItems)
                {
                    _read(item);
                }
            });
            ReadCommand = new RelayCommand<Item>(i => _read(i));
            UnreadCommand = new RelayCommand<Item>(i => _read(i, false));
            NavigateCommand = new RelayCommand<string>(s =>
            {
                new WebBrowserTask()
                {
                    Uri = new Uri(s)
                }.Show();
            });

            // listen to view messenger events
            Messenger.Default.Register<bool>(this, "logout", b => googleReaderService.Logout());
            Messenger.Default.Register<bool>(this, "refresh", b => _refresh());
            Messenger.Default.Register<bool>(this, "config", b => navigationService.NavigateTo(ViewModelLocator.ConfigView));

            // init display
            googleReaderService.Authenticated += (s, e) =>
            {
                if (e.IsAuthenticated)
                {
                    _logIn();
                }
                else
                {
                    _logOut();
                }
            };
            if (googleReaderService.IsAuthenticated)
            {
                _logIn();
            }
            else
            {
                _logOut();
            }

            // listen to loading event
            IsLoading = _googleReaderService.IsLoading;
            googleReaderService.Loading += (s, e) => { IsLoading = e.IsLoading; };
        }

        private void _read(Item item, bool read = true)
        {

            if (!item.Read)
            {
                item.Read = read;
                _googleReaderService.MarkAsRead(item, read);
            }
        }

        private void _logOut()
        {
            IsAuthenticated = false;
            Feed = null;
            Messenger.Default.Send<bool>(false, "authenticate");
        }

        private void _logIn()
        {
            IsAuthenticated = true;
            _config.Init();
            _refresh();
            Messenger.Default.Send<bool>(true, "authenticate");
        }

        private void _refresh()
        {
            if (IsAuthenticated)
            {
                if (_config.Tag != Tag.All)
                {
                    _googleReaderService.GetFeed(f => Feed = f, _config.Tag, _config.ShowRead);
                }
                else
                {
                    _googleReaderService.GetFeed(f => Feed = f, _config.ShowRead);
                }
            }
        }
    }
}
