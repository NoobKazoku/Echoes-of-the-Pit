using Godot;

namespace EchoesOfThePit.scripts.move_manager.events;

/// <summary>
/// 格子选中事件
/// 当格子被选中时触发
/// </summary>
public sealed class GridSelectedEvent
{
	/// <summary>
	/// 被选中的格子位置
	/// </summary>
	public Vector2 GridPosition { get; set; }
	
	/// <summary>
	/// 格子实例（可选）
	/// </summary>
	public object GridInstance { get; set; }
}
