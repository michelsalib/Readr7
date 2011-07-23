//===============================================================================
// Copyright  Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Diagnostics;

namespace Readr7.Controls
{
    /// <summary>
    /// The implementation of the inline ListPicker control
    /// </summary>
    public class ListPicker : ListBox
    {
        #region fields

        private double itemHeight;
        private bool expanded;
        private int selectedIndex;
        private Storyboard storyboard;
        private bool animating;
        private bool focused;
        private bool loaded;
        private ScrollViewer scrollViewer;
        //private int layoutUpdatedCounter;

        private Brush backgroundBrush;


        #endregion

        #region constructor

        public ListPicker()
        {                                  
            // This code should be for release tools
            this.backgroundBrush = (Brush)(Application.Current.Resources["PhoneTextBoxBrush"]);
                       
            this.DefaultStyleKey = typeof(ListPicker);           
            base.SelectionChanged += new SelectionChangedEventHandler(ListPicker_SelectionChanged);
            this.Unloaded += new RoutedEventHandler(ListPicker_Unloaded);
            this.SizeChanged += new SizeChangedEventHandler(ListPicker_SizeChanged);
        }
       

        void ListPicker_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((this.Items.Count > 0 && selectedIndex > -1) || !loaded)
            {
                Debug.WriteLine("ListPicker_SizeChanged before ScrollIntoView " + selectedIndex);
                try
                {
                    this.ScrollIntoView(this.Items[selectedIndex]);
                    loaded = true;
                }
                catch { }
            }
        }

        void ListPicker_Unloaded(object sender, RoutedEventArgs e)
        {
            this.SizeChanged += new SizeChangedEventHandler(ListPicker_SizeChanged);
        }

        #endregion

        #region overrides
       

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            this.focused = true;
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            this.focused = false;

            if (expanded && !animating)
            {
                this.HandleExpandCollapse();
                Debug.WriteLine("OnLostFocus after HandleExpandCollapse");
            }
         
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.scrollViewer = this.GetTemplateChild("ScrollViewer") as ScrollViewer;

            //if (this.Items.Count > 5)
            //{
            //    throw new InvalidOperationException("The control cannot have more than 5 items.");
            //}
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            // Capture the original height
            ListBoxItem item = this.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;

            Size size = base.ArrangeOverride(finalSize);

            // Resize the collapsed height of the control depending in the height of the item
            if (itemHeight == 0 && item != null)
            {
                this.itemHeight = item.ActualHeight;
                this.Height = this.itemHeight;
                size.Height = item.ActualHeight + 1;
            }

            return size;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            e.Handled = true;

            base.OnMouseLeftButtonDown(e);

           
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            // Change background color
            this.Background = new SolidColorBrush(Colors.White);

            this.CaptureMouse();

            this.HandleExpandCollapse();

        }
     

        public new object SelectedItem
        {
            get
            {
                if (selectedIndex == -1)
                    return null;

                return this.Items[selectedIndex];
            }
            set
            {
                base.SelectedItem = value;

            }
        }

        public new int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                this.selectedIndex = value;
               
                if (!this.animating)
                {
                    Debug.WriteLine("SelectedIndex before ScrollIntoView");
                    base.SelectedIndex = value;
                    this.ScrollIntoView(this.selectedIndex);
                    base.SelectedIndex = -1;
                }
            }
        }

        #endregion

        #region event handlersca

        void ListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            if (base.SelectedIndex > -1 && expanded) 
            {
                selectedIndex = base.SelectedIndex;
            }
        }       

        #endregion

        #region helper methods

        private void HandleExpandCollapse()
        {            

            if (this.Height <= this.itemHeight)
            {
                // Show border when expanded
                this.BorderThickness = new Thickness(2);
                // Create and begin animation   
                base.SelectedIndex = selectedIndex;      
                this.storyboard = GetDropDownAnimation(this.Height, this.itemHeight * this.Items.Count);
                this.storyboard.Completed += new EventHandler(storyboard_Completed);
                this.animating = true;
                this.storyboard.Begin();
                this.expanded = true;
            }
            else
            {
                // Hide border
                this.BorderThickness = new Thickness(0);
                // Restore background 
                this.Background = backgroundBrush;
               
                // Unselect an item in the listbox              
                ListBoxItem item = this.ItemContainerGenerator.ContainerFromIndex(this.selectedIndex) as ListBoxItem;
                if (item != null)
                {
                    item.IsSelected = false;
                }
                // Create and begin animation               
                this.storyboard = GetDropDownAnimation(this.Height, itemHeight);
                this.storyboard.Completed += new EventHandler(storyboard_Completed);
                this.animating = true;                
                this.storyboard.Begin();
                this.expanded = false;
            }
           
        }

        void storyboard_Completed(object sender, EventArgs e)
        {            
            this.storyboard.Completed -= new EventHandler(storyboard_Completed);
            this.animating = false;
            Debug.WriteLine("storyboard_Completed");

        }

        private Storyboard GetDropDownAnimation(double from, double to)
        {
            CubicEase ease = new CubicEase() { EasingMode = EasingMode.EaseInOut };
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            animation.From = from;
            animation.To = to;
            animation.FillBehavior = FillBehavior.HoldEnd;
            animation.EasingFunction = ease;
         
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(ListPicker.Height)"));
            
            Storyboard sb = new Storyboard();
            sb.Children.Add(animation);
            return sb;
        }

        #endregion
    }
}
