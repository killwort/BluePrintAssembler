using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using BluePrintAssembler.Domain;
using QuickGraph;

namespace BluePrintAssembler.UI.VM
{
    [Serializable]
    public class SelectRecipe: BaseFlowNode, ISelectableElement
    {
        public override float Speed => 1;

        public SelectRecipe(IEnumerable<Domain.Recipe> enumerable, BaseProducibleObject result)
        {
            PossibleRecipes = new ObservableCollection<Recipe>(enumerable.Select(x => new Recipe(x)));
            Results = new[] {new RecipeIO(this, new ItemWithAmount {Name = result.Name, Type = result.Type, Amount = 1, Probability = 1})};
        }

        public SelectRecipe(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Results = new[] { new RecipeIO(this, new ItemWithAmount { Name = info.GetString("Name"), Type = info.GetString("Type"), Amount = 1, Probability = 1 }) };
            PossibleRecipes = new ObservableCollection<Recipe>(((string[]) info.GetValue("Possibilities", typeof(string[]))).Select(x => new Recipe(Configuration.Instance.RawData.Recipes[x])));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Type",Results.First().MyItem.Type);
            info.AddValue("Name", Results.First().MyItem.Name);
            info.AddValue("Possibilities",PossibleRecipes.Select(x=>x.MyRecipe.Name).ToArray());
        }

        public ObservableCollection<Recipe> PossibleRecipes { get; }


        public override IEnumerable<Edge<IGraphNode>> IngressEdges=>new Edge<IGraphNode>[0];
        public override IEnumerable<Edge<IGraphNode>> EgressEdges => Results.SelectMany(x => x.RelatedItems);
    }
}
