using EchoesOfThePit.scripts.inventory.models;
using GFramework.Core.Abstractions.system;

namespace EchoesOfThePit.scripts.inventory.systems;

/// <summary>
/// 背包系统接口，定义背包系统的公开API
/// </summary>
public interface IInventorySystem : ISystem
{
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
    /// 随机添加一个物品到背包
    /// </summary>
    void RandomAddItem();

    /// <summary>
    /// 打开背包界面
    /// </summary>
    void OpenInventory();

    /// <summary>
    /// 关闭背包界面
    /// </summary>
    void CloseInventory();
}
