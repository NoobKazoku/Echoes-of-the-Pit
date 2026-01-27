namespace EchoesOfThePit.scripts.inventory.models;

/// <summary>
/// 背包格子类，存储单个格子中的物品数据和堆叠数量
/// </summary>
public class InventorySlot
{
    /// <summary>
    /// 格子中存储的物品数据，null表示空格子
    /// </summary>
    public ItemData? Item { get; private set; }

    /// <summary>
    /// 当前堆叠数量
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// 检查格子是否为空
    /// </summary>
    public bool IsEmpty => Item == null || Count <= 0;

    /// <summary>
    /// 创建一个空的背包格子
    /// </summary>
    public InventorySlot()
    {
        Item = null;
        Count = 0;
    }

    /// <summary>
    /// 创建一个包含指定物品的背包格子
    /// </summary>
    /// <param name="item">物品数据</param>
    /// <param name="count">初始数量</param>
    public InventorySlot(ItemData item, int count = 1)
    {
        Item = item;
        Count = count;
    }

    /// <summary>
    /// 检查是否可以向该格子添加指定物品
    /// </summary>
    /// <param name="item">要添加的物品</param>
    /// <param name="amount">要添加的数量</param>
    /// <returns>如果可以添加返回true</returns>
    public bool CanAdd(ItemData item, int amount = 1)
    {
        // 空格子可以添加任何物品
        if (IsEmpty) return true;

        // 检查是否为相同物品且未达到最大堆叠
        return Item?.Id == item.Id && Count + amount <= item.MaxStack;
    }

    /// <summary>
    /// 向格子添加物品
    /// </summary>
    /// <param name="item">物品数据</param>
    /// <param name="amount">添加数量</param>
    /// <returns>实际添加的数量</returns>
    public int Add(ItemData item, int amount = 1)
    {
        if (IsEmpty)
        {
            Item = item;
            Count = System.Math.Min(amount, item.MaxStack);
            return Count;
        }

        if (Item?.Id != item.Id) return 0;

        var spaceLeft = item.MaxStack - Count;
        var added = System.Math.Min(amount, spaceLeft);
        Count += added;
        return added;
    }

    /// <summary>
    /// 从格子移除指定数量的物品
    /// </summary>
    /// <param name="amount">移除数量</param>
    /// <returns>实际移除的数量</returns>
    public int Remove(int amount = 1)
    {
        if (IsEmpty) return 0;

        var removed = System.Math.Min(amount, Count);
        Count -= removed;

        // 数量为0时清空物品引用
        if (Count <= 0)
        {
            Item = null;
            Count = 0;
        }

        return removed;
    }

    /// <summary>
    /// 清空格子
    /// </summary>
    public void Clear()
    {
        Item = null;
        Count = 0;
    }
}
