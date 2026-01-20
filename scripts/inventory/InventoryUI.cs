using System;
using EchoesOfThePit.scripts.command.inventory;
using EchoesOfThePit.scripts.command.inventory.input;
using EchoesOfThePit.scripts.enums;
using EchoesOfThePit.scripts.events.inventory;
using EchoesOfThePit.scripts.inventory.models;
using EchoesOfThePit.scripts.inventory.systems;
using EchoesOfThePit.scripts.inventory.ui;
using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;

namespace EchoesOfThePit.scripts.inventory;

/// <summary>
/// 背包主界面，显示24格网格物品栏
/// </summary>
[ContextAware]
public partial class InventoryUI : PanelContainer
{
    [Export] public PackedScene? SlotScene { get; set; }

    private GridContainer gridContainer = null!;
    private Button closeButton = null!;
    private Button randomAddButton = null!;
    private IInventorySystem? inventorySystem;
    private IInventoryModel? inventoryModel;
    private readonly InventorySlotUI[] slotUis = new InventorySlotUI[36];

    /// <summary>
    /// 关闭界面事件
    /// </summary>
    public event Action? OnClose;

    /// <summary>
    /// 请求显示文档事件
    /// </summary>
    public event Action<models.ItemData>? OnDocumentRequested;

    /// <summary>
    /// 尝试获取系统服务
    /// </summary>
    private void TryGetServices()
    {
        inventoryModel = ContextAwareExtensions.GetModel<IInventoryModel>(this);
        inventorySystem = ContextAwareExtensions.GetSystem<IInventorySystem>(this);

        // 使用框架事件系统订阅背包变更事件
        this.RegisterEvent<InventoryChangedEvent>(_ => RefreshAll());
    }

    /// <summary>
    /// 节点准备完成时调用
    /// </summary>
    public override void _Ready()
    {
        gridContainer = GetNode<GridContainer>("MarginContainer/VBoxContainer/GridContainer");
        closeButton = GetNode<Button>("MarginContainer/VBoxContainer/Header/CloseButton");

        // 获取系统和模型
        TryGetServices();
        // 初始化格子UI
        InitializeSlots();

        models.ItemData? item = ResourceLoader.Load<models.ItemData>("res://scripts/inventory/entity/document_test.tres");
        if (item != null)
        {
            this.SendCommand(new AddItemCommand(new AddItemCommandInput
            {
                Item = item,
                Amount = 1
            }));
        }
        else
        {
            GD.PushError("Failed to load item resource: res://scripts/inventory/entity/document_test.tres");
        }

        closeButton.Pressed += () => OnClose?.Invoke();
    }

    /// <summary>
    /// 初始化格子UI
    /// </summary>
    private void InitializeSlots()
    {
        // 清空现有子节点
        foreach (Node? child in gridContainer.GetChildren())
        {
            child.QueueFree();
        }

        // 创建36个格子
        for (int i = 0; i < slotUis.Length; i++)
        {
            InventorySlotUI slotUi = SlotScene?.Instantiate<InventorySlotUI>() ?? CreateDefaultSlot();
            slotUi.SlotIndex = i;
            slotUi.OnSlotClicked += OnSlotClicked;
            slotUi.OnSlotRightClicked += OnSlotRightClicked;
            gridContainer.AddChild(slotUi);
            slotUis[i] = slotUi;
        }

        RefreshAll();
    }

    /// <summary>
    /// 创建默认格子（无预制体时使用）
    /// </summary>
    private InventorySlotUI CreateDefaultSlot()
    {
        var slot = new InventorySlotUI();
        slot.CustomMinimumSize = new Vector2(64, 64);
        return slot;
    }

    /// <summary>
    /// 刷新所有格子显示
    /// </summary>
    public void RefreshAll()
    {
        if (inventoryModel == null) return;

        for (var i = 0; i < slotUis.Length; i++)
        {
            var slotData = inventoryModel.GetSlot(i);
            slotUis[i].SetSlot(slotData);
        }
    }

    /// <summary>
    /// 处理格子左键点击
    /// </summary>
    private void OnSlotClicked(int slotIndex)
    {
        // 左键点击暂时不做处理，可扩展为选中物品
        GD.Print($"格子 {slotIndex} 被点击");
    }

    /// <summary>
    /// 处理格子右键点击（使用/阅读）
    /// </summary>
    private void OnSlotRightClicked(int slotIndex)
    {
        InventorySlot? slot = inventoryModel?.GetSlot(slotIndex);
        if (slot == null || slot.IsEmpty) return;

        ItemData item = slot.Item!;
        GD.Print($"使用物品: {item.Name}");
        // 文档类物品弹出阅读窗口
        if (item.ItemType == ItemType.Document)
        {
            OnDocumentRequested?.Invoke(item);
            return;
        }

        // 其他物品尝试使用
        inventorySystem?.UseItem(slotIndex);
    }
}
