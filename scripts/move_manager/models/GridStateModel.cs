using System.Collections.Generic;
using Godot;
using GFramework.Core.model;

namespace EchoesOfThePit.scripts.move_manager.models;

/// <summary>
/// 格子状态模型实现
/// 负责管理网格中各个格子的状态
/// </summary>
public partial class GridStateModel : AbstractModel, IGridStateModel
{
    private Dictionary<Vector2, IGridStateModel.GridState> _gridStates = new();
    private List<Vector2> _movableGrids = new();

    /// <summary>
    /// 初始化方法
    /// </summary>
    protected override void OnInit()
    {
        _gridStates.Clear();
        _movableGrids.Clear();
    }

    /// <summary>
    /// 获取指定格子的状态
    /// </summary>
    /// <param name="position">格子位置</param>
    /// <returns>格子状态，如果未记录则返回Normal</returns>
    public IGridStateModel.GridState GetGridState(Vector2 position)
    {
        return _gridStates.TryGetValue(position, out var state) ? state : IGridStateModel.GridState.Normal;
    }

    /// <summary>
    /// 设置指定格子的状态
    /// </summary>
    /// <param name="position">格子位置</param>
    /// <param name="state">新状态</param>
    public void SetGridState(Vector2 position, IGridStateModel.GridState state)
    {
        _gridStates[position] = state;
        
        // 如果状态变为Movable，添加到可移动列表
        if (state == IGridStateModel.GridState.Movable && !_movableGrids.Contains(position))
        {
            _movableGrids.Add(position);
        }
        // 如果状态从Movable变为其他，从可移动列表移除
        else if (state != IGridStateModel.GridState.Movable && _movableGrids.Contains(position))
        {
            _movableGrids.Remove(position);
        }
        
        // 可以在这里发送格子状态变更事件
        // this.SendEvent(new GridStateChangedEvent { Position = position, NewState = state });
    }

    /// <summary>
    /// 获取所有可移动格子的位置
    /// </summary>
    /// <returns>可移动格子位置数组</returns>
    public Vector2[] GetMovableGrids()
    {
        return _movableGrids.ToArray();
    }

    /// <summary>
    /// 设置可移动格子
    /// </summary>
    /// <param name="positions">可移动格子位置数组</param>
    public void SetMovableGrids(Vector2[] positions)
    {
        // 先清除现有的可移动状态
        foreach (var position in _movableGrids.ToArray())
        {
            if (_gridStates.ContainsKey(position) && _gridStates[position] == IGridStateModel.GridState.Movable)
            {
                _gridStates[position] = IGridStateModel.GridState.Normal;
            }
        }
        
        _movableGrids.Clear();
        
        // 设置新的可移动格子
        foreach (var position in positions)
        {
            SetGridState(position, IGridStateModel.GridState.Movable);
        }
    }

    /// <summary>
    /// 清除所有格子的特殊状态（恢复为Normal）
    /// </summary>
    public void ClearAllStates()
    {
        var keys = new List<Vector2>(_gridStates.Keys);
        foreach (var position in keys)
        {
            _gridStates[position] = IGridStateModel.GridState.Normal;
        }
        _movableGrids.Clear();
    }
}
