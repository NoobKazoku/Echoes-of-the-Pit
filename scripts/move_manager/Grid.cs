using System;
using EchoesOfThePit.scripts.move_manager.commands;
using EchoesOfThePit.scripts.move_manager.events;
using EchoesOfThePit.scripts.move_manager.models;
using EchoesOfThePit.scripts.move_manager.systems;
using Godot;
using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;

[ContextAware]
[Log]
public partial class Grid : Area2D, IController
{
	private Area2D GridArea => GetNode<Area2D>("%CollisionShape2D");
	private AnimatedSprite2D Gridsprite => GetNode<AnimatedSprite2D>("%AnimatedSprite2D");
	
	/// <summary>
	/// 节点准备就绪时的回调方法
	/// 在节点添加到场景树后调用
	/// </summary>
	public override void _Ready()
	{
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
		InputEvent += OnInputEvent;
		BodyEntered += OnbodyEntered;
		BodyExited += OnbodyExited;
		
		// 注册格子选中事件监听
		this.RegisterEvent<GridSelectedEvent>(OnGridSelected);
		
		// 注册玩家移动事件监听
		this.RegisterEvent<PlayerMovedEvent>(OnPlayerMoved);
	}
	
	/// <summary>
	/// 节点退出场景时的回调方法
	/// </summary>
	public override void _ExitTree()
	{
		MouseEntered -= OnMouseEntered;
		MouseExited -= OnMouseExited;
		InputEvent -= OnInputEvent;
		BodyEntered -= OnbodyEntered;
		BodyExited -= OnbodyExited;
	}
	
	/// <summary>
	/// 有角色进入，格子被占用无法移动到这里
	/// </summary>
	/// <param name="body"></param>
	private void OnbodyEntered(Node2D body)
	{
		// 当有角色进入时，将格子状态设置为Blocked
		var gridStateModel = this.GetModel<IGridStateModel>()!;
		gridStateModel.SetGridState(GlobalPosition, IGridStateModel.GridState.Blocked);
	}

	/// <summary>
	/// 角色离开格子，格子恢复可移动状态
	/// </summary>
	/// <param name="body"></param>
	private void OnbodyExited(Node2D body)
	{
		// 当角色离开时，将格子状态恢复为Normal
		var gridStateModel = this.GetModel<IGridStateModel>()!;
		gridStateModel.SetGridState(GlobalPosition, IGridStateModel.GridState.Normal);
	}
	private void OnMouseEntered()
	{
		Gridsprite.Play("hover");
	}
	
	private void OnMouseExited()
	{
		Gridsprite.Play("normal");
	}
	
	private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			_log.Debug("选中格子({0},{1})", GlobalPosition.X, GlobalPosition.Y);
			Gridsprite.Play("pressed");
			
			// 获取玩家当前位置
			var playerPositionModel = this.GetModel<IPlayerPositionModel>();
			if (playerPositionModel == null)
			{
				_log.Warn("无法获取玩家位置模型");
				return;
			}
			
			var playerPosition = playerPositionModel.GetPosition();
			var targetPosition = GlobalPosition;
			
			// 计算距离
			var distance = playerPosition.DistanceTo(targetPosition);
			
			// 如果距离正好是一个格子的距离（64像素），则执行单格移动
			if (Mathf.Abs(distance - 64) < 0.1f)
			{
				_log.Debug("距离({0})为单格移动距离，执行单格移动", distance);
				
				// 创建选择格子命令输入（用于单格移动）
				var selectInput = new SelectGridCommandInput
				{
					GridNode = this,
					GridPosition = GlobalPosition
				};
				
				// 发送选择格子命令
				this.SendCommand(new SelectGridCommand(selectInput));
			}
			else
			{
				// 距离超过一个格子，执行多格移动
				_log.Debug("距离({0})超过单格移动距离，执行多格移动到({1},{2})", 
					distance, GlobalPosition.X, GlobalPosition.Y);
				
				// 执行多格移动
				ExecuteMultiGridMove();
			}
		}
	}
	
	
	/// <summary>
	/// 执行多格移动
	/// </summary>
	private void ExecuteMultiGridMove()
	{
		try
		{
			// 获取玩家位置模型
			var playerPositionModel = this.GetModel<IPlayerPositionModel>();
			if (playerPositionModel == null)
			{
				_log.Warn("无法获取玩家位置模型，无法执行多格移动");
				return;
			}
			
			// 获取路径查找系统
			var pathfindingSystem = this.GetSystem<IPathfindingSystem>();
			if (pathfindingSystem == null)
			{
				_log.Warn("无法获取路径查找系统，无法执行多格移动");
				return;
			}
			
			// 获取玩家当前位置
			var playerPosition = playerPositionModel.GetPosition();
			var targetPosition = GlobalPosition;
			
			// 查找路径
			var path = pathfindingSystem.FindPath(playerPosition, targetPosition);
			
			if (path == null || path.Length == 0)
			{
				_log.Warn("未找到从玩家位置到目标位置的路径，无法执行多格移动");
				return;
			}
			
			// 创建多格移动命令输入
			var moveInput = new MovePlayerMultiGridCommandInput
			{
				Node = this,
				TargetPosition = GlobalPosition,
				Path = path
			};
			
			// 发送多格移动命令
			this.SendCommand(new MovePlayerMultiGridCommand(moveInput));
			
			_log.Debug("发送多格移动命令到({0},{1})，路径长度: {2}", 
				GlobalPosition.X, GlobalPosition.Y, path.Length);
		}
		catch (Exception ex)
		{
			_log.Error("执行多格移动失败: {0}", ex.Message);
		}
	}
	
	
	/// <summary>
	/// 处理格子选中事件
	/// </summary>
	/// <param name="e">格子选中事件</param>
	private void OnGridSelected(GridSelectedEvent e)
	{
		// 如果这个格子被选中，更新动画状态
		if (e.GridInstance == this || e.GridPosition == GlobalPosition)
		{
			Gridsprite.Play("pressed");
		}
		else
		{
			// 如果其他格子被选中，恢复这个格子的正常状态
			Gridsprite.Play("normal");
		}
	}
	
	/// <summary>
	/// 处理玩家移动事件
	/// </summary>
	/// <param name="e">玩家移动事件</param>
	private void OnPlayerMoved(PlayerMovedEvent e)
	{
		// 当玩家移动后，恢复格子的正常状态
		Gridsprite.Play("normal");
	}
}
