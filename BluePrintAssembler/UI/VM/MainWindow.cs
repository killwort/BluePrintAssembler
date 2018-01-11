using System.ComponentModel;
using System.Runtime.CompilerServices;
using BluePrintAssembler.Annotations;

namespace BluePrintAssembler.UI.VM
{
    public class MainWindow:INotifyPropertyChanged
    {
        private Workspace _currentWorkspace=new Workspace();

        public Workspace CurrentWorkspace
        {
            get => _currentWorkspace;
            set
            {
                if (Equals(value, _currentWorkspace)) return;
                _currentWorkspace = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
