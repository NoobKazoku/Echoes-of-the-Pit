using EchoesOfThePit.scripts.move_manager.commands;
using EchoesOfThePit.scripts.move_manager.events;
using EchoesOfThePit.scripts.move_manager.models;
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
            
            // 创建选择格子命令输入
            var selectInput = new SelectGridCommandInput
            {
                GridNode = this,
                GridPosition = GlobalPosition
            };
            
            // 发送选择格子命令
            this.SendCommand(new SelectGridCommand(selectInput));
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
}
