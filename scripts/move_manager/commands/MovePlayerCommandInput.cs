using GFramework.Core.Abstractions.command;
using Godot;

namespace EchoesOfThePit.scripts.move_manager.commands;

/// <summary>
/// 移动玩家命令输入参数
/// </summary>
public sealed class MovePlayerCommandInput : ICommandInput
{
    /// <summary>
    /// 目标位置
    /// </summary>
    public required Vector2 TargetPosition { get; init; }
    
    /// <summary>
    /// 格子实例（可选）
    /// </summary>
    public object? Grid { get; init; }
}
