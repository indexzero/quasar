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
using System.Windows.Controls.Primitives;
using System.Collections.Generic;

namespace Quasar.DragDrop
{
    public static class AttachedDragDropBehavior
    {
        #region Public Attached Properties

        #region IsEnabled Property

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(AttachedDragDropBehavior),
            new PropertyMetadata(false, OnIsEnabledChanged));

        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }

        private static void OnIsEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            UIElement dragSource = obj as UIElement;

            bool wasEnabled = (args.OldValue != null) ? (bool)args.OldValue : false;
            bool isEnabled = (args.NewValue != null) ? (bool)args.NewValue : false;

            // If the behavior is being disabled, remove the event handler
            if (wasEnabled && !isEnabled)
            {
                dragSource.MouseLeftButtonDown -= OnDragStart;
            }

            // If the behavior is being attached, add the event handler
            if (!wasEnabled && isEnabled)
            {
                dragSource.MouseLeftButtonDown += OnDragStart;
            }
        }

        private static void OnDragStart(object sender, MouseButtonEventArgs args)
        {
            UIElement dragSource = sender as UIElement;
            DragStartedEventHandler dragStarted = AttachedDragDropBehavior.GetBeginDragHandler(dragSource);

            // If the dragSource doesn't have a custom handler for BeginDrag, use the default behavior
            if (dragStarted != null)
            {
                Point positionInSource = args.GetPosition(dragSource);

                // Call the custom handler for BeginDrag
                dragStarted(dragSource, new DragStartedEventArgs(positionInSource.X, positionInSource.Y));
            }
            else
            {
                // Create the TranslateTransform that will perform our drag operation
                TranslateTransform dragTransform = new TranslateTransform();
                dragTransform.X = 0;
                dragTransform.Y = 0;

                // Set the TranslateTransform if it is the first time DragDrop is being used
                dragSource.RenderTransform = (dragSource.RenderTransform is TranslateTransform) ? dragSource.RenderTransform : dragTransform;

                Canvas.SetZIndex(dragSource, 100);
            }

            // Attach the event handlers for MouseMove and MouseLeftButtonUp for dragging and dropping respectively
            dragSource.MouseMove += OnDragDelta;
            dragSource.MouseLeftButtonUp += OnDragCompleted;

            //Capture the Mouse
            dragSource.CaptureMouse();
        }

        private static void OnDragDelta(object sender, MouseEventArgs args)
        {
            FrameworkElement dragSource = sender as FrameworkElement;

            DragDeltaEventHandler dragDelta = AttachedDragDropBehavior.GetDragDeltaHandler(dragSource);

            // Calculate the offset of the dragSource and update its TranslateTransform
            FrameworkElement dragDropHost = FindDragDropHost(dragSource);
            Point relativeLocationInHost = args.GetPosition(dragDropHost);
            Point relativeLocationInSource = args.GetPosition(dragSource);

            double xPosition = GetX(dragSource);
            double yPosition = GetY(dragSource);

            double xChange = relativeLocationInHost.X - xPosition;
            double yChange = relativeLocationInHost.Y - yPosition;

            // Update the TotalX and TotalY for DragCompletedEventArgs
            double totalX = GetTotalX(dragSource);
            totalX = double.IsNaN(totalX) ? xChange : totalX + xChange;

            double totalY = GetTotalY(dragSource);
            totalY = double.IsNaN(totalY) ? yChange : totalY + yChange;

            // If the dragSource doesn't have a custom handler for Drag, use the default behavior
            if (dragDelta != null)
            {
                // Call the custom handler for Drag
                object dragData = AttachedDragDropBehavior.GetDragData(dragSource);
                //DragDropEffects effects = AttachedDragDropBehavior.GetDragDropEffects(dragSource);
                dragDelta(dragSource, new DragDeltaEventArgs(xChange,yChange));
            }
            else
            {
                bool restrictToParentBounds = GetRestrictToParentBounds(dragSource);

                double xLeftBound = relativeLocationInHost.X - relativeLocationInSource.X;
                double xRightBound = (relativeLocationInHost.X - relativeLocationInSource.X) + dragSource.ActualWidth;
                double yTopBound = relativeLocationInHost.Y - relativeLocationInSource.Y;
                double yBottomBound = (relativeLocationInHost.Y - relativeLocationInSource.Y) + dragSource.ActualHeight;

                bool inLeftBound = xLeftBound >= 0;
                bool inRightBound = xRightBound <= dragDropHost.ActualWidth;
                bool inTopBound = yTopBound >= 0;
                bool inBottomBound = yBottomBound <= dragDropHost.ActualHeight;   

                bool updatedX = false;
                if ((inLeftBound && inRightBound) || (!inLeftBound && xChange >= 0)
                    || (!inRightBound && xChange <= 0) || !restrictToParentBounds)
                {
                    if(!double.IsNaN(xPosition))
                    {
                        ((TranslateTransform)dragSource.RenderTransform).X += xChange;
                        updatedX = true;
                    }
                }

                // If we're in bounds or if we have not yet set the X position
                if (updatedX || double.IsNaN(xPosition))
                {
                    SetX(dragSource, relativeLocationInHost.X);
                }

                bool updatedY = false;
                if ((inTopBound && inBottomBound) || (!inTopBound && yChange >= 0) 
                    || (!inBottomBound && yChange <= 0) || !restrictToParentBounds)
                {
                    if (!double.IsNaN(yPosition))
                    {
                        ((TranslateTransform)dragSource.RenderTransform).Y += yChange;
                        updatedY = true;
                    }
                }

                if (updatedY || double.IsNaN(yPosition))
                {
                    SetY(dragSource, relativeLocationInHost.Y);
                }
            }

            // Execute the Drag Enter or Drag Leave operations of the first element found under the current mouse position
            Point globalLocation = args.GetPosition(null);
            IEnumerable<UIElement> uiChildren = VisualTreeHelper.FindElementsInHostCoordinates(globalLocation, Application.Current.RootVisual);

            UIElement dragTarget = FindFirstDragDropTarget(uiChildren);
            if (dragTarget != null)
            {
                object dragData = GetDragData(dragSource);
                DragDropEffects effects = GetDragDropEffects(dragSource);

                UIElement oldDragTarget = GetItemUnderDrag(dragSource);
                if (oldDragTarget != null && !object.ReferenceEquals(oldDragTarget, dragTarget))
                {
                    DragEventHandler dragLeave = GetDragLeaveHandler(oldDragTarget);
                    if (dragLeave != null)
                    {
                        dragLeave(oldDragTarget, new DragEventArgs(dragSource, dragData, effects, Keyboard.Modifiers, args));
                    }
                }

                DragEventHandler dragEnter = GetDragEnterHandler(dragTarget);
                if (dragEnter != null)
                {
                    dragEnter(dragTarget, new DragEventArgs(dragSource, dragData, effects, Keyboard.Modifiers, args));
                }    
            }

            SetItemUnderDag(dragSource, dragTarget);
        }

        private static void OnDragCompleted(object sender, MouseButtonEventArgs args)
        {
            UIElement dragSource = sender as UIElement;

            DragCompletedEventHandler dragCompleted = AttachedDragDropBehavior.GetDragCompletedHandler(dragSource);

            double totalX = GetTotalX(dragSource);
            double totalY = GetTotalY(dragSource);

            // If the dragSource doesn't have a custom handler for DragCompleted, use the default behavior
            if (dragCompleted != null)
            {
                // Call the custom handler for Drop
                dragCompleted(dragSource, new DragCompletedEventArgs(totalX, totalY, false));
            }
            else
            {
                Canvas.SetZIndex(dragSource, 0);
            }

            // Remove the event handlers for MouseMove and MouseLeftButtonUp, for dragging and dropping respectively
            dragSource.MouseMove -= OnDragDelta;
            dragSource.MouseLeftButtonUp -= OnDragCompleted;

            // Set the X & Y Values so that they can be reset next MouseDown
            SetX(dragSource, double.NaN);
            SetY(dragSource, double.NaN);

            // Set the TotalX & TotalY Values so that they can be reset next MouseDown
            SetTotalX(dragSource, double.NaN);
            SetTotalY(dragSource, double.NaN);

            // Execute the Drop operation of the first element found under the current mouse position
            Point globalLocation = args.GetPosition(null);
            IEnumerable<UIElement> uiChildren = VisualTreeHelper.FindElementsInHostCoordinates(globalLocation, Application.Current.RootVisual);

            UIElement dropTarget = FindFirstDragDropTarget(uiChildren);
            if (dropTarget != null)
            {
                object dragData = GetDragData(dragSource);
                DragDropEffects effects = GetDragDropEffects(dragSource);

                DragEventHandler drop = GetDropHandler(dropTarget);
                if (drop != null)
                {
                    drop(dropTarget, new DragEventArgs(dragSource, dragData, effects, Keyboard.Modifiers, args));
                }
            }

            // Reset the ItemUnderDrag
            SetItemUnderDag(dragSource, null);

            // Release Mouse Capture
            dragSource.ReleaseMouseCapture();
        }

        public static FrameworkElement FindDragDropHost(UIElement element)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            while (parent != null && !AttachedDragDropBehavior.GetIsHost(parent))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as FrameworkElement;
        }

        private static UIElement FindFirstDragDropTarget(IEnumerable<UIElement> elements)
        {
            foreach (UIElement element in elements)
            {
                if (AttachedDragDropBehavior.GetIsEnabled(element))
                    return element;
            }
            return null;
        }

        #endregion

        #region IsHost Property

        public static readonly DependencyProperty IsHostProperty = DependencyProperty.RegisterAttached(
            "IsHost",
            typeof(bool),
            typeof(AttachedDragDropBehavior),
            new PropertyMetadata(false));

        public static bool GetIsHost(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsHostProperty); 
        }

        public static void SetIsHost(DependencyObject obj, bool value)
        {
            obj.SetValue(IsHostProperty, value);
        }

        #endregion

        #region RestrictToParentBounds Property

        public static readonly DependencyProperty RestrictToParentBoundsProperty = DependencyProperty.RegisterAttached(
            "RestrictToParentBounds",
            typeof(bool),
            typeof(AttachedDragDropBehavior),
            new PropertyMetadata(false));

        public static bool GetRestrictToParentBounds(DependencyObject obj)
        {
            return (bool)obj.GetValue(RestrictToParentBoundsProperty);
        }

        public static void SetRestrictToParentBounds(DependencyObject obj, bool value)
        {
            obj.SetValue(RestrictToParentBoundsProperty, value); 
        }

        #endregion

        #region X Property

        public static readonly DependencyProperty XProperty = DependencyProperty.RegisterAttached(
            "X",
            typeof(double),
            typeof(AttachedDragDropBehavior),
            new PropertyMetadata(double.NaN));

        public static double GetX(DependencyObject obj)
        {
            return (double)obj.GetValue(XProperty);
        }

        public static void SetX(DependencyObject obj, double value)
        {
            obj.SetValue(XProperty, value);
        }

        #endregion

        #region Y Property

        public static readonly DependencyProperty YProperty = DependencyProperty.RegisterAttached(
            "Y",
            typeof(double),
            typeof(AttachedDragDropBehavior),
            new PropertyMetadata(double.NaN));

        public static double GetY(DependencyObject obj)
        {
            return (double)obj.GetValue(YProperty);
        }

        public static void SetY(DependencyObject obj, double value)
        {
            obj.SetValue(YProperty, value);
        }

        #endregion

        #region DragData Property

        public static readonly DependencyProperty DragDataProperty = DependencyProperty.RegisterAttached(
            "DragData",
            typeof(object),
            typeof(AttachedDragDropBehavior),
            new PropertyMetadata(null));

        public static object GetDragData(DependencyObject obj)
        {
            return obj.GetValue(DragDataProperty);
        }

        public static void SetDragData(DependencyObject obj, object value)
        {
            obj.SetValue(DragDataProperty, value);
        }
        
        #endregion

        // Remark: DragDropEffect Not Consumed in Default DragDrop Behavior (v. 0.5)
        #region DragDropEffects Property

        public static readonly DependencyProperty DragDropEffectsProperty = DependencyProperty.RegisterAttached(
            "DragDropEffects",
            typeof(DragDropEffects),
            typeof(AttachedDragDropBehavior),
            new PropertyMetadata(DragDropEffects.None));

        public static DragDropEffects GetDragDropEffects(DependencyObject obj)
        {
            return (DragDropEffects)obj.GetValue(DragDropEffectsProperty);
        }

        public static void SetDragDropEffects(DependencyObject obj, DragDropEffects value)
        {
            obj.SetValue(DragDropEffectsProperty, value);
        }

        #endregion

        #region DragStart Property (Attached EventHandler in XAML)

        public static readonly DependencyProperty DragStartProperty = DependencyProperty.RegisterAttached(
            "DragStart",
            typeof(string),
            typeof(AttachedDragDropBehavior),
            new PropertyMetadata(null));

        public static string GetDragStart(DependencyObject obj)
        {
            return (string)obj.GetValue(DragStartProperty);
        }

        public static void SetDragStart(DependencyObject obj, string value)
        {
            // Set the Attached Property
            obj.SetValue(DragStartProperty, value);

            //Set the Attached EventHandler
            AttachedEventHandlerManager.AttachEventHandler(obj, value, new Type[] { typeof(object), typeof(DragStartedEventArgs) }, (sender, target, info) =>
            {
                FrameworkElement senderElement = sender as FrameworkElement;
                AttachedDragDropBehavior.SetBeginDragHandler(senderElement, (newSender, args) =>
                {
                    info.Invoke(target, new object[] { newSender, args });
                });
            });
        }

        #endregion

        #region DragDelta Property (Attached EventHandler in XAML)

        public static readonly DependencyProperty DragDeltaProperty = DependencyProperty.RegisterAttached(
            "DragDelta",
            typeof(string),
            typeof(AttachedDragDropBehavior),
            new PropertyMetadata(null));

        public static string GetDragDelta(DependencyObject obj)
        {
            return (string)obj.GetValue(DragDeltaProperty);
        }

        public static void SetDragDelta(DependencyObject obj, string value)
        {
            // Set the Attached Property
            obj.SetValue(DragDeltaProperty, value);

            // Set the Attached EventHandler
            AttachedEventHandlerManager.AttachEventHandler(obj, value, new Type[] { typeof(object), typeof(DragDeltaEventArgs) }, (sender, target, info) =>
            {
                FrameworkElement senderElement = sender as FrameworkElement;
                AttachedDragDropBehavior.SetDragDeltaHandler(senderElement, (newSender, args) =>
                {
                    info.Invoke(target, new object[] { newSender, args });
                });
            });
        }

        #endregion

        #region DragEnter Property (Attached EventHandler in XAML)

        public static readonly DependencyProperty DragEnterProperty = DependencyProperty.RegisterAttached(
            "DragEnter",
            typeof(string),
            typeof(AttachedDragDropBehavior),
            new PropertyMetadata(null));

        public static string GetDragEnter(DependencyObject obj)
        {
            return (string)obj.GetValue(DragEnterProperty);
        }

        public static void SetDragEnter(DependencyObject obj, string value)
        {
            // Set the Attached Property
            obj.SetValue(DragEnterProperty, value);

            // Set the Attached EventHandler
            AttachedEventHandlerManager.AttachEventHandler(obj, value, new Type[] { typeof(object), typeof(DragEventArgs) }, (sender, target, info) =>
            {
                FrameworkElement senderElement = sender as FrameworkElement;
                AttachedDragDropBehavior.SetDragEnterHandler(senderElement, (newSender, args) =>
                {
                    info.Invoke(target, new object[] { newSender, args });
                });
            });
        }

        #endregion

        #region DragLeave Property (Attached EventHandler in XAML)

        public static readonly DependencyProperty DragLeaveProperty = DependencyProperty.RegisterAttached(
            "DragLeave",
            typeof(string),
            typeof(AttachedDragDropBehavior),
            new PropertyMetadata(null));

        public static string GetDragLeave(DependencyObject obj)
        {
            return (string)obj.GetValue(DragLeaveProperty);
        }

        public static void SetDragLeave(DependencyObject obj, string value)
        {
            // Set the Attached Property
            obj.SetValue(DragLeaveProperty, value);

            // Set the Attached EventHandler
            AttachedEventHandlerManager.AttachEventHandler(obj, value, new Type[] { typeof(object), typeof(DragEventArgs) }, (sender, target, info) =>
            {
                FrameworkElement senderElement = sender as FrameworkElement;
                AttachedDragDropBehavior.SetDragLeaveHandler(senderElement, (newSender, args) =>
                {
                    info.Invoke(target, new object[] { newSender, args });
                });
            });
        }

        #endregion

        #region DragCompleted Property (Attached EventHandler in XAML)

        public static readonly DependencyProperty DragCompletedProperty = DependencyProperty.RegisterAttached(
            "DragCompleted",
            typeof(string),
            typeof(AttachedDragDropBehavior),
            new PropertyMetadata(null));

        public static string GetDragCompleted(DependencyObject obj)
        {
            return (string)obj.GetValue(DragCompletedProperty);
        }

        public static void SetDragCompleted(DependencyObject obj, string value)
        {
            // Set the Attached Property
            obj.SetValue(DragCompletedProperty, value);

            //Set the Attached EventHandler
            AttachedEventHandlerManager.AttachEventHandler(obj, value, new Type[] { typeof(object), typeof(DragCompletedEventArgs) }, (sender, target, info) =>
            {
                FrameworkElement senderElement = sender as FrameworkElement;
                AttachedDragDropBehavior.SetDragCompletedHandler(senderElement, (newSender, args) =>
                {
                    info.Invoke(target, new object[] { newSender, args });
                });
            });
        }

        #endregion

        #region Drop Property (Attached EventHandler in XAML)

        public static readonly DependencyProperty DropProperty = DependencyProperty.RegisterAttached(
            "Drop",
            typeof(string),
            typeof(AttachedDragDropBehavior),
            new PropertyMetadata(null));

        public static string GetDrop(DependencyObject obj)
        {
            return (string)obj.GetValue(DropProperty);
        }

        public static void SetDrop(DependencyObject obj, string value)
        {
            // Set the Attached Property
            obj.SetValue(DropProperty, value);

            //Set the Attached EventHandler
            AttachedEventHandlerManager.AttachEventHandler(obj, value, new Type[] { typeof(object), typeof(DragEventArgs) }, (sender, target, info) =>
            {
                FrameworkElement senderElement = sender as FrameworkElement;
                AttachedDragDropBehavior.SetDropHandler(senderElement, (newSender, args) =>
                {
                    info.Invoke(target, new object[] { newSender, args });
                });
            });
        }

        #endregion

        #endregion

        #region Private Attached Properties

        #region DragStartHandler Property (Attached EventHandler in Code)

        private static readonly DependencyProperty DragStartHandlerProperty = DependencyProperty.RegisterAttached(
            "BeginDragHandler",
            typeof(DragStartedEventHandler),
            typeof(AttachedDragDropBehavior),
            new PropertyMetadata(null));

        public static DragStartedEventHandler GetBeginDragHandler(DependencyObject obj)
        {
            return (DragStartedEventHandler)obj.GetValue(DragStartHandlerProperty);
        }

        // Remark: To maintain parity between XAML and procedural code, this method should never be called directly
        // outside the context of AttachedDragDropBehavior.SetBeginDrag
        private static void SetBeginDragHandler(DependencyObject obj, DragStartedEventHandler value)
        {
            obj.SetValue(DragStartHandlerProperty, value);
        }

        #endregion

        #region DragDeltaHandler Property (Attached EventHandler in Code)

        private static readonly DependencyProperty DragDeltaHandlerProperty = DependencyProperty.RegisterAttached(
            "DragDeltaHandler",
            typeof(DragDeltaEventHandler),
            typeof(AttachedDragDropBehavior),
            new PropertyMetadata(null));

        public static DragDeltaEventHandler GetDragDeltaHandler(DependencyObject obj)
        {
            return (DragDeltaEventHandler)obj.GetValue(DragDeltaHandlerProperty);
        }

        // Remark: To maintain parity between XAML and procedural code, this method should never be called directly
        // outside the context of AttachedDragDropBehavior.SetDragDelta
        private static void SetDragDeltaHandler(DependencyObject obj, DragDeltaEventHandler value)
        {
            obj.SetValue(DragDeltaHandlerProperty, value);
        }

        #endregion

        #region DropHandler Property (Attached EventHandler in Code)

        private static readonly DependencyProperty DragCompletedHandlerProperty = DependencyProperty.RegisterAttached(
            "DragCompletedHandler",
            typeof(DragCompletedEventHandler),
            typeof(AttachedDragDropBehavior),
            new PropertyMetadata(null));

        public static DragCompletedEventHandler GetDragCompletedHandler(DependencyObject obj)
        {
            return (DragCompletedEventHandler)obj.GetValue(DragCompletedHandlerProperty);
        }

        // Remark: To maintain parity between XAML and procedural code, this method should never be called directly
        // outside the context of AttachedDragDropBehavior.SetDragCompleted
        private static void SetDragCompletedHandler(DependencyObject obj, DragCompletedEventHandler value)
        {
            obj.SetValue(DragCompletedHandlerProperty, value);
        }

        #endregion

        #region DragEnterHandler Property (Attached EventHandler in Code)

        private static readonly DependencyProperty DragEnterHandlerProperty = DependencyProperty.RegisterAttached(
            "DragEnterHandler",
            typeof(DragEventHandler),
            typeof(AttachedDragDropBehavior),
            new PropertyMetadata(null));

        public static DragEventHandler GetDragEnterHandler(DependencyObject obj)
        {
            return (DragEventHandler)obj.GetValue(DragEnterHandlerProperty);
        }

        // Remark: To maintain parity between XAML and procedural code, this method should never be called directly
        // outside the context of AttachedDragDropBehavior.SetDragEnter
        private static void SetDragEnterHandler(DependencyObject obj, DragEventHandler value)
        {
            obj.SetValue(DragEnterHandlerProperty, value);
        }

        #endregion

        #region DragLeaveHandler Property (Attached EventHandler in Code)

        private static readonly DependencyProperty DragLeaveHandlerProperty = DependencyProperty.RegisterAttached(
            "DragLeaveHandler",
            typeof(DragEventHandler),
            typeof(AttachedDragDropBehavior),
            new PropertyMetadata(null));

        public static DragEventHandler GetDragLeaveHandler(DependencyObject obj)
        {
            return (DragEventHandler)obj.GetValue(DragLeaveHandlerProperty);
        }

        // Remark: To maintain parity between XAML and procedural code, this method should never be called directly
        // outside the context of AttachedDragDropBehavior.SetDragLeave
        private static void SetDragLeaveHandler(DependencyObject obj, DragEventHandler value)
        {
            obj.SetValue(DragLeaveHandlerProperty, value);
        }

        #endregion

        #region DropHandler Property (Attached EventHandler in Code)

        private static readonly DependencyProperty DropHandlerProperty = DependencyProperty.RegisterAttached(
            "DropHandler",
            typeof(DragEventHandler),
            typeof(AttachedDragDropBehavior),
            new PropertyMetadata(null));

        public static DragEventHandler GetDropHandler(DependencyObject obj)
        {
            return (DragEventHandler)obj.GetValue(DropHandlerProperty);
        }

        // Remark: To maintain parity between XAML and procedural code, this method should never be called directly
        // outside the context of AttachedDragDropBehavior.SetDrop
        private static void SetDropHandler(DependencyObject obj, DragEventHandler value)
        {
            obj.SetValue(DropHandlerProperty, value);
        }

        #endregion

        #region ItemUnderDrag Property

        private static readonly DependencyProperty ItemUnderDragProperty = DependencyProperty.RegisterAttached(
            "ItemUnderDrag",
            typeof(UIElement),
            typeof(AttachedDragDropBehavior),
            new PropertyMetadata(null));

        private static UIElement GetItemUnderDrag(DependencyObject obj)
        {
            return (UIElement)obj.GetValue(ItemUnderDragProperty);
        }

        private static void SetItemUnderDag(DependencyObject obj, UIElement value)
        {
            obj.SetValue(ItemUnderDragProperty, value); 
        }

        #endregion

        #region TotalX Property

        private static readonly DependencyProperty TotalXProperty = DependencyProperty.RegisterAttached(
            "TotalX",
            typeof(double),
            typeof(AttachedDragDropBehavior),
            new PropertyMetadata(double.NaN));

        private static double GetTotalX(DependencyObject obj)
        {
            return (double)obj.GetValue(TotalXProperty);
        }

        private static void SetTotalX(DependencyObject obj, double value)
        {
            obj.SetValue(TotalXProperty, value); 
        }

        #endregion

        #region TotalY Property

        private static readonly DependencyProperty TotalYProperty = DependencyProperty.RegisterAttached(
            "TotalY",
            typeof(double),
            typeof(AttachedDragDropBehavior),
            new PropertyMetadata(double.NaN));

        private static double GetTotalY(DependencyObject obj)
        {
            return (double)obj.GetValue(TotalYProperty);
        }

        private static void SetTotalY(DependencyObject obj, double value)
        {
            obj.SetValue(TotalYProperty, value);
        }

        #endregion

        #endregion

        // Remark: These are not consumed by the default DragDrop Behavior (v. 0.5)
        //internal static bool IsValidDragDropEffects(DragDropEffects dragDropEffects)
        //{
        //    int num = -2147483641;
        //    if ((dragDropEffects & ~num) != DragDropEffects.None)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        //internal static bool IsValidDragDropKeyStates(DragDropKeyStates dragDropKeyStates)
        //{
        //    int num = 0x3f;
        //    if ((dragDropKeyStates & ~num) != DragDropKeyStates.None)
        //    {
        //        return false;
        //    }
        //    return true;
        //}
    }
}
