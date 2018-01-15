using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using BluePrintAssembler.Annotations;
using BluePrintAssembler.Domain;
using BluePrintAssembler.Utils;
using QuickGraph;

namespace BluePrintAssembler.UI.VM
{
    public class ManualItemSource:BaseFlowNode,IGraphNode,INotifyPropertyChanged, ISelectableElement
    {
        private NotifyTaskCompletion<Bitmap> _icon;

        public ManualItemSource(BaseProducibleObject result)
        {
            MyItem = result;
            Results = new RecipeIO[] {new RecipeIO(this, new ItemWithAmount {Name = result.Name, Type = result.Type, Amount = 1, Probability = 1})};
        }

        public BaseProducibleObject MyItem { get; }
        public override IEnumerable<Edge<IGraphNode>> IngressEdges=>new Edge<IGraphNode>[0];
        public override IEnumerable<Edge<IGraphNode>> EgressEdges => Results.SelectMany(x => x.RelatedItems);
        public NotifyTaskCompletion<Bitmap> Icon => _icon ?? (_icon = new NotifyTaskCompletion<Bitmap>(Configuration.Instance.GetIcon(MyItem)));

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

        public event EventHandler<BaseProducibleObject> AddedToFactory;

        public void AddToFactory()
        {
            AddedToFactory?.Invoke(this, Results.First().RealItem);
        }
    }
}