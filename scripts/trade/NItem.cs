using Godot;
using System;

namespace EchoesOfThePit.scripts.trade;

/// <summary>
/// 物品节点类，用于在场景中表示可交互的物品
/// </summary>
public partial class NItem : Area2D
{
	public static string CUR_SEL_ITEM = "CUR_SEL_ITEM";
	/// <summary>
	/// 物品资源引用，包含物品的基本属性和数据
	/// </summary>
	[Export] public CItem mItem;

	/// <summary>
	/// 物品图片显示组件
	/// </summary>
	TextureRect mImg;
	
	// Called when the node enters the scene tree for the first time.
	/// <summary>
	/// 节点进入场景树时调用的方法，用于初始化物品节点
	/// </summary>
	public override void _Ready()
	{
		// 获取物品图片显示节点的引用
		mImg = GetNode<TextureRect>("TextureRect");
		
		// 如果物品资源存在且有有效的图片路径，则加载并设置物品图片
		if (mItem != null && !string.IsNullOrEmpty(mItem.mImg))
			mImg.Texture = ResourceLoader.Load<Texture2D>(mItem.mImg);
		_adjustPos();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	void _adjustPos()
	{
		NInventoryPanel p =	 GetParentOrNull<NInventoryPanel>();
		if (p != null)
		{
			Position = p.Size / 2;
		}
	}

}
