using Godot;
using GFramework.Core.Abstractions.model;

namespace EchoesOfThePit.scripts.move_manager.models;

/// <summary>
/// 网格地图模型接口
/// 负责管理网格地图数据和A*路径查找图
/// </summary>
public interface IGridMapModel : IModel
{
    /// <summary>
    /// 初始化网格地图
    /// </summary>
    /// <param name="tileMapLayer">TileMapLayer节点，包含所有网格位置</param>
    void Initialize(TileMapLayer tileMapLayer);
    
    /// <summary>
    /// 获取AStar2D图实例
    /// </summary>
    /// <returns>AStar2D图实例</returns>
    AStar2D GetAStarGraph();
    
    /// <summary>
    /// 获取网格点的ID
    /// </summary>
    /// <param name="position">网格位置</param>
    /// <returns>点的ID，如果不存在返回-1</returns>
    long GetPointId(Vector2 position);
    
    /// <summary>
    /// 获取所有网格位置
    /// </summary>
    /// <returns>所有网格位置的数组</returns>
    Vector2[] GetAllGridPositions();
    
    /// <summary>
    /// 检查位置是否是有效的网格位置
    /// </summary>
    /// <param name="position">要检查的位置</param>
    /// <returns>是否是有效的网格位置</returns>
    bool IsValidGridPosition(Vector2 position);
    
    /// <summary>
    /// 获取相邻的网格位置
    /// </summary>
    /// <param name="position">当前位置</param>
    /// <returns>相邻位置数组</returns>
    Vector2[] GetNeighborPositions(Vector2 position);
}
