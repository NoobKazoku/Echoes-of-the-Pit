using Godot;

namespace EchoesOfThePit.scripts.trade;

[GlobalClass]
/// <summary>
/// 武器类，继承自物品基类，用于表示可装备的武器物品
/// </summary>
public partial class CWeaponItem : CItem
{
	/// <summary>
	/// 武器的伤害值
	/// </summary>
	[Export] public float mChangeAttack;
	
	/// <summary>
	/// 武器的防御值
	/// </summary>
	[Export] public float mChangeDefense;
	/// <summary>
	/// 重写基类方法，武器可以装备
	/// </summary>
	/// <returns>返回 true，表示武器可以装备</returns>
	public override bool canEquip()
	{
		return true;
	}
}
