using System;
using System.Collections.Generic;
using EchoesOfThePit.scripts.move_manager.events;
using EchoesOfThePit.scripts.move_manager.models;
using Godot;
using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;

[ContextAware]
[Log]
public partial class Player : CharacterBody2D, IController
{
    private TileMapLayer MoveTileMapLayer => GetNode<TileMapLayer>("%MoveTileMapLayer");
    private AnimatedSprite2D PlayerSprite => GetNode<AnimatedSprite2D>("%AnimatedSprite2D");
    
    /// <summary>
    /// 节点准备就绪时的回调方法
    /// 在节点添加到场景树后调用
    /// </summary>
    public override void _Ready()
    {
        // 注册玩家移动事件监听
        this.RegisterEvent<PlayerMovedEvent>(OnPlayerMoved);
        
        // 初始化玩家位置到模型
        var playerPositionModel = this.GetModel<IPlayerPositionModel>()!;
        playerPositionModel.SetPosition(GlobalPosition);
        
        // 初始化网格地图模型
        InitializeGridMapModel();
    }
    
    /// <summary>
    /// 初始化网格地图模型
    /// </summary>
    private void InitializeGridMapModel()
    {
        try
        {
            var gridMapModel = this.GetModel<IGridMapModel>();
            if (gridMapModel == null)
            {
                _log.Warn("无法获取网格地图模型");
                return;
            }
            
            // 初始化网格地图模型，直接传递TileMapLayer
            gridMapModel.Initialize(MoveTileMapLayer);
            
            _log.Debug("网格地图模型初始化完成");
        }
        catch (Exception ex)
        {
            _log.Error("初始化网格地图模型失败: {0}", ex.Message);
        }
    }
    
    /// <summary>
    /// 处理玩家移动事件
    /// </summary>
    /// <param name="e">玩家移动事件</param>
    private void OnPlayerMoved(PlayerMovedEvent e)
    {
        // 使用补间动画平滑移动到新位置
        var tween = CreateTween();
        tween.TweenProperty(this, "global_position", e.TargetPosition, 0.2f);

        // 根据移动方向更新玩家朝向
        PlayerSprite.FlipH = e.TargetPosition.X < GlobalPosition.X;
        
        _log.Debug("玩家移动到新位置({0},{1})", e.TargetPosition.X, e.TargetPosition.Y);
    }
    
    /// <summary>
    /// 移动玩家到相邻格子（旧方法，保留兼容性）
    /// 注意：现在应该使用Command来执行移动操作
    /// </summary>
    /// <param name="grid">目标格子</param>
    public void MoveToGrid(Grid grid)
    {
        // 检查移动距离是否为64像素
        if ((grid.GlobalPosition - GlobalPosition).Length() != 64)
        {
            _log.Debug("目标格子不在移动范围内，无法移动");
            return;
        }
        
        // 创建移动命令输入
        var moveInput = new EchoesOfThePit.scripts.move_manager.commands.MovePlayerCommandInput
        {
            TargetPosition = grid.GlobalPosition,
            Grid = grid
        };
        
        // 发送移动命令
        this.SendCommand(new EchoesOfThePit.scripts.move_manager.commands.MovePlayerCommand(moveInput));
    }
}
