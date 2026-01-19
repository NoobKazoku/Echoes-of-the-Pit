using Godot;

namespace EchoesOfThePit.scripts.move_manager.models;

/// <summary>
/// 格子状态模型接口
/// 负责管理网格中各个格子的状态
/// </summary>
public interface IGridStateModel : GFramework.Core.Abstractions.model.IModel
{
    /// <summary>
    /// 格子状态枚举
    /// </summary>
    public enum GridState
    {
        Normal,     // 正常状态
        Hover,      // 鼠标悬停
        Selected,   // 被选中
        Movable,    // 可移动
        Blocked     // 被阻挡
    }
    
    /// <summary>
    /// 获取指定格子的状态
    /// </summary>
    /// <param name="gridId">格子标识符（可以使用位置或实例ID）</param>
    /// <returns>格子状态</returns>
    GridState GetGridState(Vector2 position);
    
    /// <summary>
    /// 设置指定格子的状态
    /// </summary>
    /// <param name="gridId">格子标识符</param>
    /// <param name="state">新状态</param>
    void SetGridState(Vector2 position, GridState state);
    
    /// <summary>
    /// 获取所有可移动格子的位置
    /// </summary>
    /// <returns>可移动格子位置数组</returns>
    Vector2[] GetMovableGrids();
    
    /// <summary>
    /// 设置可移动格子
    /// </summary>
    /// <param name="positions">可移动格子位置数组</param>
    void SetMovableGrids(Vector2[] positions);
    
    /// <summary>
    /// 清除所有格子的特殊状态（恢复为Normal）
    /// </summary>
    void ClearAllStates();
}
