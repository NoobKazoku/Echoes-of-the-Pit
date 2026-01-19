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
/// 选择格子命令类，用于处理格子选择操作
/// </summary>
/// <param name="input">选择格子命令输入参数</param>
[Log]
public sealed partial class SelectGridCommand(SelectGridCommandInput input) : AbstractCommand<SelectGridCommandInput>(input)
{
    /// <summary>
    /// 执行选择格子命令的具体逻辑
    /// </summary>
    /// <param name="input">选择格子命令输入参数，包含格子节点和位置信息</param>
    protected override void OnExecute(SelectGridCommandInput input)
    {
        // 获取格子状态模型和移动系统
        var gridStateModel = this.GetModel<IGridStateModel>()!;
        var movementSystem = this.GetSystem<IMovementSystem>()!;
        
        _log.Debug("选择格子命令: 位置({0},{1})", 
            input.GridPosition.X, input.GridPosition.Y);
        
        // 更新格子状态为选中
        gridStateModel.SetGridState(input.GridPosition, IGridStateModel.GridState.Selected);
        
        // 发送格子选中事件
        this.SendEvent(new GridSelectedEvent
        {
            GridPosition = input.GridPosition,
            GridInstance = input.GridNode
        });
        
        // 检查是否可以移动玩家到该格子
        var playerPositionModel = this.GetModel<IPlayerPositionModel>()!;
        var playerPosition = playerPositionModel.GetPosition();
        
        // 计算移动距离
        var distance = playerPosition.DistanceTo(input.GridPosition);
        
        // 如果距离正好是一个格子的距离（64像素），则执行移动
        if (Mathf.Abs(distance - 64) < 0.1f)
        {
            // 创建移动命令输入
            var moveInput = new MovePlayerCommandInput
            {
                TargetPosition = input.GridPosition,
                Grid = input.GridNode
            };
            
            // 发送移动命令
            this.SendCommand(new MovePlayerCommand(moveInput));
        }
        else
        {
            _log.Debug("距离({0})不是有效的移动距离(64像素)", distance);
        }
    }
}
