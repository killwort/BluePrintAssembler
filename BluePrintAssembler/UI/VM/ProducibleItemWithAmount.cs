using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using BluePrintAssembler.Annotations;
using BluePrintAssembler.Domain;
using BluePrintAssembler.Utils;

namespace BluePrintAssembler.UI.VM
{
    [Serializable]
    public class ProducibleItemWithAmount:INotifyPropertyChanged,ISerializable
    {
        public ProducibleItemWithAmount(BaseProducibleObject item)
        {
            _myItem = item;
        }

        private BaseProducibleObject _myItem;
        private NotifyTaskCompletion<Bitmap> _icon;
        private float _amount;
        public NotifyTaskCompletion<Bitmap> Icon => _icon ?? (_icon = new NotifyTaskCompletion<Bitmap>(Configuration.Instance.GetIcon(_myItem)));

        public BaseProducibleObject MyItem
        {
            get { return _myItem; }
            set
            {
                if (Equals(value, _myItem)) return;
                _myItem = value;
                _icon = null;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Icon));
            }
        }

        public float Amount
        {
            get { return _amount; }
            set
            {
                if (value.Equals(_amount)) return;
                _amount = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Type",MyItem.Type);
            info.AddValue("Name", MyItem.Name);
            info.AddValue("Amount", Amount);
        }
        public ProducibleItemWithAmount(SerializationInfo info, StreamingContext context)
        {
            MyItem=Configuration.Instance.RawData.Get(info.GetString("Type"),info.GetString("Name"));
            Amount=info.GetSingle("Amount");
        }
    }
}
