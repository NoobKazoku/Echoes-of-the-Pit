using System;
using GFramework.Core.model;

namespace EchoesOfThePit.scripts.inventory.models;

/// <summary>
/// 背包数据模型，管理24个格子的物品存储
/// 继承自AbstractModel，遵循项目架构模式
/// </summary>
public class InventoryModel : AbstractModel, IInventoryModel
{
    /// <summary>
    /// 默认背包格子数量 (6列 x 6行)
    /// </summary>
    private const int DEFAULT_SLOT_COUNT = 36;

    /// <summary>
    /// 背包格子数组
    /// </summary>
    private readonly InventorySlot[] slots;

    /// <summary>
    /// 背包格子总数
    /// </summary>
    public int SlotCount => slots.Length;

    /// <summary>
    /// 构造函数，初始化背包格子
    /// </summary>
    public InventoryModel()
    {
        slots = new InventorySlot[DEFAULT_SLOT_COUNT];
        for (int i = 0; i < DEFAULT_SLOT_COUNT; i++)
        {
            slots[i] = new InventorySlot();
        }
    }

    /// <summary>
    /// 初始化方法
    /// </summary>
    protected override void OnInit()
    {
    }

    /// <summary>
    /// 获取指定索引的格子
    /// </summary>
    public InventorySlot GetSlot(int index)
    {
        if (index < 0 || index >= SlotCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        return slots[index];
    }

    /// <summary>
    /// 添加物品到背包
    /// </summary>
    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;

        int remaining = amount;

        // 首先尝试堆叠到现有格子
        if (item.IsStackable)
        {
            for (int i = 0; i < SlotCount && remaining > 0; i++)
            {
                InventorySlot slot = slots[i];
                if (!slot.IsEmpty && slot.Item?.Id == item.Id && slot.CanAdd(item, remaining))
                {
                    remaining -= slot.Add(item, remaining);
                }
            }
        }

        // 然后放入空格子
        for (int i = 0; i < SlotCount && remaining > 0; i++)
        {
            InventorySlot slot = slots[i];
            if (slot.IsEmpty)
            {
                remaining -= slot.Add(item, remaining);
            }
        }

        return remaining < amount; // 至少添加了一部分
    }

    /// <summary>
    /// 从指定格子移除物品
    /// </summary>
    public int RemoveItem(int slotIndex, int amount = 1)
    {
        if (slotIndex < 0 || slotIndex >= SlotCount) return 0;
        return slots[slotIndex].Remove(amount);
    }

    /// <summary>
    /// 使用指定格子中的物品
    /// </summary>
    public bool UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= SlotCount) return false;

        var slot = slots[slotIndex];
        if (slot.IsEmpty) return false;

        // 使用后减少数量
        slot.Remove(1);
        return true;
    }

    /// <summary>
    /// 检查背包是否有空位
    /// </summary>
    public bool HasEmptySlot()
    {
        for (var i = 0; i < SlotCount; i++)
        {
            if (slots[i].IsEmpty) return true;
        }

        return false;
    }

    /// <summary>
    /// 检查背包中是否包含指定物品
    /// </summary>
    public bool HasItem(string itemId)
    {
        for (var i = 0; i < SlotCount; i++)
        {
            if (!slots[i].IsEmpty && slots[i].Item?.Id == itemId)
                return true;
        }

        return false;
    }

    /// <summary>
    /// 获取指定物品的总数量
    /// </summary>
    public int GetItemCount(string itemId)
    {
        var count = 0;
        for (var i = 0; i < SlotCount; i++)
        {
            if (!slots[i].IsEmpty && slots[i].Item?.Id == itemId)
                count += slots[i].Count;
        }

        return count;
    }
}
