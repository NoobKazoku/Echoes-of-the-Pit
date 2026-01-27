using EchoesOfThePit.scripts.trade;
using Godot;

public partial class NInventoryPanel : Panel
{
	public static string CUR_SEL_PANEL = "CUR_SEL_PANEL";
	/// <summary>
	/// 节点准备就绪时的回调方法
	/// 在节点添加到场景树后调用
	/// </summary>
	public override void _Ready()
	{
		GuiInput += (@event) => _on_gui_input(@event);
	}

	public override void _Process(double delta)
	{
		if(GetTree().GetFirstNodeInGroup(NItem.CUR_SEL_ITEM) is NItem item)
		{
			item.GlobalPosition = GetGlobalMousePosition();
		}
	}
	/// <summary>
	/// 获取或设置库存面板中的物品
	/// </summary>
	public NItem Item
	{
		get
		{
			if (GetChildCount() > 0)
			{
				return GetChildOrNull<NItem>(0);
			}
			else
			{
				return null;
			}
		}
	}

	/// <summary>
	/// 处理库存面板的GUI输入事件
	/// </summary>
	/// <param name="@event">输入事件对象，包含用户的输入信息</param>
	void _on_gui_input(InputEvent @event)
	{
		// 检查输入事件是否为鼠标按钮事件
		if (@event is InputEventMouseButton mb)
		{
			// 检查是否为鼠标左键按下事件
			if (mb.Pressed && mb.ButtonIndex == MouseButton.Left)
			{
				NItem item = Item;
				NItem selItem = GetTree().GetFirstNodeInGroup(NItem.CUR_SEL_ITEM) as NItem;
				if (selItem != null)
				{
					if (item != null)
					{
						// 交换物品
						NInventoryPanel oriGroupPanel = GetTree().GetFirstNodeInGroup(NInventoryPanel.CUR_SEL_PANEL) as NInventoryPanel;
						if (oriGroupPanel != null)
						{
							selItem.Reparent(oriGroupPanel);
							oriGroupPanel.RemoveFromGroup(NInventoryPanel.CUR_SEL_PANEL);
						}

						NInventoryPanel selParent = selItem.GetParentOrNull<NInventoryPanel>();
						if (selParent != null)
						{
							// 将选中物品移动到当前面板
							selItem.Reparent(this);
							selItem.ZIndex = 0;
							selItem.Position = Size / 2;
							selItem.RemoveFromGroup(NItem.CUR_SEL_ITEM);
							
							// 将当前面板物品移动到原选中物品的父面板
							item.Reparent(selParent);
							item.ZIndex = 0;
							item.Position = selParent.Size / 2;
						}
					}
					else
					{
						// 将选中物品移动到当前面板
						selItem.Reparent(this);
						selItem.ZIndex = 0;
						selItem.Position = Size / 2;
						selItem.RemoveFromGroup(NItem.CUR_SEL_ITEM);
					}
				}
				else
				{
					if (item != null)
					{
						// 选中当前面板的物品
						item.AddToGroup(NItem.CUR_SEL_ITEM, true);
						item.ZIndex = 1;
					}
				}
			}
		}
	}
}
