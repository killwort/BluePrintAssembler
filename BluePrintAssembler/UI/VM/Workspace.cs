using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using BluePrintAssembler.Domain;
using BluePrintAssembler.Utils;
using QuickGraph;

namespace BluePrintAssembler.UI.VM
{
    [Serializable]
    public class Workspace : IBidirectionalGraph<IGraphNode, ProducibleItem>, ISerializable
    {
        public Workspace()
        {
            WantedResults.CollectionChanged += CallFixProductionFlow;
            ExistingSources.CollectionChanged += CallFixProductionFlow;

            BindCommands();
        }


        #region Commands
        public ICommand UseRecipe { get; private set; }
        public ICommand AddToFactory { get; private set; }
        public ICommand Clear { get; private set; }

        private void BindCommands()
        {
            AddToFactory = new DelegateCommand<BaseFlowNode>(AddedToFactory);
            UseRecipe = new DelegateCommand<Tuple<BaseFlowNode, Recipe>>(SelectorRecipeUsed);
            Clear = new DelegateCommand<bool>(Cleared);
        }

        private void AddedToFactory(BaseFlowNode e)
        {
            ProductionNodes.Remove(e);
            foreach (var r in e.Results)
            {
                _satisfierNodes.Remove(r.RealItem);
                ExistingSources.Add(new ProducibleItemWithAmount(r.RealItem));
            }

            FixProductionFlow();
        }

        private void SelectorRecipeUsed(Tuple<BaseFlowNode, Recipe> e)
        {
            var satisfier = _satisfierNodes.First(x => x.Value == e.Item1);
            ProductionNodes.Remove(e.Item1);
            _selectedRecipies[satisfier.Key] = e.Item2.MyRecipe;
            var newSatisfier = new Recipe(e.Item2.MyRecipe);
            foreach (var egress in e.Item2.MyRecipe.CurrentMode.Results)
                _satisfierNodes[Configuration.Instance.RawData.Get(egress.Value.Type, egress.Value.Name)] = newSatisfier;
            ProductionNodes.Add(newSatisfier);
            FixProductionFlow();
        }

        private void Cleared(bool fullClear)
        {
            ProductionNodes.Clear();
            WantedResults.Clear();
            _satisfierNodes.Clear();
            _selectedRecipies.Clear();
            if(fullClear)
                ExistingSources.Clear();
        }
        #endregion

        private readonly Dictionary<BaseProducibleObject, BaseFlowNode> _satisfierNodes = new Dictionary<BaseProducibleObject, BaseFlowNode>();
        private readonly Dictionary<BaseProducibleObject, Domain.Recipe> _selectedRecipies = new Dictionary<BaseProducibleObject, Domain.Recipe>();


        private void CallFixProductionFlow(object sender, NotifyCollectionChangedEventArgs e)
        {
            FixProductionFlow();
        }

        private void FixProductionFlow()
        {
            /*var rawData = Configuration.Instance.RawData;
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
                    //Bind(satisfier);
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
                    //Bind(selector);
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
            }*/

            FlowChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler FlowChanged;


        public CompositeCollection RenderableElements => new CompositeCollection
        {
            new CollectionContainer {Collection = Items},
            new CollectionContainer {Collection = ProductionNodes},
        };

        public ObservableCollection<BaseFlowNode> ProductionNodes { get; } = new ObservableCollection<BaseFlowNode>();
        public ObservableCollection<ProducibleItem> Items { get; } = new ObservableCollection<ProducibleItem>();

        public ObservableCollection<ProducibleItemWithAmount> WantedResults { get; } = new ObservableCollection<ProducibleItemWithAmount>();
        public ObservableCollection<ProducibleItemWithAmount> ExistingSources { get; } = new ObservableCollection<ProducibleItemWithAmount>();

        #region BidirectionalGraph implementation

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

        #endregion

        #region Serialization

        [Serializable]
        public class SelectedRecipe : ISerializable
        {
            public KeyValuePair<BaseProducibleObject, Domain.Recipe> KV { get; }

            public SelectedRecipe(KeyValuePair<BaseProducibleObject, Domain.Recipe> kv)
            {
                this.KV = kv;
            }

            public SelectedRecipe(SerializationInfo info, StreamingContext context)
            {
                KV = new KeyValuePair<BaseProducibleObject, Domain.Recipe>(
                    Configuration.Instance.RawData.Get(info.GetString("Type"), info.GetString("Name")),
                    Configuration.Instance.RawData.Recipes[info.GetString("Recipe")]
                );
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("Type", KV.Key.Type);
                info.AddValue("Name", KV.Key.Name);
                info.AddValue("Recipe", KV.Value.Name);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Nodes", ProductionNodes.ToArray());
            info.AddValue("SelectedRecipes", _selectedRecipies.Select(x => new SelectedRecipe(x)).ToArray());
            info.AddValue("Egress", WantedResults.ToArray());
            info.AddValue("Ingress", ExistingSources.ToArray());
        }

        public Workspace(SerializationInfo info, StreamingContext context)
        {
            AddToFactory = new DelegateCommand<BaseFlowNode>(AddedToFactory);
            UseRecipe = new DelegateCommand<Tuple<BaseFlowNode, Recipe>>(SelectorRecipeUsed);

            ProductionNodes = new ObservableCollection<BaseFlowNode>((BaseFlowNode[]) info.GetValue("Nodes", typeof(BaseFlowNode[])));
            /*foreach (var node in ProductionNodes)
                Bind(node);*/
            _satisfierNodes = ProductionNodes.SelectMany(x => x.Results).ToLookup(x => x.RealItem, x => x.Parent).ToDictionary(x => x.Key, x => x.First());
            _selectedRecipies = ((SelectedRecipe[]) info.GetValue("SelectedRecipes", typeof(SelectedRecipe[])))
                .ToDictionary(x => x.KV.Key, x => x.KV.Value);
            WantedResults = new ObservableCollection<ProducibleItemWithAmount>(((ProducibleItemWithAmount[]) info.GetValue("Egress", typeof(ProducibleItemWithAmount[]))));
            ExistingSources = new ObservableCollection<ProducibleItemWithAmount>(((ProducibleItemWithAmount[]) info.GetValue("Ingress", typeof(ProducibleItemWithAmount[]))));
            BindCommands();
            FixProductionFlow();
        }

        #endregion
    }
}