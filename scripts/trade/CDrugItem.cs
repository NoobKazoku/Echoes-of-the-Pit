using Godot;

namespace EchoesOfThePit.scripts.trade;

[GlobalClass]
/// <summary>
/// 药品类，继承自物品基类，用于表示可使用的药品物品
/// </summary>
public partial class CDrugItem : CItem
{
	/// <summary>
	/// 药品恢复的生命值
	/// </summary>
	[Export] public int mHealthRestore;
	
	/// <summary>
	/// 药品恢复的魔法值
	/// </summary>
	[Export] public int mManaRestore;

	/// <summary>
	/// 重写基类方法，药品可以使用
	/// </summary>
	/// <returns>返回 true，表示药品可以使用</returns>
	public override bool canUse()
	{
		return true;
	}
}
