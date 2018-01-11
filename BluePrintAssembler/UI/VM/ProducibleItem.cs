namespace BluePrintAssembler.UI.VM
{
    public class ProducibleItem
    {
        public Domain.BaseProducibleObject MyItem { get; set; }
        public Recipe Egress { get; set; }
        public Recipe Ingress { get; set; }
    }
}