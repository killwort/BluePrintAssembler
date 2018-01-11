﻿using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using BluePrintAssembler.Annotations;
using BluePrintAssembler.Domain;
using BluePrintAssembler.Utils;

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
        public Recipe Egress { get; set; }
        public Recipe Ingress { get; set; }
        public NotifyTaskCompletion<Bitmap> Icon => _icon ?? (_icon = new NotifyTaskCompletion<Bitmap>(Configuration.Instance.GetIcon(_myItem)));
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}