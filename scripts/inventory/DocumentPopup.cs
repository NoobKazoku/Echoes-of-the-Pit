using System;
using EchoesOfThePit.scripts.inventory.models;
using Godot;

namespace EchoesOfThePit.scripts.inventory;

/// <summary>
/// 文档阅读弹窗，显示文档内容
/// </summary>
public partial class DocumentPopup : Control
{
    private Label TitleLabel => GetNode<Label>("%TitleLabel")!;
    private Button CloseButton => GetNode<Button>("%CloseButton")!;
    private RichTextLabel ContentLabel => GetNode<RichTextLabel>("%ContentLabel")!;
    private ColorRect Overlay => GetNode<ColorRect>("%Overlay")!;
    
    /// <summary>
    /// 关闭弹窗事件
    /// </summary>
    public event Action? OnClose;
    
    /// <summary>
    /// 节点准备完成时调用
    /// </summary>
    public override void _Ready()
    {
        CloseButton.Pressed += Close;
        Overlay.GuiInput += OnOverlayInput;
        
        // 初始隐藏
        Visible = false;
    }
    
    /// <summary>
    /// 处理遮罩层点击（关闭弹窗）
    /// </summary>
    private void OnOverlayInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton && 
            mouseButton.Pressed && 
            mouseButton.ButtonIndex == MouseButton.Left)
        {
            Close();
        }
    }
    
    /// <summary>
    /// 显示文档内容
    /// </summary>
    /// <param name="item">文档物品数据</param>
    public void ShowDocument(ItemData item)
    {
        if (!item.IsDocument) return;
        
        TitleLabel.Text = item.Name;
        ContentLabel.Text = item.DocumentContent;
        
        Visible = true;
    }
    
    /// <summary>
    /// 关闭弹窗
    /// </summary>
    public void Close()
    {
        Visible = false;
        OnClose?.Invoke();
    }
    
    /// <summary>
    /// 处理输入事件（ESC关闭）
    /// </summary>
    public override void _Input(InputEvent @event)
    {
        if (!Visible) return;
        
        if (@event.IsActionPressed("ui_cancel"))
        {
            Close();
            GetViewport().SetInputAsHandled();
        }
    }
}
