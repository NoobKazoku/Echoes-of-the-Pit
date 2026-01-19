using Godot;
using GFramework.Core.Abstractions.controller;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;


[ContextAware]
[Log]
public partial class Player :CharacterBody2D,IController
{
	private TileMapLayer MoveTileMapLayer => GetNode<TileMapLayer>("%MoveTileMapLayer");
	/// <summary>
	/// 节点准备就绪时的回调方法
	/// 在节点添加到场景树后调用
	/// </summary>
	public override void _Ready()
	{
		
	}

	/// <summary>
	/// 移动玩家到相邻格子
	/// </summary>
	public void MoveToGrid(Grid grid)
	{
		if ((grid.GlobalPosition - GlobalPosition).Length() != 64)
		{
			_log.Debug("目标格子不在移动范围内，无法移动");
			return;
		}
		else
		{
			GlobalPosition = grid.GlobalPosition;
			_log.Debug("玩家移动到格子({0},{1})",GlobalPosition.X,GlobalPosition.Y);

		}

	}
}


