using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Input;
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
            SelectProducer = new DelegateCommand<ProducingEntity>(SelectProducerImpl);

            MyRecipe = Configuration.Instance.RawData.Recipes[info.GetString("Name")];
            try
            {
                Producer = (ProducingEntity) info.GetValue("Producer", typeof(ProducingEntity));
                Producers = info.GetInt32("Producers");
            }
            catch
            {
                Producer = null;
                Producers = 1;
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Name",MyRecipe.Name);
            info.AddValue("Producer",Producer);
            info.AddValue("Producers",Producers);
        }

        private Domain.Recipe _myRecipe;
        private NotifyTaskCompletion<Bitmap> _icon;
        private ProducingEntity _producer;
        private IEnumerable<ProducingEntity> _possibleProducers;
        private int _producers = 1;

        public Recipe(Domain.Recipe value)
        {
            MyRecipe = value;
            SelectProducer = new DelegateCommand<ProducingEntity>(SelectProducerImpl);
        }
        
        public ICommand SelectProducer { get; private set; }

        private void SelectProducerImpl(ProducingEntity obj)
        {
            Producer = obj;
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
                if (!string.IsNullOrEmpty(MyRecipe.Category))
                {
                    PossibleProducers = Configuration.Instance.RawData.Assemblers.Select(x => (BaseProducingEntity) x.Value).Concat(Configuration.Instance.RawData.Furnaces.Select(x => (BaseProducingEntity) x.Value))
                        .Where(x => x.CraftingCategories.ContainsValue(MyRecipe.Category)).Select(x => new ProducingEntity(x))
                        .OrderBy(x => x.MyEntity.Name)
                        .ToArray();
                    if (PossibleProducers.Count() == 1)
                        Producer = PossibleProducers.First();
                }

                OnPropertyChanged();
                OnPropertyChanged(nameof(Sources));
                OnPropertyChanged(nameof(Results));
            }
        }

        public NotifyTaskCompletion<Bitmap> Icon => _icon ?? (_icon = new NotifyTaskCompletion<Bitmap>(Configuration.Instance.GetIcon(_myRecipe)));
        /*public override RecipeIO[] Sources { get; private set; }
        public override RecipeIO[] Results { get; private set; }*/
        public override float Speed => (float) Math.Round(Producers * (Producer?.MyEntity.CraftingSpeed ?? 1) / MyRecipe.CurrentMode.BaseProductionTime, 2);
        public override float BaseSpeed => (float) Math.Round((Producer?.MyEntity.CraftingSpeed ?? 1) / MyRecipe.CurrentMode.BaseProductionTime, 2);
        public override IEnumerable<Edge<IGraphNode>> IngressEdges => Sources.SelectMany(x => x.RelatedItems);
        public override IEnumerable<Edge<IGraphNode>> EgressEdges => Results.SelectMany(x => x.RelatedItems);

        public IEnumerable<ProducingEntity> PossibleProducers
        {
            get { return _possibleProducers; }
            private set
            {
                if (Equals(value, _possibleProducers)) return;
                _possibleProducers = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProducerVisibility));
                OnPropertyChanged(nameof(ProducerSelectorVisibility));
            }
        }

        public ProducingEntity Producer
        {
            get { return _producer; }
            set
            {
                if (Equals(value, _producer)) return;
                _producer = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProducerVisibility));
                OnPropertyChanged(nameof(Speed));
                OnPropertyChanged(nameof(BaseSpeed));
                OnPropertyChanged(nameof(ProducerSelectorVisibility));
            }
        }

        public int Producers
        {
            get { return _producers; }
            set
            {
                if (value == _producers) return;
                _producers = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Speed));
            }
        }

        public Visibility ProducerVisibility => Producer != null?Visibility.Visible:Visibility.Collapsed;
        public Visibility ProducerSelectorVisibility => (PossibleProducers?.Any()??false) && Producer == null ? Visibility.Visible : Visibility.Collapsed;
    }
}