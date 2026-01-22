using EchoesOfThePit.scripts.inventory;
using ItemData = EchoesOfThePit.scripts.inventory.models.ItemData;

namespace EchoesOfThePit.scripts.events.inventory;

/// <summary>
/// 物品添加事件，当物品被添加到背包时触发
/// </summary>
public sealed class ItemAddedEvent
{
    /// <summary>
    /// 获取或设置被添加的物品数据
    /// </summary>
    public required ItemData Item { get; init; }
    
    /// <summary>
    /// 获取或设置添加的物品数量
    /// </summary>
    public required int Amount { get; init; }
}
