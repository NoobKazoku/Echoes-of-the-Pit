using Godot;
using GFramework.Core.Abstractions.model;

namespace EchoesOfThePit.scripts.move_manager.models;

/// <summary>
/// 玩家位置模型接口
/// 负责管理玩家在网格上的位置数据
/// </summary>
public interface IPlayerPositionModel : IModel
{
    /// <summary>
    /// 获取玩家当前位置
    /// </summary>
    Vector2 GetPosition();
    
    /// <summary>
    /// 设置玩家新位置
    /// </summary>
    /// <param name="position">新位置</param>
    void SetPosition(Vector2 position);
    
    /// <summary>
    /// 检查位置是否有效（在网格范围内）
    /// </summary>
    /// <param name="position">要检查的位置</param>
    /// <returns>是否有效</returns>
    bool IsValidPosition(Vector2 position);
}
