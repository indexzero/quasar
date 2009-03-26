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

namespace Quasar.Workarounds
{
    public class CanvasListBoxItem : ListBoxItem
    {
        #region Dependency Properties

        public static readonly DependencyProperty TopPathProperty = DependencyProperty.Register(
            "Top",
            typeof(string),
            typeof(CanvasListBoxItem),
            new PropertyMetadata(0));

        public string TopPath
        {
            get { return (string)GetValue(TopPathProperty); }
            set { SetValue(TopPathProperty, value); }
        }

        public static readonly DependencyProperty LeftPathProperty = DependencyProperty.Register(
            "Left",
            typeof(string),
            typeof(CanvasListBoxItem),
            new PropertyMetadata(0));

        public string LeftPath
        {
            get { return (string)GetValue(LeftPathProperty); }
            set { SetValue(LeftPathProperty, value); }
        }

        #endregion

        public CanvasListBoxItem()
        {

        }
    }
}
