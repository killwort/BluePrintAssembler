using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using BluePrintAssembler.Domain;
using QuickGraph;

namespace BluePrintAssembler.UI.VM
{
    public class Workspace : IBidirectionalGraph<IGraphNode, ProducibleItem>
    {
        public CompositeCollection RenderableElements => new CompositeCollection
        {
            new CollectionContainer {Collection = Recipes},
            new CollectionContainer {Collection = Items},
            new CollectionContainer {Collection = UnspecifiedRecipies}
        };

        public ObservableCollection<Recipe> Recipes { get; } = new ObservableCollection<Recipe>();
        public ObservableCollection<ProducibleItem> Items { get; } = new ObservableCollection<ProducibleItem>();
        public ObservableCollection<RecipeSelector> UnspecifiedRecipies { get; } = new ObservableCollection<RecipeSelector>();
        public ObservableCollection<ManualItemSource> ManualItemSources { get; } = new ObservableCollection<ManualItemSource>();

        public void Test()
        {
            var rawData = Configuration.Instance.RawData;
            var satisfied = new HashSet<BaseProducibleObject>();
            var satisfiedRaw = new HashSet<BaseProducibleObject>();
            var unsatisfied = new HashSet<BaseProducibleObject> {rawData.Items["iron-plate"]};
            while (unsatisfied.Any())
            {
                var result = unsatisfied.First();
                var possibleRecipies = rawData.Recipes.Where(x => x.Value.HasResult(result)).ToArray();
                if (!possibleRecipies.Any())
                {
                    unsatisfied.Remove(result);
                    satisfiedRaw.Add(result);
                }
                else
                {
                    Recipes.Add(new Recipe {MyRecipe = possibleRecipies.First().Value});
                    foreach (var r in possibleRecipies.First().Value.CurrentMode.Results)
                    {
                        var res = rawData.Get(r.Value.Type, r.Value.Name);
                        satisfied.Add(res);
                        unsatisfied.Remove(res);
                    }

                    foreach (var r in possibleRecipies.First().Value.CurrentMode.Sources)
                    {
                        var res = rawData.Get(r.Value.Type, r.Value.Name);
                        if (!satisfied.Contains(res) && !satisfiedRaw.Contains(res))
                        {
                            unsatisfied.Add(res);
                        }
                    }
                }
            }

            foreach (var s in satisfied)
            {
                foreach (var pair in
                    Recipes.SelectMany(x => x.Sources.Where(i => i.MyItem.Type == s.Type && i.MyItem.Name == s.Name))
                        .SelectMany(ingress => Recipes.SelectMany(x => x.Results.Where(i => i.MyItem.Type == s.Type && i.MyItem.Name == s.Name).Select(egress => new ProducibleItem(egress, ingress))))
                    //Recipes.Where(x => x.MyRecipe.HasResult(s)).SelectMany(egress => Recipes.Where(x => x.MyRecipe.HasSource(s)).Select(ingress => new ProducibleItem {Egress = egress, Ingress = ingress}))
                )
                {
                    pair.MyItem = s;
                    Items.Add(pair);
                }
            }

            foreach (var u in unsatisfied)
            {
                foreach (var unsatisfiedSlot in Recipes.Where(x => x.MyRecipe.HasResult(u)))
                {
                    ManualItemSources.Add(new ManualItemSource {MyItem = u, Egress = unsatisfiedSlot});
                }
            }
        }

        public bool IsDirected => true;
        public bool AllowParallelEdges => false;

        public bool ContainsVertex(IGraphNode vertex)
        {
            return Recipes.Contains(vertex)
                //||ManualItemSources.Contains(vertex)
                //||UnspecifiedRecipies.Contains(vertex)
                ;
        }

        public bool IsOutEdgesEmpty(IGraphNode v)
        {
            return !Items.Any(x => x.Egress.Parent == v);
        }

        public int OutDegree(IGraphNode v)
        {
            return Items.Count(x => x.Egress.Parent == v);
        }

        public IEnumerable<ProducibleItem> OutEdges(IGraphNode v)
        {
            return Items.Where(x => x.Egress.Parent == v);
        }

        public bool TryGetOutEdges(IGraphNode v, out IEnumerable<ProducibleItem> edges)
        {
            edges = Items.Where(x => x.Egress.Parent == v);
            return true;
        }

        public ProducibleItem OutEdge(IGraphNode v, int index)
        {
            return Items.Where(x => x.Egress.Parent == v).Skip(index).First();
        }

        public bool ContainsEdge(IGraphNode source, IGraphNode target)
        {
            return Items.Any(x => x.Egress.Parent == source && x.Ingress.Parent == target);
        }

        public bool TryGetEdges(IGraphNode source, IGraphNode target, out IEnumerable<ProducibleItem> edges)
        {
            edges = Items.Where(x => x.Egress.Parent == source && x.Ingress.Parent == target);
            return true;
        }

        public bool TryGetEdge(IGraphNode source, IGraphNode target, out ProducibleItem edge)
        {
            edge = Items.FirstOrDefault(x => x.Egress.Parent == source && x.Ingress.Parent == target);
            return edge != null;
        }

        public bool IsVerticesEmpty => Recipes.Count == 0 && ManualItemSources.Count == 0 && UnspecifiedRecipies.Count == 0;
        public int VertexCount => Recipes.Count + ManualItemSources.Count + UnspecifiedRecipies.Count;
        public IEnumerable<IGraphNode> Vertices => Recipes.Cast<IGraphNode>().Concat(ManualItemSources.Cast<IGraphNode>()).Concat(UnspecifiedRecipies.Cast<IGraphNode>());

        public bool ContainsEdge(ProducibleItem edge)
        {
            return Items.Contains(edge);
        }

        public bool IsEdgesEmpty => !Items.Any();
        public int EdgeCount => Items.Count;
        public IEnumerable<ProducibleItem> Edges => Items;

        public bool IsInEdgesEmpty(IGraphNode v)
        {
            return !Items.Any(x => x.Ingress.Parent == v);
        }

        public int InDegree(IGraphNode v)
        {
            return Items.Count(x => x.Ingress.Parent == v);
        }

        public IEnumerable<ProducibleItem> InEdges(IGraphNode v)
        {
            return Items.Where(x => x.Ingress.Parent == v);
        }

        public bool TryGetInEdges(IGraphNode v, out IEnumerable<ProducibleItem> edges)
        {
            edges = Items.Where(x => x.Ingress.Parent == v);
            return true;
        }

        public ProducibleItem InEdge(IGraphNode v, int index)
        {
            return Items.Where(x => x.Ingress.Parent == v).Skip(index).First();
        }

        public int Degree(IGraphNode v)
        {
            return Items.Count(x => x.Ingress.Parent == v || x.Egress.Parent == v);
        }
    }
}