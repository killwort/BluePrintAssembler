using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using BluePrintAssembler.Annotations;
using BluePrintAssembler.Domain;
using BluePrintAssembler.Utils;
using Point = System.Windows.Point;

namespace BluePrintAssembler.UI.VM
{
    public class ProducibleItem:INotifyPropertyChanged
    {
        public Domain.BaseProducibleObject MyItem
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
                //if (_egress != null) _egress.PropertyChanged -= ConnectedPropertyChanged;

                _egress = value;
                //if (_egress != null) _egress.PropertyChanged -= ConnectedPropertyChanged;

            }
        }

        public RecipeIO Ingress
        {
            get { return _ingress; }
            set
            {
                //if (_ingress != null) _ingress.PropertyChanged -= ConnectedPropertyChanged;
                _ingress = value;
                //if (_ingress != null) _ingress.PropertyChanged += ConnectedPropertyChanged;

            }
        }

        private void ConnectedPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ConnectionPoint")
            {
                OnPropertyChanged(nameof(SinkPoint));
                OnPropertyChanged(nameof(SinkTangent));
                OnPropertyChanged(nameof(SourcePoint));
                OnPropertyChanged(nameof(SourceTangent));
            }
        }

        public NotifyTaskCompletion<Bitmap> Icon => _icon ?? (_icon = new NotifyTaskCompletion<Bitmap>(Configuration.Instance.GetIcon(_myItem)));
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        //Required to draw correctly
        public double Left { get { return 0; } }
        public double Top { get { return 0; } }

        public Point SourcePoint
        {
            get { return Egress.ConnectionPoint; }
        }
        public Point SinkPoint
        {
            get { return Ingress.ConnectionPoint; }
        }
        public Point SourceTangent
        {
            get
            {
                var w = SinkPoint.X - SourcePoint.X;
                if (Math.Abs(w) < 100.0) w = Math.Sign(w) * 100.0;
                return new Point(w > 0 ? (w / 2.0 + SourcePoint.X) : SourcePoint.X - w / 2.0, SourcePoint.Y);
            }
        }
        public Point SinkTangent
        {
            get
            {
                var w = SinkPoint.X - SourcePoint.X;
                if (Math.Abs(w) < 100.0) w = Math.Sign(w) * 100.0;
                return new Point(w > 0 ? (-w / 2.0 + SinkPoint.X) : SinkPoint.X + w / 2.0, SinkPoint.Y);
            }
        }
    }
}