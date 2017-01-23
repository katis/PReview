using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PReview
{
    public abstract class NotifyExt : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Set<T>(ref T field, T value, [CallerMemberName] string name = null)
        {
            field = value;
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        protected static void RunInUI(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }
    }
}
