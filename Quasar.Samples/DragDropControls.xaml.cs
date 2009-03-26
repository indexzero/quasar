using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Quasar.DragDrop;
using System.Windows.Controls.Primitives;

namespace Quasar.Samples
{
    public partial class DragDropControls : UserControl
    {
        #region Members

        private DragDropViewModel _viewModel = new DragDropViewModel();
        public DragDropViewModel ViewModel
        {
            get { return _viewModel; }
        }

        #endregion

        public DragDropControls()
        {
            InitializeComponent();

            this.DataContext = this.ViewModel;
        }

        public void CustomCanvasDrag(object sender, DragDeltaEventArgs args)
        {
            UIElement dragSource = sender as UIElement;

            double canvasTop = Canvas.GetTop(dragSource);
            double canvasLeft = Canvas.GetLeft(dragSource);

            canvasLeft += args.HorizontalChange;
            canvasTop += args.VerticalChange;
            
            Canvas.SetLeft(dragSource, canvasLeft);
            Canvas.SetTop(dragSource, canvasTop);
       
            AttachedDragDropBehavior.SetX(dragSource, canvasLeft);
            AttachedDragDropBehavior.SetY(dragSource, canvasTop);
        }

        public void OnDragEnter(object sender, DragEventArgs args)
        {
            int x = 0;
            x++;
        }

        public void OnDragLeave(object sender, DragEventArgs args)
        {
            int x = 0;
            x++;
        }

        public void OnDrop(object sender, DragEventArgs args)
        {
            int x = 0;
            x++;
        }
    }

    public class DragDropViewModel
    {
        #region DragDropViewModel Members

        private Random _randomGenerator = new Random();

        private ObservableCollection<Brush> _brushes = new ObservableCollection<Brush>();
        public ObservableCollection<Brush> Brushes
        {
            get { return _brushes; }
        }

        private ObservableCollection<DragDropSourceData> _dragDropSourceData = new ObservableCollection<DragDropSourceData>();
        public ObservableCollection<DragDropSourceData> DragDropSourceData
        {
            get { return _dragDropSourceData; }
        }

        private Size defaultSize = new Size(30, 30);

        #endregion

        public DragDropViewModel()
        {
            InitializeBrushes();
        }

        private void InitializeBrushes()
        {
            _brushes.Add(new SolidColorBrush(Colors.Black));
            _brushes.Add(new SolidColorBrush(Colors.Brown));
            _brushes.Add(new SolidColorBrush(Colors.Cyan));
            _brushes.Add(new SolidColorBrush(Colors.Gray));
            _brushes.Add(new SolidColorBrush(Colors.Orange));
            _brushes.Add(new SolidColorBrush(Colors.Purple));
            _brushes.Add(new SolidColorBrush(Colors.White));
            _brushes.Add(new SolidColorBrush(Colors.Yellow));
            _brushes.Add(new SolidColorBrush(Colors.Red));
            _brushes.Add(new SolidColorBrush(Colors.LightGray));
            _brushes.Add(new SolidColorBrush(Colors.Green));
            _brushes.Add(new SolidColorBrush(Colors.Blue));
            _brushes.Add(new SolidColorBrush(Colors.Magenta));

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    int backgroundNumber = _randomGenerator.Next(0, _brushes.Count - 1);
                    int borderNumber = _randomGenerator.Next(0, _brushes.Count - 1);
                    _dragDropSourceData.Add(new DragDropSourceData(_brushes[backgroundNumber], _brushes[borderNumber], GetLocation(i,j)));
                }
            }
        }

        private Point GetLocation(int x, int y)
        {
            return new Point((defaultSize.Width * x) + 10, (defaultSize.Height * y) + 10); 
        }
    }

    public class DragDropSourceData : INotifyPropertyChanged
    {
        #region DragDropSourceData Members

        private Brush backgroundBrush;
        public Brush BackgroundBrush
        {
            get { return backgroundBrush; }
            set
            {
                backgroundBrush = value;
                OnNotifyPropertyChanged("BackgroundBrush");
            }
        }

        private Brush borderBrush;
        public Brush BorderBrush
        {
            get { return borderBrush; }
            set
            {
                borderBrush = value;
                OnNotifyPropertyChanged("BorderBrush");
            }
        }

        private Point location;
        public Point Location
        {
            get { return location; }
            set
            {
                location = value;
                OnNotifyPropertyChanged("Location");
                OnNotifyPropertyChanged("X");
                OnNotifyPropertyChanged("Y");
            }
        }

        public double X
        {
            get { return (double)location.X; }
        }

        public double Y
        {
            get { return (double)location.Y; }
        }

        #endregion

        public DragDropSourceData(Brush background, Brush border, Point location)
        {
            this.backgroundBrush = background;
            this.borderBrush = border;
            this.location = location; 
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="E:NotifyPropertyChanged"/> event.
        /// </summary>
        /// <param name="name">The name of the property that was changed.</param>
        /// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
        protected void OnNotifyPropertyChanged(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            OnNotifyPropertyChanged(new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Raises the <see cref="E:NotifyPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="ArgumentNullException">Thrown when an argument is null.</exception>
        protected virtual void OnNotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            // Thread-safe invocation of the event...
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion
    }
}
