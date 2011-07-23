using System.Windows;
using System.Windows.Controls;

namespace Readr7.Controls
{
    public partial class FeedPanel : UserControl
    {
        public FeedPanel()
        {
            InitializeComponent();
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
