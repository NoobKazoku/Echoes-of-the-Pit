using System;
using EchoesOfThePit.scripts.inventory.models;
using Godot;

namespace EchoesOfThePit.scripts.inventory;

/// <summary>
/// 背包单格UI组件，显示物品图标和堆叠数量
/// </summary>
public partial class InventorySlotUI : PanelContainer
{
	/// <summary>
	/// 格子索引
	/// </summary>
	public int SlotIndex { get; set; }

	/// <summary>
	/// 格子点击事件
	/// </summary>
	public event Action<int>? OnSlotClicked;

	/// <summary>
	/// 格子右键点击事件
	/// </summary>
	public event Action<int>? OnSlotRightClicked;

	private TextureRect Icon => GetNode<TextureRect>("%Icon")!;
	private Label CountLabel => GetNode<Label>("%CountLabel");
	private InventorySlot? currentSlot;

	/// <summary>
	/// 节点准备完成时调用
	/// </summary>
	public override void _Ready()
	{
		// 连接鼠标事件
		GuiInput += OnGuiInput;

		// 初始化为空
		Clear();
	}

	/// <summary>
	/// 处理GUI输入事件
	/// </summary>
	private void OnGuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
		{
			if (mouseButton.ButtonIndex == MouseButton.Left)
			{
				OnSlotClicked?.Invoke(SlotIndex);
			}
			else if (mouseButton.ButtonIndex == MouseButton.Right)
			{
				OnSlotRightClicked?.Invoke(SlotIndex);
			}
		}
	}

	/// <summary>
	/// 设置格子数据
	/// </summary>
	/// <param name="slot">背包格子数据</param>
	public void SetSlot(InventorySlot? slot)
	{
		currentSlot = slot;

		if (slot == null || slot.IsEmpty)
		{
			Clear();
			return;
		}

		// 显示物品图标
		Icon.Texture = slot.Item?.Icon;
		Icon.Visible = true;

		// 显示堆叠数量（大于1时显示）
		if (slot.Count > 1)
		{
			CountLabel.Text = slot.Count.ToString();
			CountLabel.Visible = true;
		}
		else
		{
			CountLabel.Visible = false;
		}
	}

	/// <summary>
	/// 获取当前格子数据
	/// </summary>
	public InventorySlot? GetSlot() => currentSlot;

	/// <summary>
	/// 清空格子显示
	/// </summary>
	public void Clear()
	{
		Icon.Texture = null;
		Icon.Visible = false;
		CountLabel.Text = "";
		CountLabel.Visible = false;
		currentSlot = null;
	}
}
