using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BluePrintAssembler.Annotations;
using BluePrintAssembler.Domain;
using BluePrintAssembler.Utils;
using QuickGraph;

namespace BluePrintAssembler.UI.VM
{
    public class SelectRecipe: BaseFlowNode, ISelectableElement, INotifyPropertyChanged
    {

        public SelectRecipe(IEnumerable<Domain.Recipe> enumerable, BaseProducibleObject result)
        {
            PossibleRecipes = new ObservableCollection<Recipe>(enumerable.Select(x => new Recipe(x)));
            Results = new[] {new RecipeIO(this, new ItemWithAmount {Name = result.Name, Type = result.Type, Amount = 1, Probability = 1})};
        }


        public ObservableCollection<Recipe> PossibleRecipes { get; }


        public override IEnumerable<Edge<IGraphNode>> IngressEdges=>new Edge<IGraphNode>[0];
        public override IEnumerable<Edge<IGraphNode>> EgressEdges => Results.SelectMany(x => x.RelatedItems);
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler<Recipe> RecipeUsed;
        public void UseRecipe(Recipe recipe)
        {
            RecipeUsed?.Invoke(this, recipe);
        }
    }
}
