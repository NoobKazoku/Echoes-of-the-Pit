using Godot;
using GFramework.Core.Abstractions.controller;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using System;


[ContextAware]
[Log]
public partial class Grid :Area2D,IController
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
		if(@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			_log.Debug("选中格子({0},{1})",GlobalPosition.X,GlobalPosition.Y);
			Gridsprite.Play("pressed");
		}
	}
}
