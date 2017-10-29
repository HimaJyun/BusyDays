using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BusyDays.Common {
    public abstract class NotifyBase : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(params string[] propertyNames) {
            foreach (var name in propertyNames) {
                this.OnPropertyChanged(name);
            }
        }
    }
}
