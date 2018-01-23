using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using BluePrintAssembler.Annotations;
using QuickGraph;

namespace BluePrintAssembler.UI.VM
{
    [Serializable]
    public abstract class BaseFlowNode : IGraphNode, INotifyPropertyChanged, ISerializable
    {
        public abstract IEnumerable<Edge<IGraphNode>> IngressEdges { get; }
        public abstract IEnumerable<Edge<IGraphNode>> EgressEdges { get; }
        public RecipeIO[] Sources { get; protected set; } = new RecipeIO[0];
        public RecipeIO[] Results { get; protected set; } = new RecipeIO[0];
        public abstract float Speed { get; }
        public abstract float BaseSpeed { get; }
        private double _left;
        private double _top;

        public double LayoutTop
        {
            get { return _top; }
            set
            {
                if (value.Equals(_top)) return;
                _top = value;
                OnPropertyChanged();
            }
        }

        public double LayoutLeft
        {
            get { return _left; }
            set
            {
                if (value.Equals(_left)) return;
                _left = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("LayoutLeft", LayoutLeft);
            info.AddValue("LayoutTop", LayoutTop);
        }
        public BaseFlowNode(SerializationInfo info, StreamingContext context)
        {
            LayoutLeft=info.GetDouble("LayoutLeft");
            LayoutTop=info.GetDouble("LayoutTop");
        }
        public BaseFlowNode() { }
    }
}