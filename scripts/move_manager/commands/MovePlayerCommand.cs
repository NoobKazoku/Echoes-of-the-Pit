using EchoesOfThePit.scripts.move_manager.commands;
using EchoesOfThePit.scripts.move_manager.models;
using EchoesOfThePit.scripts.move_manager.systems;
using GFramework.Core.command;
using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using Godot;

namespace EchoesOfThePit.scripts.move_manager.commands;

/// <summary>
/// 移动玩家命令类，用于执行玩家移动操作
/// </summary>
/// <param name="input">移动玩家命令输入参数</param>
[Log]
public sealed partial class MovePlayerCommand(MovePlayerCommandInput input) : AbstractCommand<MovePlayerCommandInput>(input)
{
    /// <summary>
    /// 执行移动玩家命令的具体逻辑
    /// </summary>
    /// <param name="input">移动玩家命令输入参数，包含目标位置信息</param>
    protected override void OnExecute(MovePlayerCommandInput input)
    {
        // 获取玩家位置模型和移动系统
        var playerPositionModel = this.GetModel<IPlayerPositionModel>()!;
        var movementSystem = this.GetSystem<IMovementSystem>()!;
        
        // 获取玩家当前位置
        var fromPosition = playerPositionModel.GetPosition();
        var toPosition = input.TargetPosition;
        
        _log.Debug("执行移动命令: 从({0},{1})到({2},{3})", 
            fromPosition.X, fromPosition.Y, toPosition.X, toPosition.Y);
        
        // 执行移动
        var success = movementSystem.MovePlayer(fromPosition, toPosition);
        
        if (!success)
        {
            _log.Warn("移动命令执行失败");
            // 可以在这里发送移动失败事件
        }
    }
}
