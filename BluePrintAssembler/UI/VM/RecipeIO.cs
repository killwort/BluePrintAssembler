﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using BluePrintAssembler.Annotations;
using BluePrintAssembler.Domain;
using BluePrintAssembler.Utils;
using Point = System.Windows.Point;

namespace BluePrintAssembler.UI.VM
{
    public class RecipeIO:INotifyPropertyChanged
    {
        private NotifyTaskCompletion<Bitmap> _icon;
        private ItemWithAmount _myItem;
        private Point _connectionPoint;

        public BaseFlowNode Parent { get; }

        public ItemWithAmount MyItem
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

        public RecipeIO(BaseFlowNode parent,ItemWithAmount myItem)
        {
            Parent = parent;
            MyItem = myItem;
            Parent.PropertyChanged += (sender, arg) =>
            {
                if (arg.PropertyName == nameof(Recipe.Speed))
                    OnPropertyChanged(nameof(Rate));
                if (arg.PropertyName == nameof(Recipe.BaseSpeed))
                    OnPropertyChanged(nameof(BaseRate));
            };
        }

        public NotifyTaskCompletion<Bitmap> Icon => _icon ?? (_icon = new NotifyTaskCompletion<Bitmap>(Configuration.Instance.GetIcon(Configuration.Instance.RawData.Get(_myItem.Type,_myItem.Name))));

        public Point ConnectionPoint
        {
            get { return _connectionPoint; }
            set
            {
                if (value.Equals(_connectionPoint)) return;
                _connectionPoint = value;
                OnPropertyChanged();
            }
        }

        public BaseProducibleObject RealItem => Configuration.Instance.RawData.Get(MyItem.Type, MyItem.Name);
        public List<ProducibleItem> RelatedItems { get; private set; } = new List<ProducibleItem>();

        public float Rate => MyItem.Amount * Parent.Speed;
        public float BaseRate => MyItem.Amount * Parent.BaseSpeed;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}