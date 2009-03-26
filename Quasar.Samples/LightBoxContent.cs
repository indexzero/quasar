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
    public class LightBoxContent : INotifyPropertyChanged
    {
        private string content;

        private string header;

        public LightBoxContent(string header, string content)
        {
            this.header = header;
            this.content = content;
        }

        public string Content
        {
            get { return content; }
            set
            {
                content = value;
                NotifyPropertyChanged("Content");
            }
        }
        
        public string Header
        {
            get { return header; }
            set
            {
                header = value;
                NotifyPropertyChanged("Header");
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
