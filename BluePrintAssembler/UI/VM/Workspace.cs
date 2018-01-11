using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using BluePrintAssembler.Domain;

namespace BluePrintAssembler.UI.VM
{
    public class Workspace
    {
        public CompositeCollection RenderableElements => new CompositeCollection
        {
            new CollectionContainer{Collection = Recipes},
            new CollectionContainer{Collection = Items},
            new CollectionContainer{Collection = UnspecifiedRecipies}
        };
        public ObservableCollection<Recipe> Recipes { get; }=new ObservableCollection<Recipe>();
        public ObservableCollection<ProducibleItem> Items { get; }=new ObservableCollection<ProducibleItem>();
        public ObservableCollection<RecipeSelector> UnspecifiedRecipies { get; } = new ObservableCollection<RecipeSelector>();
        public ObservableCollection<ManualItemSource> ManualItemSources { get; } = new ObservableCollection<ManualItemSource>();

        public void Test()
        {
            var rawData = Configuration.Instance.RawData;
            var satisfied=new HashSet<BaseProducibleObject>();
            var satisfiedRaw=new HashSet<BaseProducibleObject>();
            var unsatisfied=new HashSet<BaseProducibleObject>{rawData.Items["iron-plate"]};
            while (unsatisfied.Any())
            {
                var result = unsatisfied.First();
                var possibleRecipies=rawData.Recipes.Where(x => x.Value.HasResult(result)).ToArray();
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
                        var res=rawData.Get(r.Value.Type, r.Value.Name);
                        satisfied.Add(res);
                        unsatisfied.Remove(res);
                    }
                    foreach (var r in possibleRecipies.First().Value.CurrentMode.Sources)
                    {
                        var res=rawData.Get(r.Value.Type, r.Value.Name);
                        if (!satisfied.Contains(res) && !satisfiedRaw.Contains(res))
                        {
                            unsatisfied.Add(res);
                        }
                    }
                }
            }

            foreach (var s in satisfied)
            {
                foreach (var pair in Recipes.Where(x => x.MyRecipe.HasResult(s)).SelectMany(egress => Recipes.Where(x => x.MyRecipe.HasSource(s)).Select(ingress => new ProducibleItem {Egress = egress, Ingress = ingress})))
                {
                    pair.MyItem = s;
                    Items.Add(pair);
                }
            }

            foreach (var u in unsatisfied)
            {
                foreach (var unsatisfiedSlot in Recipes.Where(x => x.MyRecipe.HasResult(u)))
                {
                    ManualItemSources.Add(new ManualItemSource{MyItem = u,Egress=unsatisfiedSlot});
                }
            }
        }
    }
}