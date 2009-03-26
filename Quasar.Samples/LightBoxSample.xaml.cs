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

namespace Quasar.Samples
{
    public partial class LightBoxSample : UserControl
    {
        public LightBoxSample()
        {
            InitializeComponent();

            this.DataContext = new LightBoxViewModel();
        }

        private void OpenLightBox(object sender, RoutedEventArgs e)
        {
            ((LightBoxViewModel)this.DataContext).IsOpened = true;
        }
    }
}
