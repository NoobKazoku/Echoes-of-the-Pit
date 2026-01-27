using GFramework.Core.Abstractions.model;

namespace EchoesOfThePit.scripts.inventory.models;

/// <summary>
/// 背包模型接口，定义背包数据管理的公开API
/// </summary>
public interface IInventoryModel : IModel
{
    /// <summary>
    /// 背包格子总数
    /// </summary>
    int SlotCount { get; }

    /// <summary>
    /// 获取指定索引的格子
    /// </summary>
    /// <param name="index">格子索引</param>
    /// <returns>格子数据</returns>
    InventorySlot GetSlot(int index);

    /// <summary>
    /// 添加物品到背包
    /// </summary>
    /// <param name="item">物品数据</param>
    /// <param name="amount">数量</param>
    /// <returns>是否成功添加</returns>
    bool AddItem(ItemData item, int amount = 1);

    /// <summary>
    /// 从指定格子移除物品
    /// </summary>
    /// <param name="slotIndex">格子索引</param>
    /// <param name="amount">移除数量</param>
    /// <returns>实际移除的数量</returns>
    int RemoveItem(int slotIndex, int amount = 1);

    /// <summary>
    /// 使用指定格子中的物品
    /// </summary>
    /// <param name="slotIndex">格子索引</param>
    /// <returns>是否成功使用</returns>
    bool UseItem(int slotIndex);

    /// <summary>
    /// 检查背包是否有空位
    /// </summary>
    /// <returns>是否有空位</returns>
    bool HasEmptySlot();

    /// <summary>
    /// 检查背包中是否包含指定物品
    /// </summary>
    /// <param name="itemId">物品ID</param>
    /// <returns>是否包含</returns>
    bool HasItem(string itemId);

    /// <summary>
    /// 获取指定物品的总数量
    /// </summary>
    /// <param name="itemId">物品ID</param>
    /// <returns>总数量</returns>
    int GetItemCount(string itemId);
}
