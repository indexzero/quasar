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

namespace Quasar
{
    [TemplateVisualState(GroupName = "LightBoxStates", Name = "Opened")]
    [TemplateVisualState(GroupName = "LightBoxStates", Name = "Closed")]
    public class LightBox : ContentControl
    {
        #region Dependency Properties

        public static readonly DependencyProperty CloseButtonStyleProperty = DependencyProperty.Register(
            "CloseButtonStyle",
            typeof(Style),
            typeof(LightBox),
            new PropertyMetadata(null));

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius",
            typeof(CornerRadius),
            typeof(LightBox),
            new PropertyMetadata(new CornerRadius(4)));

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header",
            typeof(object),
            typeof(LightBox),
            new PropertyMetadata(null));

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
            "HeaderTemplate",
            typeof(object),
            typeof(LightBox),
            new PropertyMetadata(null));

        public static readonly DependencyProperty IsOpenedProperty = DependencyProperty.Register(
            "IsOpened",
            typeof(bool),
            typeof(LightBox),
            new PropertyMetadata(false, OnIsOpenedChanged));

        public static readonly DependencyProperty LightBoxHeightProperty = DependencyProperty.Register(
            "LightBoxHeight",
            typeof(double),
            typeof(LightBox),
            new PropertyMetadata(400.0));

        public static readonly DependencyProperty LightBoxWidthProperty = DependencyProperty.Register(
            "LightBoxWidth",
            typeof(double),
            typeof(LightBox),
            new PropertyMetadata(600.0));

        //public static readonly DependencyProperty MinimizeButtonStyleProperty = DependencyProperty.Register(
        //    "MinimizeButtonStyle",
        //    typeof(Style),
        //    typeof(LightBox),
        //    new PropertyMetadata(null, OnMinimizeButtonStyleChanged));

        //public static readonly DependencyProperty MaximizeButtonStyleProperty = DependencyProperty.Register(
        //    "MaximizeButtonStyle",
        //    typeof(Style),
        //    typeof(LightBox),
        //    new PropertyMetadata(null, OnMaximizeButtonStyleChanged));

        public static readonly DependencyProperty ShadowProperty = DependencyProperty.Register(
            "Shadow",
            typeof(Brush),
            typeof(LightBox),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        #endregion

        #region Fields

        protected const string HeaderPresenterName = "HeaderPresenter";
        protected const string ContentPresenterName = "ContentPresenter";
        protected const string CloseButtonName = "CloseButton"; 
        protected const string MinimizeButtonName = "MinimizeButton";
        protected const string MaximizeButonName = "MaximizeButton";

        protected Button closeButton = null;
        //protected Button MinimizeButton = null;
        //protected Button MaximizeButton = null;
        protected ContentPresenter HeaderPresenter = null;
        protected ContentPresenter ContentPresenter = null;

        #endregion

        #region Properties

        public Style CloseButtonStyle
        {
            get { return (Style)GetValue(CloseButtonStyleProperty); }
            set { SetValue(CloseButtonStyleProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public bool IsOpened
        {
            get { return (bool)GetValue(IsOpenedProperty); }
            set { SetValue(IsOpenedProperty, value); }
        }

        public double LightBoxHeight
        {
            get { return (double)GetValue(LightBoxHeightProperty); }
            set { SetValue(LightBoxHeightProperty, value); }
        }

        public double LightBoxWidth
        {
            get { return (double)GetValue(LightBoxWidthProperty); }
            set { SetValue(LightBoxWidthProperty, value); }
        }

        //public Style MinimizeButtonStyle
        //{
        //    get { return (Style)GetValue(MinimizeButtonStyleProperty); }
        //    set { SetValue(MinimizeButtonStyleProperty, value); }
        //}

        //public Style MaximizeButtonStyle
        //{
        //    get { return (Style)GetValue(MaximizeButtonStyleProperty); }
        //    set { SetValue(MaximizeButtonStyleProperty, value); }
        //}

        public Brush Shadow
        {
            get { return (Brush)GetValue(LightBox.ShadowProperty); }
            set { SetValue(ShadowProperty, value); }
        }

        #endregion

        #region Events

        public event EventHandler Opening;
        public event EventHandler Opened;
        public event EventHandler Closing;
        public event EventHandler Closed;

        #endregion

        public LightBox()
        {
            this.DefaultStyleKey = typeof(LightBox);

            this.Opening += OnOpened;
            this.Closing += OnClosed;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Button oldCloseButton = this.closeButton;
            this.closeButton = GetTemplateChild(CloseButtonName) as Button;
            
            if (!object.ReferenceEquals(oldCloseButton, this.closeButton))
            {
                if (oldCloseButton != null)
                {
                    oldCloseButton.Click -= OnClosed;
                }

                if (this.closeButton != null)
                {
                    this.closeButton.Click += OnCloseButtonClicked;
                }
            }

            UpdateVisualState();
        }

        protected void OnClosing()
        {
            EventHandler closing = Closing;
            if (closing != null)
            {
                closing(this, EventArgs.Empty);
            }
        }

        protected void OnIsOpenedChanged(bool oldValue, bool newValue)
        {
            if (oldValue != newValue)
            {
                if (newValue)
                {
                    OnOpening();
                }
                else
                {
                    OnClosing();
                }
            }
        }

        protected void OnOpening()
        {
            EventHandler opening = Opening;

            if (opening != null)
            {
                opening(this, EventArgs.Empty);
            }
        }

        protected void UpdateVisualState()
        {
            string stateName = (this.IsOpened) ? "Opened" : "Closed";

            VisualStateManager.GoToState(this, stateName, true);
        }

        private void OnCloseButtonClicked(object sender, RoutedEventArgs args)
        {
            this.IsOpened = !this.IsOpened;
        }

        private void OnClosed(object sender, EventArgs args)
        {
            UpdateVisualState();

            EventHandler closed = Closed;
            if (closed != null)
            {
                closed(this, EventArgs.Empty);
            }
        }

        private void OnOpened(object sender, EventArgs args)
        {
            UpdateVisualState();

            EventHandler opened = Opened;
            if (opened != null)
            {
                opened(this, EventArgs.Empty);
            }
        }

        private static void OnIsOpenedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            LightBox lightBox = obj as LightBox;
            if (lightBox != null)
            {
                lightBox.OnIsOpenedChanged((bool)args.OldValue, (bool)args.NewValue);
            }
        }
    }
}
