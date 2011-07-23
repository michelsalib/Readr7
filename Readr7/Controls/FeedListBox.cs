using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Readr7.Model;

namespace Readr7.Controls
{
    public class FeedListBox : ListBox
    {
        public event EventHandler<ItemScrolledEvent> ItemScrolled;

        public FeedListBox()
        {
            ManipulationStarted += _manipulationStarted;
        }

        private void _manipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            if (ItemScrolled != null)
            {
                var currentListBoxItem = GetCurrentListBoxItem();
                if (currentListBoxItem != null)
                {
                    var currentItem = currentListBoxItem.DataContext as Item;
                    ItemScrolled(this, new ItemScrolledEvent(currentItem));
                }
            }
        }

        public ListBoxItem GetCurrentListBoxItem()
        {
            return (VisualTreeHelper.FindElementsInHostCoordinates(new Point(200, 235), this)
                    .FirstOrDefault(t => t.GetType() == typeof(ListBoxItem)) as ListBoxItem);
        }
    }
}
