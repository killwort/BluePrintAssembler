using System.Collections.Generic;
using System.Linq;
using BluePrintAssembler.Domain;
using QuickGraph;

namespace BluePrintAssembler.UI.VM
{
    public class ManualItemSource:BaseFlowNode,IGraphNode
    {

        public ManualItemSource(BaseProducibleObject result)
        {
            MyItem = result;
            Results = new RecipeIO[] {new RecipeIO(this, new ItemWithAmount {Name = result.Name, Type = result.Type, Amount = 1, Probability = 1})};
        }

        public BaseProducibleObject MyItem { get; }
        public override IEnumerable<Edge<IGraphNode>> IngressEdges=>new Edge<IGraphNode>[0];
        public override IEnumerable<Edge<IGraphNode>> EgressEdges => Results.SelectMany(x => x.RelatedItems);
    }
}