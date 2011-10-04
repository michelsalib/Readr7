using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;

namespace Readr7.Controls
{
    public class CompressableListBox : MultiselectList
    {
        public event EventHandler BottomReached;

        private bool _alreadyHookedScrollEvents = false;

        public CompressableListBox()
        {
            Loaded += (s, e) =>
            {
                // prevent several hooking
                if (_alreadyHookedScrollEvents)
                    return;
                _alreadyHookedScrollEvents = true;

                // hook
                var sv = (ScrollViewer)FindElementRecursive(this, typeof(ScrollViewer));
                if (sv != null)
                {
                    FrameworkElement element = VisualTreeHelper.GetChild(sv, 0) as FrameworkElement;
                    if (element != null)
                    {
                        // get visual state
                        VisualStateGroup vgroup = FindVisualState(element, "VerticalCompression");
                        if (vgroup != null)
                        {
                            vgroup.CurrentStateChanging += (se, ev) =>
                            {
                                // on bottom compression, need to call event
                                if (ev.NewState.Name == "CompressionBottom")
                                {
                                    if (BottomReached != null)
                                        BottomReached(this, new EventArgs());
                                }
                            };
                        }
                    }
                }
            };
        }

        private UIElement FindElementRecursive(FrameworkElement parent, Type targetType)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            UIElement returnElement = null;
            if (childCount > 0)
            {
                for (int i = 0; i < childCount; i++)
                {
                    Object element = VisualTreeHelper.GetChild(parent, i);
                    if (element.GetType() == targetType)
                    {
                        return element as UIElement;
                    }
                    else
                    {
                        returnElement = FindElementRecursive(VisualTreeHelper.GetChild(parent, i) as FrameworkElement, targetType);
                    }
                }
            }
            return returnElement;
        }

        private VisualStateGroup FindVisualState(FrameworkElement element, string name)
        {
            if (element == null)
                return null;

            IList groups = VisualStateManager.GetVisualStateGroups(element);
            foreach (VisualStateGroup group in groups)
                if (group.Name == name)
                    return group;

            return null;
        }

        public void ScrollToTop()
        {
            var scroller = GetTemplateChild("ScrollViewer") as ScrollViewer;
            scroller.InvalidateScrollInfo();
            scroller.ScrollToVerticalOffset(0);
        }
    }
}
