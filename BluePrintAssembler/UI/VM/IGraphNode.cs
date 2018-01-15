using System.Collections.Generic;
using QuickGraph;

namespace BluePrintAssembler.UI.VM
{
    public interface IGraphNode
    {
        IEnumerable<Edge<IGraphNode>> IngressEdges { get; }
        IEnumerable<Edge<IGraphNode>> EgressEdges { get; }
    }
}