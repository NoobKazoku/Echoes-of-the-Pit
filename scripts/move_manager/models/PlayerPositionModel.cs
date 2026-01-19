using Godot;
using GFramework.Core.model;

namespace EchoesOfThePit.scripts.move_manager.models;

/// <summary>
/// 玩家位置模型实现
/// 负责管理玩家在网格上的位置数据
/// </summary>
public partial class PlayerPositionModel : AbstractModel, IPlayerPositionModel
{
    private Vector2 _position = Vector2.Zero;

    /// <summary>
    /// 初始化方法
    /// </summary>
    protected override void OnInit()
    {
        // 可以在这里初始化默认位置
        _position = Vector2.Zero;
    }

    /// <summary>
    /// 获取玩家当前位置
    /// </summary>
    public Vector2 GetPosition()
    {
        return _position;
    }

    /// <summary>
    /// 设置玩家新位置
    /// </summary>
    /// <param name="position">新位置</param>
    public void SetPosition(Vector2 position)
    {
        var oldPosition = _position;
        _position = position;
        
        // 可以在这里发送位置变更事件
        // this.SendEvent(new PlayerPositionChangedEvent { OldPosition = oldPosition, NewPosition = position });
    }

    /// <summary>
    /// 检查位置是否有效（在网格范围内）
    /// 目前简单实现，后续可以扩展网格边界检查
    /// </summary>
    /// <param name="position">要检查的位置</param>
    /// <returns>是否有效</returns>
    public bool IsValidPosition(Vector2 position)
    {
        // 简单实现：位置不能为负数
        // 后续可以添加网格边界检查
        return position.X >= 0 && position.Y >= 0;
    }
}
