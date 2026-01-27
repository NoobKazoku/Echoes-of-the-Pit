using EchoesOfThePit.scripts.trade;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class NInventory : Panel
{
	List<NItem> mItems = new List<NItem>();
	GridContainer mItemContainer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		mItemContainer = GetNodeOrNull<GridContainer>("GridContainer");
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	void _on_tab_container_tab_selected(int tabIndex)
	{
		//切换tab页发生
		if(GetTree().GetFirstNodeInGroup(NItem.CUR_SEL_ITEM) is NItem item)
		{
			//先保存父节点
			item.GetParent().AddToGroup(NInventoryPanel.CUR_SEL_PANEL);
			//重置父节点
			item.Reparent(this);
		
			
		}
	}



	void _on_link_button_pressed()
	{
		_orderItems();
	}
	void _orderItems()
	{
		mItems.Clear();
		foreach(NInventoryPanel p in mItemContainer.GetChildren())
		{
			NItem item = p.Item;
			if(item != null)
			{
				mItems.Add(item);
				//分离物品及其父级
				item.GetParent().RemoveChild(item);
			}
				
		}
		//整理
		mItems = mItems.OrderBy(i => i.mItem.mName).ToList();
		int i = 0;
		//放回背包
		foreach(NItem item in mItems)
		{
			NInventoryPanel p = mItemContainer.GetChild<NInventoryPanel>(i);
			p.AddChild(item);
			item.Position = p.Size / 2;
			i++;
		}
	}
}
