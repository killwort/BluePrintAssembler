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
    public class Recipe : BaseFlowNode, ISelectableElement, INotifyPropertyChanged, IGraphNode
    {
        private Domain.Recipe _myRecipe;
        private NotifyTaskCompletion<Bitmap> _icon;
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

        public Recipe(Domain.Recipe value)
        {
            MyRecipe = value;
        }

        public Domain.Recipe MyRecipe
        {
            get => _myRecipe;
            set
            {
                if (Equals(value, _myRecipe)) return;
                _myRecipe = value;
                _icon = null;
                Sources = _myRecipe.CurrentMode.Sources?.Select(x => new RecipeIO(this, x.Value)).ToArray() ?? new RecipeIO[0];
                Results = _myRecipe.CurrentMode.Results?.Select(x => new RecipeIO(this, x.Value)).ToArray() ?? new RecipeIO[0];
                OnPropertyChanged();
                OnPropertyChanged(nameof(Sources));
                OnPropertyChanged(nameof(Results));
            }
        }

        public NotifyTaskCompletion<Bitmap> Icon => _icon ?? (_icon = new NotifyTaskCompletion<Bitmap>(Configuration.Instance.GetIcon(_myRecipe)));
        /*public override RecipeIO[] Sources { get; private set; }
        public override RecipeIO[] Results { get; private set; }*/
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public float Speed => (float) Math.Round(1f / MyRecipe.CurrentMode.BaseProductionTime, 1);
        public override IEnumerable<Edge<IGraphNode>> IngressEdges => Sources.SelectMany(x => x.RelatedItems);
        public override IEnumerable<Edge<IGraphNode>> EgressEdges => Results.SelectMany(x => x.RelatedItems);
    }
}