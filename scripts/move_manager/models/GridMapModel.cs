using System.Collections.Generic;
using Godot;
using GFramework.Core.extensions;
using GFramework.Core.model;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;

namespace EchoesOfThePit.scripts.move_manager.models;

/// <summary>
/// 网格地图模型实现
/// 负责管理网格地图数据和A*路径查找图
/// </summary>
[Log]
public partial class GridMapModel : AbstractModel, IGridMapModel
{
	private AStar2D _astar = null!;
	private Dictionary<Vector2, long> _positionToIdMap = new Dictionary<Vector2, long>();
	private Dictionary<long, Vector2> _idToPositionMap = new Dictionary<long, Vector2>();
	private Vector2[] _allGridPositions = new Vector2[0];
	
	/// <summary>
	/// 网格大小（像素）
	/// </summary>
	private const float GRID_SIZE = 64f;
	
	/// <summary>
	/// 初始化网格地图
	/// </summary>
	/// <param name="tileMapLayer">TileMapLayer节点，包含所有网格位置</param>
	public void Initialize(TileMapLayer tileMapLayer)
	{
		_astar = new AStar2D();
		_positionToIdMap.Clear();
		_idToPositionMap.Clear();
		
		// 获取所有网格位置
		var usedCells = tileMapLayer.GetUsedCells();
		_allGridPositions = new Vector2[usedCells.Count];
		
		_log.Debug("TileMapLayer 包含 {0} 个单元格", usedCells.Count);
		
		long pointId = 0;
		foreach (var cell in usedCells)
		{
			var position = tileMapLayer.MapToLocal(cell);
			_allGridPositions[pointId] = position;
			
			// 添加点到A*图
			_astar.AddPoint(pointId, position);
			_positionToIdMap[position] = pointId;
			_idToPositionMap[pointId] = position;
			
			pointId++;
		}
		
		_log.Debug("已添加 {0} 个网格点到A*图", usedCells.Count);
		
		// 连接相邻的点
		ConnectNeighborPoints();
		
		_log.Info("GridMapModel 初始化完成，共 {0} 个网格点", _allGridPositions.Length);
	}
	
	/// <summary>
	/// 连接相邻的点
	/// </summary>
	private void ConnectNeighborPoints()
	{
		int totalConnections = 0;
		
		foreach (var position in _allGridPositions)
		{
			var pointId = GetPointId(position);
			if (pointId == -1) continue;
			
			// 检查四个方向的相邻点
			var neighborOffsets = new Vector2[]
			{
				new Vector2(0, -GRID_SIZE),  // 上
				new Vector2(0, GRID_SIZE),   // 下
				new Vector2(-GRID_SIZE, 0),  // 左
				new Vector2(GRID_SIZE, 0)    // 右
			};
			
			foreach (var offset in neighborOffsets)
			{
				var neighborPosition = position + offset;
				var neighborId = GetPointId(neighborPosition);
				
				if (neighborId != -1)
				{
					_astar.ConnectPoints(pointId, neighborId);
					totalConnections++;
				}
			}
		}
		
		_log.Debug("A*图连接完成，共 {0} 个连接", totalConnections);
	}
	
	/// <summary>
	/// 获取AStar2D图实例
	/// </summary>
	/// <returns>AStar2D图实例</returns>
	public AStar2D GetAStarGraph()
	{
		return _astar;
	}
	
	/// <summary>
	/// 获取网格点的ID
	/// </summary>
	/// <param name="position">网格位置</param>
	/// <returns>点的ID，如果不存在返回-1</returns>
	public long GetPointId(Vector2 position)
	{
		// 由于浮点数精度问题，需要近似匹配
		foreach (var kvp in _positionToIdMap)
		{
			if ((kvp.Key - position).Length() < 1f)
			{
				return kvp.Value;
			}
		}
		return -1;
	}
	
	/// <summary>
	/// 获取所有网格位置
	/// </summary>
	/// <returns>所有网格位置的数组</returns>
	public Vector2[] GetAllGridPositions()
	{
		return _allGridPositions;
	}
	
	/// <summary>
	/// 检查位置是否是有效的网格位置
	/// </summary>
	/// <param name="position">要检查的位置</param>
	/// <returns>是否是有效的网格位置</returns>
	public bool IsValidGridPosition(Vector2 position)
	{
		return GetPointId(position) != -1;
	}
	
	/// <summary>
	/// 获取相邻的网格位置
	/// </summary>
	/// <param name="position">当前位置</param>
	/// <returns>相邻位置数组</returns>
	public Vector2[] GetNeighborPositions(Vector2 position)
	{
		var pointId = GetPointId(position);
		if (pointId == -1) return new Vector2[0];
		
		var connectedIds = _astar.GetPointConnections(pointId);
		var neighbors = new Vector2[connectedIds.Length];
		
		for (int i = 0; i < connectedIds.Length; i++)
		{
			neighbors[i] = _idToPositionMap[connectedIds[i]];
		}
		
		return neighbors;
	}
	
	/// <summary>
	/// 初始化方法
	/// </summary>
	protected override void OnInit()
	{
		_log.Info("GridMapModel 初始化开始");
	}
}
