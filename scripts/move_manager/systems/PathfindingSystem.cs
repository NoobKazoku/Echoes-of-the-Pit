using System.Collections.Generic;
using EchoesOfThePit.scripts.move_manager.models;
using GFramework.Core.extensions;
using GFramework.Core.system;
using GFramework.SourceGenerators.Abstractions.logging;
using Godot;

namespace EchoesOfThePit.scripts.move_manager.systems;

/// <summary>
/// 路径查找系统实现
/// 负责使用A*算法查找路径
/// </summary>
[Log]
public partial class PathfindingSystem : AbstractSystem, IPathfindingSystem
{
    private IGridMapModel _gridMapModel = null!;
    private IGridStateModel _gridStateModel = null!;
    
    /// <summary>
    /// 初始化方法
    /// </summary>
    protected override void OnInit()
    {
        _gridMapModel = this.GetModel<IGridMapModel>()!;
        _gridStateModel = this.GetModel<IGridStateModel>()!;
    }
    
    /// <summary>
    /// 查找从起点到终点的路径
    /// </summary>
    /// <param name="from">起点位置</param>
    /// <param name="to">终点位置</param>
    /// <returns>路径上的位置数组，如果找不到路径返回空数组</returns>
    public Vector2[] FindPath(Vector2 from, Vector2 to)
    {
        // 检查起点和终点是否有效
        if (!_gridMapModel.IsValidGridPosition(from) || !_gridMapModel.IsValidGridPosition(to))
        {
            _log.Debug("路径查找失败: 起点({0},{1})或终点({2},{3})无效", from.X, from.Y, to.X, to.Y);
            return new Vector2[0];
        }
        
        // 获取AStar2D图实例
        var astar = _gridMapModel.GetAStarGraph();
        
        // 获取点ID
        long fromId = _gridMapModel.GetPointId(from);
        long toId = _gridMapModel.GetPointId(to);
        
        _log.Debug("路径查找: 从({0},{1})[ID:{2}] 到 ({3},{4})[ID:{5}]", 
            from.X, from.Y, fromId, to.X, to.Y, toId);
        
        if (fromId == -1 || toId == -1)
        {
            _log.Debug("路径查找失败: 起点ID或终点ID无效");
            return new Vector2[0];
        }
        
        // 使用AStar2D查找路径
        var pointPath = astar.GetPointPath(fromId, toId);
        
        _log.Debug("A*算法返回原始路径: {0}个点", pointPath.Length);
        for (int i = 0; i < pointPath.Length; i++)
        {
            _log.Debug("  点{0}: ({1},{2})", i, pointPath[i].X, pointPath[i].Y);
        }
        
        // 直接返回路径（AStar2D.GetPointPath()已经返回Vector2数组）
        // 但需要过滤掉起点和检查阻挡
        var path = new List<Vector2>();
        for (int i = 0; i < pointPath.Length; i++)
        {
            var position = pointPath[i];
            
            // 跳过起点（已经在玩家位置）
            if (i == 0) continue;
            
            // 检查格子是否被阻挡
            var gridState = _gridStateModel.GetGridState(position);
            if (gridState == IGridStateModel.GridState.Blocked)
            {
                // 如果路径上有阻挡，返回空路径
                _log.Debug("路径查找失败: 位置({0},{1})被阻挡", position.X, position.Y);
                return new Vector2[0];
            }
            
            path.Add(position);
        }
        
        _log.Debug("过滤后路径: {0}个点", path.Count);
        for (int i = 0; i < path.Count; i++)
        {
            _log.Debug("  路径点{0}: ({1},{2})", i, path[i].X, path[i].Y);
        }
        
        return path.ToArray();
    }
    
    /// <summary>
    /// 检查路径是否有效（没有阻挡）
    /// </summary>
    /// <param name="path">要检查的路径</param>
    /// <returns>路径是否有效</returns>
    public bool ValidatePath(Vector2[] path)
    {
        if (path == null || path.Length == 0)
        {
            return false;
        }
        
        foreach (var position in path)
        {
            var gridState = _gridStateModel.GetGridState(position);
            if (gridState == IGridStateModel.GridState.Blocked)
            {
                return false;
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// 获取路径长度（格子数）
    /// </summary>
    /// <param name="path">路径</param>
    /// <returns>路径长度</returns>
    public int GetPathLength(Vector2[] path)
    {
        return path?.Length ?? 0;
    }
    
    /// <summary>
    /// 根据点ID获取位置
    /// </summary>
    /// <param name="pointId">点ID</param>
    /// <returns>位置</returns>
    private Vector2 GetPositionFromId(long pointId)
    {
        // 这里需要从GridMapModel获取位置
        // 由于GridMapModel没有提供这个方法，我们需要遍历所有位置
        var allPositions = _gridMapModel.GetAllGridPositions();
        foreach (var position in allPositions)
        {
            if (_gridMapModel.GetPointId(position) == pointId)
            {
                return position;
            }
        }
        
        return Vector2.Zero;
    }
}
