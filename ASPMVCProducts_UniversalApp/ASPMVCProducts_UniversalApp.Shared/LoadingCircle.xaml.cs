using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Xaml;
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
            Window.Current.VisibilityChanged += (o, e) =>
            {
                if(e.Visible)
                {
                    for (int i = 0; i < 8; ++i)
                    {
                        var lSB = (Storyboard)this.Resources["Animation" + i];
                        lSB.Begin();
                    }
                }
                else
                {
                    for (int i = 0; i < 8; ++i)
                    {
                        var lSB = (Storyboard)this.Resources["Animation" + i];
                        lSB.Stop();
                    }
                }
            };
        }
    }
}
