using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;

namespace Readr7
{
    public partial class ConfigView : PhoneApplicationPage
    {
        public ConfigView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            Messenger.Default.Send<bool>(true, "refresh");
        }
    }
}