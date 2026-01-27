namespace EchoesOfThePit.scripts.events.inventory;

/// <summary>
/// 物品移除事件，当物品从背包中移除时触发
/// </summary>
public sealed class ItemRemovedEvent
{
    /// <summary>
    /// 获取或设置物品所在的格子索引
    /// </summary>
    public required int SlotIndex { get; init; }
    
    /// <summary>
    /// 获取或设置移除的物品数量
    /// </summary>
    public required int Amount { get; init; }
}
