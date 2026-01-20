namespace EchoesOfThePit.scripts.data;

public enum ItemType
{
    Consumable,
    Equipment,
    Material,
    Quest,
    Misc
}

public class ItemData
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ItemType Type { get; set; }
    public int MaxStackSize { get; set; } = 99;
}