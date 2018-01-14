using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using BluePrintAssembler.Annotations;
using BluePrintAssembler.Domain;
using BluePrintAssembler.Utils;
using QuickGraph;

namespace BluePrintAssembler.UI.VM
{
    public class ProducibleItem:Edge<IGraphNode>,INotifyPropertyChanged
    {
        public BaseProducibleObject MyItem
        {
            get { return _myItem; }
            set
            {
                if (Equals(value, _myItem)) return;
                _myItem = value;
                _icon = null;
                OnPropertyChanged();
            }
        }

        private NotifyTaskCompletion<Bitmap> _icon;
        private BaseProducibleObject _myItem;

        public RecipeIO Egress { get; set; }
        public RecipeIO Ingress { get; set; }

        
        public NotifyTaskCompletion<Bitmap> Icon => _icon ?? (_icon = new NotifyTaskCompletion<Bitmap>(Configuration.Instance.GetIcon(_myItem)));
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ProducibleItem(RecipeIO source, RecipeIO target) : base(source.Parent, target.Parent)
        {
            Egress = source;
            Ingress = target;
            source.RelatedItems.Add(this);
            target.RelatedItems.Add(this);
        }
    }
}