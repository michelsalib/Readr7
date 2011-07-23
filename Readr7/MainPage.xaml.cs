using System;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Readr7
{
    public partial class MainPage : PhoneApplicationPage
    {
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
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).IsEnabled = authenticated;
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[1]).IsEnabled = authenticated;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = authenticated;

            // now listen for changes
            Messenger.Default.Unregister<bool>(this, "authenticate");
            Messenger.Default.Register<bool>(this, "authenticate", b =>
            {
                ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).IsEnabled = b;
                ((ApplicationBarMenuItem)ApplicationBar.MenuItems[1]).IsEnabled = b;
                ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = b;
            });
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
    }
}