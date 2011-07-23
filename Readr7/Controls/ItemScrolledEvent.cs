using System;
using Readr7.Model;

namespace Readr7.Controls
{
    public class ItemScrolledEvent : EventArgs
    {
        public Item Item { get; private set; }

        public ItemScrolledEvent(Item item)
        {
            Item = item;
        }
    }
}
