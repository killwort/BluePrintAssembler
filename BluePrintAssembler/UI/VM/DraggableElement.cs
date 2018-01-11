using System.ComponentModel;
using System.Runtime.CompilerServices;
using BluePrintAssembler.Annotations;

namespace BluePrintAssembler.UI.VM
{
    public class DraggableElement:INotifyPropertyChanged
    {
       /* private double _left;
        private double _top;

        public double Left
        {
            get { return _left; }
            set
            {
                if (value.Equals(_left)) return;
                _left = value;
                OnPropertyChanged();
            }
        }

        public double Top
        {
            get { return _top; }
            set
            {
                if (value.Equals(_top)) return;
                _top = value;
                OnPropertyChanged();
            }
        }*/

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}