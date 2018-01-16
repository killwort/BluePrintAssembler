using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using BluePrintAssembler.Domain;

namespace BluePrintAssembler.UI.VM
{
    public class SelectItem
    {
        public SelectItem()
        {
            ListView = new ListCollectionView(Configuration.Instance.RawData.Items.Select(x => (BaseProducibleObject) x.Value).Concat(Configuration.Instance.RawData.Fluids.Select(x => x.Value)).Select(x => new ProducibleItemWithAmount(x)).ToList());
            ListView.GroupDescriptions.Add(new PropertyGroupDescription($"{nameof(ProducibleItemWithAmount.MyItem)}.{nameof(BaseProducibleObject.Subgroup)}"));
            ListView.IsLiveGrouping = true;
        }
        public ListCollectionView ListView { get; }

        private string _filter;
        public string Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                if (string.IsNullOrWhiteSpace(_filter))
                    ListView.Filter = o => true;
                else
                    ListView.Filter = o => ((ProducibleItemWithAmount) o).MyItem.Name.ToLower().Contains(_filter.ToLower().Trim());
            }
        }
        public ProducibleItemWithAmount SelectedItem { get; set; }
    }
}
