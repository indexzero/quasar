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
using System.ComponentModel;

namespace Quasar.Samples
{
    public class LightBoxViewModel : INotifyPropertyChanged
    {
        private bool isOpened = false;

        private LightBoxContent lightBoxContent;

        public LightBoxViewModel()
        {
            lightBoxContent = new LightBoxContent("LightBox Sample", "Sample LightBox Content");
        }

        public bool IsOpened
        {
            get { return isOpened; }
            set
            {
                isOpened = value;
                NotifyPropertyChanged("IsOpened");
            }
        }

        public LightBoxContent LightBoxContent
        {
            get { return lightBoxContent; }
            set
            {
                lightBoxContent = value;
                NotifyPropertyChanged("LightBoxContent");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
