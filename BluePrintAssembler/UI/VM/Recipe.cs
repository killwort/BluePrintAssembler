using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using BluePrintAssembler.Domain;
using BluePrintAssembler.Utils;
using QuickGraph;

namespace BluePrintAssembler.UI.VM
{
    [Serializable]
    public class Recipe : BaseFlowNode, ISelectableElement
    {
        public Recipe(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            MyRecipe = Configuration.Instance.RawData.Recipes[info.GetString("Name")];
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Name",MyRecipe.Name);
        }

        private Domain.Recipe _myRecipe;
        private NotifyTaskCompletion<Bitmap> _icon;
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
        public float Speed => (float) Math.Round(1f / MyRecipe.CurrentMode.BaseProductionTime, 1);
        public override IEnumerable<Edge<IGraphNode>> IngressEdges => Sources.SelectMany(x => x.RelatedItems);
        public override IEnumerable<Edge<IGraphNode>> EgressEdges => Results.SelectMany(x => x.RelatedItems);
    }
}