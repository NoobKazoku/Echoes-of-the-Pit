using Godot;
using GFramework.Core.Abstractions.system;

namespace EchoesOfThePit.scripts.move_manager.systems;

/// <summary>
/// 路径查找系统接口
/// 负责使用A*算法查找路径
/// </summary>
public interface IPathfindingSystem : ISystem
{
    /// <summary>
    /// 查找从起点到终点的路径
    /// </summary>
    /// <param name="from">起点位置</param>
    /// <param name="to">终点位置</param>
    /// <returns>路径上的位置数组，如果找不到路径返回空数组</returns>
    Vector2[] FindPath(Vector2 from, Vector2 to);
    
    /// <summary>
    /// 检查路径是否有效（没有阻挡）
    /// </summary>
    /// <param name="path">要检查的路径</param>
    /// <returns>路径是否有效</returns>
    bool ValidatePath(Vector2[] path);
    
    /// <summary>
    /// 获取路径长度（格子数）
    /// </summary>
    /// <param name="path">路径</param>
    /// <returns>路径长度</returns>
    int GetPathLength(Vector2[] path);
}
