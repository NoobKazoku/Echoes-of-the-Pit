using EchoesOfThePit.scripts.move_manager.commands;
using EchoesOfThePit.scripts.move_manager.events;
using EchoesOfThePit.scripts.move_manager.models;
using EchoesOfThePit.scripts.move_manager.systems;
using GFramework.Core.command;
using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using Godot;

namespace EchoesOfThePit.scripts.move_manager.commands;

/// <summary>
/// 多格移动玩家命令类，用于执行玩家多格移动操作
/// </summary>
/// <param name="input">多格移动玩家命令输入参数</param>
[Log]
public sealed partial class MovePlayerMultiGridCommand(MovePlayerMultiGridCommandInput input) : AbstractCommand<MovePlayerMultiGridCommandInput>(input)
{
    /// <summary>
    /// 执行多格移动玩家命令的具体逻辑
    /// </summary>
    /// <param name="input">多格移动玩家命令输入参数，包含目标位置和路径信息</param>
    protected override void OnExecute(MovePlayerMultiGridCommandInput input)
    {
        // 获取玩家位置模型和移动系统
        var playerPositionModel = this.GetModel<IPlayerPositionModel>()!;
        var movementSystem = this.GetSystem<IMovementSystem>()!;
        
        // 获取玩家当前位置
        var currentPosition = playerPositionModel.GetPosition();
        
        _log.Debug("执行多格移动命令: 从({0},{1})到({2},{3})，路径长度: {4}", 
            currentPosition.X, currentPosition.Y, 
            input.TargetPosition.X, input.TargetPosition.Y,
            input.Path.Length);
        
        // 调试：记录路径内容
        for (int i = 0; i < input.Path.Length; i++)
        {
            _log.Debug("  路径[{0}]: ({1},{2})", i, input.Path[i].X, input.Path[i].Y);
        }
        
        // 发送路径找到事件
        this.SendEvent(new PathFoundEvent(input.Path));
        
        // 开始协程执行多步移动
        input.Node.GetTree().CreateTween().TweenCallback(Callable.From(() => 
            ExecuteMultiStepMovement(input.Node, currentPosition, input.Path, movementSystem)
        )).SetDelay(0f);
    }
    
    /// <summary>
    /// 执行多步移动的协程逻辑
    /// </summary>
    /// <param name="node">用于获取SceneTree的节点</param>
    /// <param name="startPosition">起始位置</param>
    /// <param name="path">路径数组</param>
    /// <param name="movementSystem">移动系统</param>
    private async void ExecuteMultiStepMovement(Node node, Vector2 startPosition, Vector2[] path, IMovementSystem movementSystem)
    {
        // 如果路径为空，直接返回
        if (path == null || path.Length == 0)
        {
            _log.Warn("多格移动失败: 路径为空");
            return;
        }
        
        // 从第一个路径点开始移动
        var currentPosition = startPosition;
        
        for (int i = 0; i < path.Length; i++)
        {
            var toPosition = path[i];
            
            _log.Debug("移动步骤 {0}: 从({1},{2})到({3},{4})", 
                i + 1, currentPosition.X, currentPosition.Y, toPosition.X, toPosition.Y);
            
            // 执行单步移动
            var success = movementSystem.MovePlayer(currentPosition, toPosition);
            
            if (!success)
            {
                _log.Warn("多格移动步骤 {0} 执行失败，停止移动", i + 1);
                break;
            }
            
            // 更新当前位置
            currentPosition = toPosition;
            
            // 如果不是最后一步，等待0.1秒延迟
            if (i < path.Length - 1)
            {
                await node.GetTree().ToSignal(node.GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
            }
        }
        
        _log.Debug("多格移动命令执行完成");
    }
}
