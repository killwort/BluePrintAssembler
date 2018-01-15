using System.Collections.Generic;
using QuickGraph;

namespace BluePrintAssembler.UI.VM
{
    public abstract class BaseFlowNode:IGraphNode{
        public abstract IEnumerable<Edge<IGraphNode>> IngressEdges { get; }
        public abstract IEnumerable<Edge<IGraphNode>> EgressEdges { get; }
        public RecipeIO[] Sources { get; protected set; }=new RecipeIO[0];
        public RecipeIO[] Results { get; protected set; }=new RecipeIO[0];
    }
}