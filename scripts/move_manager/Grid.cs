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
    
    // 路径预测相关
    private Vector2[] _lastPredictedPath = Array.Empty<Vector2>();
    private Vector2 _lastMouseOverPosition = Vector2.Zero;
    
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
        
        // 注册路径预测事件监听
        this.RegisterEvent<PathPredictedEvent>(OnPathPredicted);
        
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
        
        _log.Debug("格子({0},{1})被阻挡", GlobalPosition.X, GlobalPosition.Y);
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
        
        _log.Debug("格子({0},{1})恢复为正常", GlobalPosition.X, GlobalPosition.Y);
    }
    private void OnMouseEntered()
    {
        Gridsprite.Play("hover");
        
        // 记录鼠标悬停位置
        _lastMouseOverPosition = GlobalPosition;
        
        // 预测路径
        PredictPath();
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
                
                // 首先预测路径
                PredictPath();
                
                // 然后执行多格移动
                ExecuteMultiGridMove();
            }
        }
    }
    
    /// <summary>
    /// 预测从玩家当前位置到当前格子的路径
    /// </summary>
    private void PredictPath()
    {
        try
        {
            // 获取玩家位置模型
            var playerPositionModel = this.GetModel<IPlayerPositionModel>();
            if (playerPositionModel == null)
            {
                _log.Warn("无法获取玩家位置模型，跳过路径预测");
                return;
            }
            
            // 获取路径查找系统
            var pathfindingSystem = this.GetSystem<IPathfindingSystem>();
            if (pathfindingSystem == null)
            {
                _log.Warn("无法获取路径查找系统，跳过路径预测");
                return;
            }
            
            // 获取玩家当前位置
            var playerPosition = playerPositionModel.GetPosition();
            var targetPosition = GlobalPosition;
            
            // 如果目标位置与玩家当前位置相同，跳过
            if (playerPosition == targetPosition)
            {
                _log.Debug("目标位置与玩家当前位置相同，跳过路径预测");
                return;
            }
            
            // 查找路径
            var path = pathfindingSystem.FindPath(playerPosition, targetPosition);
            
            if (path == null || path.Length == 0)
            {
                _log.Debug("未找到从({0},{1})到({2},{3})的路径", 
                    playerPosition.X, playerPosition.Y, targetPosition.X, targetPosition.Y);
                return;
            }
            
            _log.Debug("预测路径: 从({0},{1})到({2},{3})，路径长度: {4}", 
                playerPosition.X, playerPosition.Y, targetPosition.X, targetPosition.Y, path.Length);
            
            // 发送路径预测事件
            this.SendEvent(new PathPredictedEvent(path));
            
            // 保存预测的路径
            _lastPredictedPath = path;
        }
        catch (Exception ex)
        {
            _log.Error("路径预测失败: {0}", ex.Message);
        }
    }
    
    /// <summary>
    /// 执行多格移动
    /// </summary>
    private void ExecuteMultiGridMove()
    {
        try
        {
            // 检查是否有预测的路径
            if (_lastPredictedPath == null || _lastPredictedPath.Length == 0)
            {
                _log.Warn("没有预测的路径，无法执行多格移动");
                return;
            }
            
            // 创建多格移动命令输入
            var moveInput = new MovePlayerMultiGridCommandInput
            {
                Node = this,
                TargetPosition = GlobalPosition,
                Path = _lastPredictedPath
            };
            
            // 发送多格移动命令
            this.SendCommand(new MovePlayerMultiGridCommand(moveInput));
            
            _log.Debug("发送多格移动命令到({0},{1})，路径长度: {2}", 
                GlobalPosition.X, GlobalPosition.Y, _lastPredictedPath.Length);
        }
        catch (Exception ex)
        {
            _log.Error("执行多格移动失败: {0}", ex.Message);
        }
    }
    
    /// <summary>
    /// 处理路径预测事件
    /// </summary>
    /// <param name="e">路径预测事件</param>
    private void OnPathPredicted(PathPredictedEvent e)
    {
        // 检查这个格子是否在预测的路径上
        var isOnPath = Array.Exists(e.Path, position => position == GlobalPosition);
        
        if (isOnPath)
        {
            // 如果在路径上，播放悬停动画
            Gridsprite.Play("hover");
        }
        else if (_lastMouseOverPosition != GlobalPosition)
        {
            // 如果不在路径上且不是当前鼠标悬停的格子，恢复正常状态
            // 注意：当前鼠标悬停的格子应该保持hover状态
            Gridsprite.Play("normal");
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
        // 当玩家移动后，清除预测的路径
        _lastPredictedPath = Array.Empty<Vector2>();
        
        // 如果这个格子不是当前鼠标悬停的格子，恢复正常状态
        if (_lastMouseOverPosition != GlobalPosition)
        {
            Gridsprite.Play("normal");
        }
        
        _log.Debug("玩家已移动，清除路径预测并更新格子状态");
    }
}
