using EchoesOfThePit.scripts.inventory;
using ItemData = EchoesOfThePit.scripts.inventory.models.ItemData;

namespace EchoesOfThePit.scripts.events.inventory;

/// <summary>
/// 物品使用事件，当背包中的物品被使用时触发
/// </summary>
public sealed class ItemUsedEvent
{
    /// <summary>
    /// 获取或设置物品所在的格子索引
    /// </summary>
    public required int SlotIndex { get; init; }
    
    /// <summary>
    /// 获取或设置被使用的物品数据
    /// </summary>
    public required ItemData Item { get; init; }
}
