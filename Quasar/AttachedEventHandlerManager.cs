/**************************************************************
 * Copyright (c) 2008 Ary Borenszweig, Charlie Robbins
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
using System.Reflection;

namespace Quasar
{
    /// <summary>
    /// Indicates how to attach an event to the given target.
    /// </summary>
    /// <param name="sender">the object that fired the event</param>
    /// <param name="target">the object to which an event must be attached</param>
    /// <param name="method">the method to invoke in the event handler</param>
    public delegate void EventAttacher(object sender, DependencyObject target, MethodInfo method);

    /// <summary>
    /// Support for attaching to an event based on the name of a method.
    /// </summary>
    public static class AttachedEventHandlerManager
    {
        /// <summary>
        /// Attaches an event to the first Parent of obj (which must be a 
        /// FrameworkElement) which declares a public method with the name 
        /// "handler" and arguments of types "types".
        /// 
        /// When that method is found, attacher is invoked with:
        /// - sender is "obj"
        /// - target is the Parent that declares the method
        /// - method is the method of Parent
        /// </summary>
        /// <param name="obj">the object to which to attach an event</param>
        /// <param name="handler">the name of the method to be found in some Parent of obj</param>
        /// <param name="attacher">indicates how to attach an event to the target</param>
        public static void AttachEventHandler(DependencyObject obj, string methodName, Type[] handlerTypes, EventAttacher attacher)
        {
            FrameworkElement element = obj as FrameworkElement;
            if (element == null)
                throw new ArgumentException("Can only attach events to FrameworkElement instances, not to '" + obj.GetType() + "'");

            element.Loaded += (sender, e) =>
            {
                DependencyObject parent = sender as DependencyObject;

                MethodInfo info = null;
                while (info == null)
                {
                    info = parent.GetType().GetMethod(methodName, handlerTypes);
                    if (info != null)
                    {
                        attacher(sender, parent, info);
                        return;
                    }

                    parent = (parent is FrameworkElement) ? ((FrameworkElement)parent).Parent : null;

                    if (parent == null)
                        throw new ArgumentException("Can't find handler '" + methodName + "' (maybe it's not public, or set in a ControlTemplate?)");
                }
            };
        }

    }
}
