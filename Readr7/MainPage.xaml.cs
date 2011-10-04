using System;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Readr7
{
    public partial class MainPage : PhoneApplicationPage
    {
        private ApplicationBarIconButton _select;
        private ApplicationBarIconButton _markAsRead;
        private ApplicationBarIconButton _refresh;
        private ApplicationBarMenuItem _config;
        private ApplicationBarMenuItem _logout;


        // Constructor
        public MainPage()
        {
            // first init
            bool authenticated = false;
            Messenger.Default.Register<bool>(this, "authenticate", b =>
            {
                authenticated = b;
            });
            InitializeComponent();
            _select = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            _markAsRead = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            _refresh = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
            _config = (ApplicationBarMenuItem)ApplicationBar.MenuItems[0];
            _logout = (ApplicationBarMenuItem)ApplicationBar.MenuItems[1];
            ApplicationBar.Buttons.Remove(_markAsRead);
            if (!authenticated)
            {
                ApplicationBar.Buttons.Remove(_select);
                ApplicationBar.Buttons.Remove(_refresh);
                ApplicationBar.MenuItems.Remove(_config);
                ApplicationBar.MenuItems.Remove(_logout);
            }

            // now listen for changes
            Messenger.Default.Unregister<bool>(this, "authenticate");
            Messenger.Default.Register<bool>(this, "authenticate", b =>
            {
                if (b)
                {
                    ApplicationBar.Buttons.Add(_refresh);
                    ApplicationBar.Buttons.Add(_select);
                    ApplicationBar.MenuItems.Add(_config);
                    ApplicationBar.MenuItems.Add(_logout);
                }
                else
                {
                    ApplicationBar.Buttons.Remove(_select);
                    ApplicationBar.Buttons.Remove(_markAsRead);
                    ApplicationBar.Buttons.Remove(_refresh);
                    ApplicationBar.MenuItems.Remove(_config);
                    ApplicationBar.MenuItems.Remove(_logout);
                }
            });

            Messenger.Default.Register<bool>(this, "refresh", b =>
            {
                FeedList.ScrollToTop();
            });
        }

        private void Read(object sender, EventArgs e)
        {
            var model = DataContext as MainViewModel;
            foreach (var item in FeedList.SelectedItems)
            {
                model.ReadCommand.Execute(item);
            }
            FeedList.IsSelectionEnabled = false;
        }

        private void IsSelectionEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                ApplicationBar.Buttons.Add(_markAsRead);
            }
            else
            {
                ApplicationBar.Buttons.Remove(_markAsRead);
            }
        }

        private void Select(object sender, EventArgs e)
        {
            FeedList.IsSelectionEnabled = !FeedList.IsSelectionEnabled;
        }

        private void Logout(object sender, EventArgs e)
        {
            Messenger.Default.Send<bool>(true, "logout");
        }

        private void Refresh(object sender, EventArgs e)
        {
            Messenger.Default.Send<bool>(true, "refresh");
        }

        private void Configuration(object sender, EventArgs e)
        {
            Messenger.Default.Send<bool>(true, "config");
        }

        private void About(object sender, EventArgs e)
        {
            Messenger.Default.Send<bool>(true, "about");
        }
    }
}