using Godot;

namespace EchoesOfThePit.scripts.move_manager.events;

/// <summary>
/// 玩家移动事件
/// 当玩家移动到新位置时触发
/// </summary>
public sealed class PlayerMovedEvent
{
    /// <summary>
    /// 目标位置
    /// </summary>
    public Vector2 TargetPosition { get; set; }
    
    /// <summary>
    /// 之前的位置
    /// </summary>
    public Vector2 PreviousPosition { get; set; }
}
