using System;
using GFramework.Core.Abstractions.controller;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;

namespace EchoesOfThePit.scripts.component;

/// <summary>
/// 确认对话框组件，用于显示确认消息并获取用户选择
/// </summary>
[ContextAware]
[Log]
public partial class ConfirmationDialog : Control, IController
{
    [Signal]
    public delegate void CancelledEventHandler();

    [Signal]
    public delegate void ConfirmedEventHandler();

    private Action? _cancelCallback;

    private Action? _confirmCallback;
    private Label MessageLabel => GetNode<Label>("%MessageLabel");
    private Button ConfirmButton => GetNode<Button>("%ConfirmButton");
    private Button CancelButton => GetNode<Button>("%CancelButton");

    public override void _Ready()
    {
        ConfirmButton.Pressed += OnConfirmButtonPressed;
        CancelButton.Pressed += OnCancelButtonPressed;
        Visible = false;
    }

    public void ShowDialog(string message, Action? confirmCallback = null, Action? cancelCallback = null)
    {
        MessageLabel.Text = message;
        _confirmCallback = confirmCallback;
        _cancelCallback = cancelCallback;
        Visible = true;
    }

    public void HideDialog()
    {
        Visible = false;
        _confirmCallback = null;
        _cancelCallback = null;
    }

    private void OnConfirmButtonPressed()
    {
        HideDialog();
        EmitSignalConfirmed();
        _confirmCallback?.Invoke();
    }

    private void OnCancelButtonPressed()
    {
        HideDialog();
        EmitSignalCancelled();
        _cancelCallback?.Invoke();
    }
}