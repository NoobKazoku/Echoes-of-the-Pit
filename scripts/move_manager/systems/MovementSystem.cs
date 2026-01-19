using EchoesOfThePit.scripts.move_manager.events;
using EchoesOfThePit.scripts.move_manager.models;
using GFramework.Core.extensions;
using GFramework.Core.system;
using GFramework.SourceGenerators.Abstractions.logging;
using Godot;

namespace EchoesOfThePit.scripts.move_manager.systems;

/// <summary>
/// 移动系统实现
/// 负责处理玩家移动逻辑
/// </summary>
[Log]
public sealed partial class MovementSystem : AbstractSystem, IMovementSystem
{
    private IPlayerPositionModel _playerPositionModel = null!;
    private IGridStateModel _gridStateModel = null!;
    private IGridMapModel _gridMapModel = null!;
    
    /// <summary>
    /// 移动距离（格子大小），默认为64像素
    /// </summary>
    private const float MOVE_DISTANCE = 64f;

    /// <summary>
    /// 初始化方法
    /// </summary>
    protected override void OnInit()
    {
        _playerPositionModel = this.GetModel<IPlayerPositionModel>()!;
        _gridStateModel = this.GetModel<IGridStateModel>()!;
        _gridMapModel = this.GetModel<IGridMapModel>()!;
        
        _log.Info("MovementSystem 初始化完成！");
    }

    /// <summary>
    /// 验证移动是否合法
    /// </summary>
    /// <param name="fromPosition">起始位置</param>
    /// <param name="toPosition">目标位置</param>
    /// <returns>是否合法</returns>
    public bool ValidateMove(Vector2 fromPosition, Vector2 toPosition)
    {
        // 检查距离是否为64像素（一个格子的距离）
        var distance = (toPosition - fromPosition).Length();
        var isValid = Mathf.Abs(distance - MOVE_DISTANCE) < 0.1f;
        
        _log.Debug("移动验证: 从({0},{1})到({2},{3}), 距离={4}, 有效={5}", 
            fromPosition.X, fromPosition.Y, toPosition.X, toPosition.Y, distance, isValid);
        
        return isValid;
    }

    /// <summary>
    /// 执行移动
    /// </summary>
    /// <param name="fromPosition">起始位置</param>
    /// <param name="toPosition">目标位置</param>
    /// <returns>移动是否成功</returns>
    public bool MovePlayer(Vector2 fromPosition, Vector2 toPosition)
    {
        // 验证移动
        if (!ValidateMove(fromPosition, toPosition))
        {
            _log.Warn("移动失败: 无效的移动距离");
            return false;
        }

        // 检查目标位置是否有效（使用网格地图模型验证位置是否在网格中）
        if (!_gridMapModel.IsValidGridPosition(toPosition))
        {
            _log.Warn("移动失败: 目标位置不在网格地图中");
            return false;
        }

        // 新增：检查目标格子是否被阻挡
        var targetGridState = _gridStateModel.GetGridState(toPosition);
        if (targetGridState == IGridStateModel.GridState.Blocked)
        {
            _log.Warn("移动失败: 目标格子被阻挡");
            return false;
        }

        // 更新玩家位置模型
        _playerPositionModel.SetPosition(toPosition);
        
        // 更新格子状态
        _gridStateModel.SetGridState(fromPosition, IGridStateModel.GridState.Normal);
        _gridStateModel.SetGridState(toPosition, IGridStateModel.GridState.Selected);
        
        // 重新计算可移动格子
        var movableGrids = CalculateMovableGrids(toPosition);
        _gridStateModel.SetMovableGrids(movableGrids);
        
        // 发送玩家移动事件
        this.SendEvent(new PlayerMovedEvent
        {
            TargetPosition = toPosition,
            PreviousPosition = fromPosition
        });
        
        _log.Info("玩家移动成功: 从({0},{1})到({2},{3})", 
            fromPosition.X, fromPosition.Y, toPosition.X, toPosition.Y);
        
        return true;
    }

    /// <summary>
    /// 计算玩家可移动的格子
    /// </summary>
    /// <param name="playerPosition">玩家当前位置</param>
    /// <returns>可移动格子位置数组</returns>
    public Vector2[] CalculateMovableGrids(Vector2 playerPosition)
    {
        // 计算四个方向的相邻格子
        var movableGrids = new Vector2[]
        {
            playerPosition + new Vector2(0, -MOVE_DISTANCE),  // 上
            playerPosition + new Vector2(0, MOVE_DISTANCE),   // 下
            playerPosition + new Vector2(-MOVE_DISTANCE, 0),  // 左
            playerPosition + new Vector2(MOVE_DISTANCE, 0)    // 右
        };

        _log.Debug("计算可移动格子: 玩家位置({0},{1}), 找到{2}个可移动格子", 
            playerPosition.X, playerPosition.Y, movableGrids.Length);
        
        return movableGrids;
    }

    /// <summary>
    /// 获取移动距离（格子大小）
    /// </summary>
    /// <returns>移动距离（像素）</returns>
    public float GetMoveDistance()
    {
        return MOVE_DISTANCE;
    }
}
