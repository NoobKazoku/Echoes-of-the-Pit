using Godot;

namespace EchoesOfThePit.scripts.move_manager.systems;

/// <summary>
/// 移动系统接口
/// 负责处理玩家移动逻辑
/// </summary>
public interface IMovementSystem : GFramework.Core.Abstractions.system.ISystem
{
    /// <summary>
    /// 验证移动是否合法
    /// </summary>
    /// <param name="fromPosition">起始位置</param>
    /// <param name="toPosition">目标位置</param>
    /// <returns>是否合法</returns>
    bool ValidateMove(Vector2 fromPosition, Vector2 toPosition);
    
    /// <summary>
    /// 执行移动
    /// </summary>
    /// <param name="fromPosition">起始位置</param>
    /// <param name="toPosition">目标位置</param>
    /// <returns>移动是否成功</returns>
    bool MovePlayer(Vector2 fromPosition, Vector2 toPosition);
    
    /// <summary>
    /// 计算玩家可移动的格子
    /// </summary>
    /// <param name="playerPosition">玩家当前位置</param>
    /// <returns>可移动格子位置数组</returns>
    Vector2[] CalculateMovableGrids(Vector2 playerPosition);
    
    /// <summary>
    /// 获取移动距离（格子大小）
    /// </summary>
    /// <returns>移动距离（像素）</returns>
    float GetMoveDistance();
}
