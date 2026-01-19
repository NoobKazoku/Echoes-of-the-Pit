using System.Collections.Generic;

namespace EchoesOfThePit.scripts.config;

/// <summary>
/// 游戏数据配置，包含角色、敌人和道具的数据结构
/// 用于从 JSON 反序列化游戏用表数据
/// </summary>
public class ConfigData
{
	/// <summary>角色列表</summary>
	public List<CharacterData> Characters { get; set; } = new List<CharacterData>();

	/// <summary>敌人列表</summary>
	public List<EnemyData> Enemies { get; set; } = new List<EnemyData>();

	/// <summary>道具列表</summary>
	public List<ItemData> Items { get; set; } = new List<ItemData>();
}

/// <summary>
/// 角色数据（示例字段：生命值、速度、防御）
/// </summary>
public class CharacterData
{
	public string Id { get; set; }
	public int Health { get; set; }
	public float Speed { get; set; }
	public int Defense { get; set; }
}

/// <summary>
/// 敌人数据（示例字段：生命值、速度、防御）
/// </summary>
public class EnemyData
{
	public string Id { get; set; }
	public int Health { get; set; }
	public float Speed { get; set; }
	public int Defense { get; set; }
}

/// <summary>
/// 道具数据（示例：可以包含增益数值或其它字段）
/// 这里保留基本属性以便扩展
/// </summary>
public class ItemData
{
	public string Id { get; set; }
	public string Name { get; set; }
	public int Health { get; set; }
	public float Speed { get; set; }
	public int Defense { get; set; }
}
