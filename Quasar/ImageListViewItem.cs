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

namespace Quasar
{
    public class ImageListViewItem : ListBoxItem
    {
        #region Dependency Properties

        public static readonly DependencyProperty DisplayImagePathProperty = DependencyProperty.Register(
            "DisplayImagePathProperty",
            typeof(string),
            typeof(ImageListViewItem),
            new PropertyMetadata(string.Empty, OnDisplayImagePathChanged));

        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(
            "Stretch",
            typeof(Stretch),
            typeof(ImageListViewItem),
            new PropertyMetadata(Stretch.UniformToFill));

        #endregion

        #region Fields

        private Image backgroundImage;
        private string BackgroundImageName = "BackgroundImage";

        #endregion

        #region Properties

        public string DisplayImagePath
        {
            get { return (string)GetValue(DisplayImagePathProperty); }
            set { SetValue(DisplayImagePathProperty, value); }
        }

        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        #endregion

        public ImageListViewItem()
        {
            this.DefaultStyleKey = typeof(ImageListViewItem);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Image oldBackgroundImage = backgroundImage;
            backgroundImage = this.GetTemplateChild(BackgroundImageName) as Image;

            UpdateDisplayImagePathBinding(this.DisplayImagePath);
        }

        protected virtual void UpdateDisplayImagePathBinding(string displayImagePath)
        {
            if (!string.IsNullOrEmpty(displayImagePath) && backgroundImage != null)
            {
                Binding displayImagePathBinding = new Binding(displayImagePath);
                displayImagePathBinding.Source = this.Content;

                backgroundImage.SetBinding(Image.SourceProperty, displayImagePathBinding);
            }
        }

        private static void OnDisplayImagePathChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ImageListViewItem item = obj as ImageListViewItem;
            if (item != null)
            {
                item.OnDisplayImagePathChanged(args.OldValue as string, args.NewValue as string);
            }
        }

        protected virtual void OnDisplayImagePathChanged(string oldValue, string newValue)
        {
            if (oldValue != newValue)
            {
                UpdateDisplayImagePathBinding(newValue);
            }
        }
    }
}
