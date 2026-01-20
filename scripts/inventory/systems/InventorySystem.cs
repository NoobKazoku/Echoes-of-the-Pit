using System;
using EchoesOfThePit.scripts.enums;
using EchoesOfThePit.scripts.events.inventory;
using EchoesOfThePit.scripts.inventory.models;
using EchoesOfThePit.scripts.inventory.systems;
using GFramework.Core.extensions;
using GFramework.Core.system;
using GFramework.SourceGenerators.Abstractions.logging;
using Godot;

namespace EchoesOfThePit.scripts.inventory;

/// <summary>
/// 背包系统，处理物品管理和UI交互
/// 继承自AbstractSystem，遵循项目架构模式
/// </summary>
[Log]
public partial class InventorySystem : AbstractSystem, IInventorySystem
{
    private IInventoryModel _inventoryModel = null!;
    private const string ItemEntityPath = "res://scripts/inventory/entity";

    /// <summary>
    /// 初始化方法，获取背包模型
    /// </summary>
    protected override void OnInit()
    {
        _inventoryModel = this.GetModel<IInventoryModel>()!;
    }

    /// <summary>
    /// 添加物品到背包
    /// </summary>
    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null)
        {
            _log.Warn("Attempted to add a null item to inventory.");
            return false;
        }

        var result = _inventoryModel.AddItem(item, amount);
        if (result)
        {
            _log.Info($"添加物品到背包: {item.Name} x{amount}");
            this.SendEvent(new ItemAddedEvent { Item = item, Amount = amount });
            this.SendEvent(new InventoryChangedEvent());
        }
        else
        {
            _log.Warn($"背包已满，无法添加物品: {item.Name}");
        }

        return result;
    }

    /// <summary>
    /// 从指定格子移除物品
    /// </summary>
    public int RemoveItem(int slotIndex, int amount = 1)
    {
        var removed = _inventoryModel.RemoveItem(slotIndex, amount);
        if (removed > 0)
        {
            _log.Info($"从格子 {slotIndex} 移除物品 x{removed}");
            this.SendEvent(new ItemRemovedEvent { SlotIndex = slotIndex, Amount = removed });
            this.SendEvent(new InventoryChangedEvent());
        }

        return removed;
    }

    /// <summary>
    /// 使用指定格子中的物品
    /// </summary>
    public bool UseItem(int slotIndex)
    {
        var slot = _inventoryModel.GetSlot(slotIndex);
        if (slot.IsEmpty) return false;

        var item = slot.Item!;

        // 根据物品类型执行不同的使用逻辑
        switch (item.ItemType)
        {
            case ItemType.Potion:
                // 药水使用后消耗
                if (_inventoryModel.UseItem(slotIndex))
                {
                    _log.Info($"使用药水: {item.Name}");
                    this.SendEvent(new ItemUsedEvent { SlotIndex = slotIndex, Item = item });
                    this.SendEvent(new InventoryChangedEvent());
                    return true;
                }

                break;

            case ItemType.Document:
                // 文档不消耗，只触发阅读事件
                _log.Info($"阅读文档: {item.Name}");
                this.SendEvent(new ItemUsedEvent { SlotIndex = slotIndex, Item = item });
                return true;

            case ItemType.Key:
                // 钥匙暂不实现自动使用，需要在场景中触发
                _log.Info($"检查钥匙: {item.Name}");
                this.SendEvent(new ItemUsedEvent { SlotIndex = slotIndex, Item = item });
                return true;

            case ItemType.Gem:
                // 宝石为收集品，不可主动使用
                _log.Info($"这是收集品，无法使用: {item.Name}");
                break;
        }

        return false;
    }

    /// <summary>
    /// 随机添加一个物品到背包
    /// </summary>
    public void RandomAddItem()
    {
        using DirAccess? dir = DirAccess.Open(ItemEntityPath);
        if (dir == null) return;
        string[] files = dir.GetFiles();
        // 随机获取一个文件加载到库存
        Random random = new Random();
        string randomFile = files[random.Next(files.Length)];
        Resource? itemResource = ResourceLoader.Load($"{ItemEntityPath}/{randomFile}");
        if (itemResource is ItemData itemData)
        {
            AddItem(itemData);
        }
    }

    /// <summary>
    /// 打开背包界面
    /// </summary>
    public void OpenInventory()
    {
        _log.Info("打开背包界面");
        // TODO: 通过UiRouter打开背包UI
    }

    /// <summary>
    /// 关闭背包界面
    /// </summary>
    public void CloseInventory()
    {
        _log.Info("关闭背包界面");
        // TODO: 通过UiRouter关闭背包UI
    }
}
