namespace BluePrintAssembler.Domain
{
    public interface INamed
    {
        string Type { get; }
        string Name { get; }
        LocalisedString LocalisedName { get; }
    }
}