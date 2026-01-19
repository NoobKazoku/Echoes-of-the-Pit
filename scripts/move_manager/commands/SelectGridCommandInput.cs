using GFramework.Core.Abstractions.command;
using Godot;

namespace EchoesOfThePit.scripts.move_manager.commands;

/// <summary>
/// 选择格子命令输入参数
/// </summary>
public sealed class SelectGridCommandInput : ICommandInput
{
    /// <summary>
    /// 被选中的格子节点
    /// </summary>
    public required Node GridNode { get; init; }
    
    /// <summary>
    /// 格子位置
    /// </summary>
    public required Vector2 GridPosition { get; init; }
}
