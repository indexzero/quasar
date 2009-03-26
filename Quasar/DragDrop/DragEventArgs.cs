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

namespace Quasar.DragDrop
{
    public class DragEventArgs : EventArgs
    {
        #region DragEventArgs Members

        private UIElement _dragSource;
        public UIElement DragSource
        {
            get { return _dragSource; }
            private set { _dragSource = value; }
        }

        private DragDropEffects _allowedEffects;
        public DragDropEffects AllowedEffects 
        {
            get { return _allowedEffects; }
            private set { _allowedEffects = value; }
        }

        private object _data;
        public object Data 
        {
            get { return _data; }
            private set { _data = value; }
        }

        public DragDropEffects Effects { get; set; }

        private ModifierKeys _keyStates;
        public ModifierKeys KeyStates 
        {
            get { return _keyStates; }
            private set { _keyStates = value; }
        }

        private MouseEventArgs _mouseArgs;
        private MouseEventArgs MouseArgs
        {
            get { return _mouseArgs; }
            set { _mouseArgs = value; }
        }

        public object OriginalSource
        {
            get { return (_mouseArgs.OriginalSource != null) ? _mouseArgs.OriginalSource : null; }
        }

        public Point GetPosition(UIElement relativeTo)
        {
            return (_mouseArgs != null) ? _mouseArgs.GetPosition(relativeTo) : new Point(double.NaN, double.NaN);
        }

        #endregion

        public DragEventArgs(UIElement dragSource, object data, DragDropEffects allowedEffects, ModifierKeys keyStates, MouseEventArgs mouseArgs)
            : base()
        {
            // Remark: These should work, but the compiler is complaining at the moment...
            //AttachedDrapDropBehavior.IsValidDragDropEffects(allowedEffects);
            //AttachedDrapDropBehavior.IsValidDragDropKeyStates(allowedEffects);

            this.DragSource = dragSource;
            this.Data = data;
            this.AllowedEffects = allowedEffects;
            this.Effects = allowedEffects;
            this.KeyStates = keyStates;
            this.MouseArgs = mouseArgs;
        }
    }
}
