using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using BluePrintAssembler.Annotations;
using BluePrintAssembler.Domain;
using BluePrintAssembler.Utils;
using QuickGraph;
using Color = System.Windows.Media.Color;

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
        private RecipeIO _egress;
        private RecipeIO _ingress;

        public RecipeIO Egress
        {
            get { return _egress; }
            set
            {
                if(_egress!=null)
                    _egress.PropertyChanged -= MaybeRecalcBalanceColor;
                if (Equals(value, _egress)) return;
                _egress = value;
                if(_egress!=null)
                    _egress.PropertyChanged += MaybeRecalcBalanceColor;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BalanceColor));
            }
        }

        public RecipeIO Ingress
        {
            get { return _ingress; }
            set
            {
                if(_ingress!=null)
                    _ingress.PropertyChanged -= MaybeRecalcBalanceColor;
                if (Equals(value, _ingress)) return;
                _ingress = value;
                if(_ingress!=null)
                    _ingress.PropertyChanged += MaybeRecalcBalanceColor;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BalanceColor));
            }
        }

        private void MaybeRecalcBalanceColor(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(RecipeIO.Rate))
                OnPropertyChanged(nameof(BalanceColor));
        }

        public float Balance => Egress.Rate - Ingress.Rate;

        public Color BalanceColor
        {
            get
            {
                var rate = (Egress.Rate / Ingress.Rate);
                if (rate > 1)
                    return Color.FromRgb(0, 0, (byte) Math.Min((rate-1) * 255, 255));
                return Color.FromRgb((byte) Math.Min((1.0 / rate-1) * 255, 255), 0, 0);
            }
        }

        public NotifyTaskCompletion<Bitmap> Icon => _icon ?? (_icon = new NotifyTaskCompletion<Bitmap>(Configuration.Instance.GetIcon(_myItem)));
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ProducibleItem(RecipeIO source, RecipeIO target, BaseProducibleObject item) : base(source.Parent, target.Parent)
        {
            Egress = source;
            Ingress = target;
            MyItem = item;
            source.RelatedItems.Add(this);
            target.RelatedItems.Add(this);
        }
    }
}