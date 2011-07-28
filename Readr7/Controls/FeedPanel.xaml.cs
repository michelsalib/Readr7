using System.Windows;
using System.Linq;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace Readr7.Controls
{
    public partial class FeedPanel : UserControl
    {
        public FeedPanel()
        {
            InitializeComponent();

            Messenger.Default.Register<bool>(this, "refresh", b =>
            {
                FeedList.ScrollToTop();
            });
        }

        private void ContextMenuOpened(object sender, RoutedEventArgs e)
        {
            this.IsHitTestVisible = false;
        }

        private void ContextMenuClosed(object sender, RoutedEventArgs e)
        {
            this.IsHitTestVisible = true;
        }
    }
}
