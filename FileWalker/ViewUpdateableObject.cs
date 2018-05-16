using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FileWalker {
    public class ViewUpdateableObject : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;

        public void Notify([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}