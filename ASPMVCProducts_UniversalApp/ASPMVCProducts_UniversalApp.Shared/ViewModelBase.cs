using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Windows.UI.Core;

namespace ASPMVCProducts_UniversalApp
{
    public class ViewModelBase<T> : INotifyPropertyChanged where T:INotifyPropertyChanged
    {
        public CoreDispatcher Dispatcher { get; protected set; }
        public T Model { get; protected set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ViewModelBase(CoreDispatcher aDispatcher, T aModel)
        {
            Dispatcher = aDispatcher;
            Model = aModel;
            Model.PropertyChanged += _OnModelPropertyChanged;
        }

        private void _OnModelPropertyChanged( object sender, PropertyChangedEventArgs e)
        { 
            if(Dispatcher.HasThreadAccess)
            {
                _NotifyPropertyChanged(e.PropertyName);
            }
            else
            {
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => _OnModelPropertyChanged(sender, e));
            }
        }

        private void _NotifyPropertyChanged(string aPropertyName)
        {
            var lHandler = PropertyChanged;
            if (lHandler != null)
            {
                lHandler(this, new PropertyChangedEventArgs(aPropertyName));
            }
        }
    }
}
