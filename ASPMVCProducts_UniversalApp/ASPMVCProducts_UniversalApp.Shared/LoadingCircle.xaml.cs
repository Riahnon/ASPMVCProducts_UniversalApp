using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace ASPMVCProducts_UniversalApp
{
    /// <summary>
    /// Interaction logic for LoadingCircle.xaml
    /// </summary>
    public partial class LoadingCircle : UserControl
    {
        public LoadingCircle()
        {
            InitializeComponent();
            this.Loaded += (o, e) =>
            {
                for (int i = 0; i < 8; ++i)
                {
                    var lSB = (Storyboard)this.Resources["Animation" + i];
                    lSB.Begin();
                }
            };
        }
    }
}
