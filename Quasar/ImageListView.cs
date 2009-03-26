/**************************************************************
 * Copyright (c) 2008 Charlie Robbins
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
**************************************************************/

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
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace Quasar
{
    public class ImageListView : ListBox
    {
		#region Dependency Properties 

        public static readonly DependencyProperty DisplayImagePathProperty = DependencyProperty.Register(
            "DisplayImagePath",
            typeof(string),
            typeof(ImageListView),
            new PropertyMetadata(string.Empty, OnDisplayImagePathChanged));

        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register(
            "ItemHeight",
            typeof(double),
            typeof(ImageListView),
            new PropertyMetadata(100.0, OnItemHeightChanged));

        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register(
            "ItemWidth",
            typeof(double),
            typeof(ImageListView),
            new PropertyMetadata(100.0, OnItemWidthChanged));

        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(
            "Stretch",
            typeof(Stretch),
            typeof(ImageListView),
            new PropertyMetadata(Stretch.UniformToFill, OnStretchChanged));

		#endregion Dependency Properties 

        public ImageListView()
        {
            this.ItemContainerManager = new ItemContainerManager(this);
        }




		#region Properties 

        public string DisplayImagePath
        {
            get { return (string)GetValue(DisplayImagePathProperty); }
            set { SetValue(DisplayImagePathProperty, value); }
        }

        public ItemContainerManager ItemContainerManager { get; private set; }

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

		#endregion Properties 


		#region Methods 


        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            ImageListViewItem itemContainer = element as ImageListViewItem;
            if (itemContainer != null)
            {
                itemContainer.ClearValue(ImageListViewItem.DisplayImagePathProperty);
                itemContainer.ClearValue(ImageListViewItem.StretchProperty);
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            ImageListViewItem item = new ImageListViewItem();
            if (this.ItemContainerStyle != null)
            {
                item.Style = this.ItemContainerStyle;
            }
            return item;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ImageListViewItem;
        }

        protected virtual void OnDisplayImagePathChanged(string oldValue, string newValue)
        {
            ReadOnlyCollection<DependencyObject> itemContainers = this.ItemContainerManager.GetContainerList();
            if (itemContainers.Count > 0)
            {
                foreach (object itemContainer in itemContainers)
                {
                    ImageListViewItem imageListViewItem = itemContainer as ImageListViewItem;
                    imageListViewItem.DisplayImagePath = newValue;
                }
            }
        }

        protected virtual void OnItemHeightChanged(double oldValue, double newValue)
        {
            ReadOnlyCollection<DependencyObject> itemContainers = this.ItemContainerManager.GetContainerList();
            if (itemContainers.Count > 0)
            {
                foreach (object itemContainer in itemContainers)
                {
                    ImageListViewItem imageListViewItem = itemContainer as ImageListViewItem;
                    imageListViewItem.Height = newValue;
                }
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs args)
        {
            base.OnItemsChanged(args);

            this.ItemContainerManager.OnItemsChanged(args);
        }

        protected virtual void OnItemWidthChanged(double oldValue, double newValue)
        {
            ReadOnlyCollection<DependencyObject> itemContainers = this.ItemContainerManager.GetContainerList();
            if (itemContainers.Count > 0)
            {
                foreach (object itemContainer in itemContainers)
                {
                    ImageListViewItem imageListViewItem = itemContainer as ImageListViewItem;
                    imageListViewItem.Width = newValue;
                }
            }
        }

        protected virtual void OnStretchChanged(Stretch oldValue, Stretch newValue)
        {
            ReadOnlyCollection<DependencyObject> itemContainers = this.ItemContainerManager.GetContainerList();
            if (itemContainers.Count > 0)
            {
                foreach (object itemContainer in itemContainers)
                {
                    ImageListViewItem imageListViewItem = itemContainer as ImageListViewItem;
                    imageListViewItem.Stretch = newValue;
                }
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            ImageListViewItem itemContainer = element as ImageListViewItem;
            if (itemContainer != null)
            {
                itemContainer.DisplayImagePath = this.DisplayImagePath;
                itemContainer.Stretch = this.Stretch;
                itemContainer.Width = this.ItemWidth;
                itemContainer.Height = this.ItemHeight;
                itemContainer.HorizontalContentAlignment = this.HorizontalContentAlignment;
            }

            // Add the container and the item to the ItemContainerManager
            this.ItemContainerManager.AddContainer(element, item);
        }


        private static void OnDisplayImagePathChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ImageListView imageViewer = obj as ImageListView;
            if (imageViewer != null)
            {
                imageViewer.OnDisplayImagePathChanged(args.OldValue as string, args.NewValue as string);
            }
        }

        private static void OnItemHeightChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ImageListView imageListView = obj as ImageListView;
            if (imageListView != null)
            {
                imageListView.OnItemHeightChanged((double)args.OldValue, (double)args.NewValue);
            }
        }

        private static void OnItemWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ImageListView imageListView = obj as ImageListView;
            if (imageListView != null)
            {
                imageListView.OnItemWidthChanged((double)args.OldValue, (double)args.NewValue);
            }
        }

        private static void OnStretchChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ImageListView imageListView = obj as ImageListView;
            if (imageListView != null)
            {
                imageListView.OnStretchChanged((Stretch)args.OldValue, (Stretch)args.NewValue);
            }
        }


		#endregion Methods 
    }
}
