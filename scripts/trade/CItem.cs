using Godot;

namespace EchoesOfThePit.scripts.trade;

[GlobalClass]
public partial class CItem : Resource
{
	[Export] public string mName;
	[Export(PropertyHint.File)] public string mImg;

	[Export] public int mMoney;    

	/// <summary>
	/// 获取物品的提示信息
	/// </summary>
	/// <returns>物品的提示信息，默认返回物品名称</returns>
	public virtual string toolTip()
	{
		return mName;
	}
	
	/// <summary>
	/// 判断物品是否可以使用
	/// </summary>
	/// <returns>如果物品可以使用返回 true，否则返回 false</returns>
	public virtual bool canUse()
	{
		return false;
	}
	
	/// <summary>
	/// 判断物品是否可以装备
	/// </summary>
	/// <returns>如果物品可以装备返回 true，否则返回 false</returns>
	public virtual bool canEquip()
	{
		return false;
	}
}
