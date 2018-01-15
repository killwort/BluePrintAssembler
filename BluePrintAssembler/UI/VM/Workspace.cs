using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Data;
using BluePrintAssembler.Domain;
using QuickGraph;

namespace BluePrintAssembler.UI.VM
{
    public class Workspace : IBidirectionalGraph<IGraphNode, ProducibleItem>
    {
        public Workspace()
        {
            WantedResults.CollectionChanged += CallFixProductionFlow;
            ExistingSources.CollectionChanged += CallFixProductionFlow;
        }


        private readonly Dictionary<BaseProducibleObject, BaseFlowNode> _satisfierNodes = new Dictionary<BaseProducibleObject, BaseFlowNode>();
        private readonly Dictionary<BaseProducibleObject, Domain.Recipe> _selectedRecipies = new Dictionary<BaseProducibleObject, Domain.Recipe>();

        private void CallFixProductionFlow(object sender, NotifyCollectionChangedEventArgs e)
        {
            FixProductionFlow();
        }

        private void FixProductionFlow()
        {
            var rawData = Configuration.Instance.RawData;
            var unsatisfied = new HashSet<BaseProducibleObject>(
                _satisfierNodes.SelectMany(x => x.Value.Sources.Select(ingress => ingress.RealItem))
                    .Concat(WantedResults.Select(w => w.MyItem))
                    .Except(_satisfierNodes.Keys)
                    .Except(ExistingSources.Select(x => x.MyItem))
            );
            while (unsatisfied.Any())
            {
                var result = unsatisfied.First();
                unsatisfied.Remove(result);
                var possibleRecipies = rawData.Recipes.Where(x => x.Value.HasResult(result)).ToArray();
                _selectedRecipies.TryGetValue(result, out var selectedRecipe);
                if (!possibleRecipies.Any())
                {
                    var satisfier = new ManualItemSource(result);
                    ProductionNodes.Add(satisfier);
                    _satisfierNodes[result] = satisfier;
                }
                else if (possibleRecipies.Length == 1 || selectedRecipe != null)
                {
                    var recipe = selectedRecipe ?? possibleRecipies.First().Value;
                    var satisfier = new Recipe(recipe);
                    ProductionNodes.Add(satisfier);
                    foreach (var r in recipe.CurrentMode.Results)
                    {
                        var res = rawData.Get(r.Value.Type, r.Value.Name);
                        _satisfierNodes[res] = satisfier;
                        unsatisfied.Remove(res);
                    }

                    foreach (var r in possibleRecipies.First().Value.CurrentMode.Sources)
                    {
                        var res = rawData.Get(r.Value.Type, r.Value.Name);
                        if (!_satisfierNodes.ContainsKey(res))
                        {
                            unsatisfied.Add(res);
                        }
                    }
                }
                else
                {
                    var selector = new SelectRecipe(possibleRecipies.Select(x => x.Value), result);
                    ProductionNodes.Add(selector);
                    selector.RecipeUsed += SelectorRecipeUsed;
                    _satisfierNodes[result] = selector;
                }
            }

            Items.Clear();
            foreach (var source in _satisfierNodes)
            {
                var egress = source.Value.Results.First(x => x.RealItem.Equals(source.Key));
                foreach (var dest in _satisfierNodes.Values.SelectMany(x => x.Sources?.Where(ingress => ingress.RealItem.Equals(source.Key)) ?? new RecipeIO[0]))
                {
                    var edge = new ProducibleItem(egress, dest, source.Key);
                    Items.Add(edge);
                }
            }
        }

        private void SelectorRecipeUsed(object sender, Recipe e)
        {
            var satisfier = _satisfierNodes.First(x => x.Value == sender);
            ProductionNodes.Remove((SelectRecipe) sender);
            _selectedRecipies[satisfier.Key] = e.MyRecipe;
            var newSatisfier = new Recipe(e.MyRecipe);
            foreach (var egress in e.MyRecipe.CurrentMode.Results)
                _satisfierNodes[Configuration.Instance.RawData.Get(egress.Value.Type, egress.Value.Name)] = newSatisfier;
            ProductionNodes.Add(newSatisfier);
            FixProductionFlow();
        }

        public CompositeCollection RenderableElements => new CompositeCollection
        {
            new CollectionContainer {Collection = ProductionNodes},
            new CollectionContainer {Collection = Items},
        };

        public ObservableCollection<BaseFlowNode> ProductionNodes { get; } = new ObservableCollection<BaseFlowNode>();
        public ObservableCollection<ProducibleItem> Items { get; } = new ObservableCollection<ProducibleItem>();

        public ObservableCollection<ProducibleItemWithAmount> WantedResults { get; } = new ObservableCollection<ProducibleItemWithAmount>();
        public ObservableCollection<ProducibleItemWithAmount> ExistingSources { get; } = new ObservableCollection<ProducibleItemWithAmount>();

        public void TestAddItem()
        {
            WantedResults.Add(new ProducibleItemWithAmount {MyItem = Configuration.Instance.RawData.Items["iron-plate"], Amount = 1});
            WantedResults.Add(new ProducibleItemWithAmount {MyItem = Configuration.Instance.RawData.Items["copper-plate"], Amount = 1});
        }

        public bool IsDirected => true;
        public bool AllowParallelEdges => false;

        public bool ContainsVertex(IGraphNode vertex)
        {
            return ProductionNodes.Contains(vertex);
        }

        public bool IsOutEdgesEmpty(IGraphNode v)
        {
            return !v.EgressEdges.Any();
        }

        public int OutDegree(IGraphNode v)
        {
            return v.EgressEdges.Count();
        }

        public IEnumerable<ProducibleItem> OutEdges(IGraphNode v)
        {
            return v.EgressEdges.Cast<ProducibleItem>();
        }

        public bool TryGetOutEdges(IGraphNode v, out IEnumerable<ProducibleItem> edges)
        {
            edges = v.EgressEdges.Cast<ProducibleItem>();
            return true;
        }

        public ProducibleItem OutEdge(IGraphNode v, int index)
        {
            return (ProducibleItem) v.EgressEdges.Skip(index).First();
        }

        public bool ContainsEdge(IGraphNode source, IGraphNode target)
        {
            return source.EgressEdges.Any(e => target.IngressEdges.Contains(e))
                   ||
                   source.IngressEdges.Any(e => target.EgressEdges.Contains(e));
        }

        public bool TryGetEdges(IGraphNode source, IGraphNode target, out IEnumerable<ProducibleItem> edges)
        {
            edges = source.EgressEdges.Where(e => target.IngressEdges.Contains(e)).Cast<ProducibleItem>()
                .Concat(source.IngressEdges.Where(e => target.EgressEdges.Contains(e)).Cast<ProducibleItem>());
            return true;
        }

        public bool TryGetEdge(IGraphNode source, IGraphNode target, out ProducibleItem edge)
        {
            edge = source.EgressEdges.Where(e => target.IngressEdges.Contains(e)).Cast<ProducibleItem>()
                .Concat(source.IngressEdges.Where(e => target.EgressEdges.Contains(e)).Cast<ProducibleItem>()).FirstOrDefault();
            return edge != null;
        }

        public bool IsVerticesEmpty => ProductionNodes.Count == 0;
        public int VertexCount => ProductionNodes.Count;
        public IEnumerable<IGraphNode> Vertices => ProductionNodes;

        public bool ContainsEdge(ProducibleItem edge)
        {
            return Items.Contains(edge);
        }

        public bool IsEdgesEmpty => !Items.Any();
        public int EdgeCount => Items.Count;
        public IEnumerable<ProducibleItem> Edges => Items;

        public bool IsInEdgesEmpty(IGraphNode v)
        {
            return !v.IngressEdges.Any();
        }

        public int InDegree(IGraphNode v)
        {
            return v.IngressEdges.Count();
        }

        public IEnumerable<ProducibleItem> InEdges(IGraphNode v)
        {
            return v.IngressEdges.Cast<ProducibleItem>();
        }

        public bool TryGetInEdges(IGraphNode v, out IEnumerable<ProducibleItem> edges)
        {
            edges = v.IngressEdges.Cast<ProducibleItem>();
            return true;
        }

        public ProducibleItem InEdge(IGraphNode v, int index)
        {
            return (ProducibleItem) v.IngressEdges.Skip(index).First();
        }

        public int Degree(IGraphNode v)
        {
            return v.IngressEdges.Count() + v.EgressEdges.Count();
        }
    }
}