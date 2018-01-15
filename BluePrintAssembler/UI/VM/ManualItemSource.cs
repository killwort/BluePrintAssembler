using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using BluePrintAssembler.Domain;
using BluePrintAssembler.Utils;
using QuickGraph;

namespace BluePrintAssembler.UI.VM
{
    [Serializable]
    public class ManualItemSource:BaseFlowNode,ISelectableElement,IAddableToFactory
    {
        private NotifyTaskCompletion<Bitmap> _icon;

        public ManualItemSource(BaseProducibleObject result)
        {
            MyItem = result;
            Results = new RecipeIO[] {new RecipeIO(this, new ItemWithAmount {Name = result.Name, Type = result.Type, Amount = 1, Probability = 1})};
        }

        public BaseProducibleObject MyItem { get; }
        public override IEnumerable<Edge<IGraphNode>> IngressEdges=>new Edge<IGraphNode>[0];
        public override IEnumerable<Edge<IGraphNode>> EgressEdges => Results.SelectMany(x => x.RelatedItems);
        public NotifyTaskCompletion<Bitmap> Icon => _icon ?? (_icon = new NotifyTaskCompletion<Bitmap>(Configuration.Instance.GetIcon(MyItem)));

        public event EventHandler<BaseProducibleObject> AddedToFactory;

        public void AddToFactory()
        {
            AddedToFactory?.Invoke(this, Results.First().RealItem);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Type",MyItem.Type);
            info.AddValue("Name", MyItem.Name);
        }

        public ManualItemSource(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            MyItem = Configuration.Instance.RawData.Get(info.GetString("Type"), info.GetString("Name"));
            Results = new RecipeIO[] { new RecipeIO(this, new ItemWithAmount { Name = MyItem.Name, Type = MyItem.Type, Amount = 1, Probability = 1 }) };
        }
    }
}