using System;
using EchoesOfThePit.scripts.core.constants;
using GFramework.Core.extensions;
using GFramework.Game.Abstractions.ui;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;

namespace EchoesOfThePit.global;

/// <summary>
/// UI画布层根节点，用于管理UI页面的添加和组织
/// 继承自CanvasLayer并实现IUiRoot接口
/// </summary>
[Log]
[ContextAware]
public partial class UiRoot : CanvasLayer, IUiRoot
{
    /// <summary>
    /// 向UI根节点添加UI页面
    /// </summary>
    /// <param name="child">要添加的UI页面行为对象</param>
    public void AddUiPage(IUiPageBehavior child)
    {
        if (child.View is not Node node)
            throw new InvalidOperationException("UIPage View must be a Godot Node");

        var currentParent = node.GetParent();

        if (currentParent == null)
        {
            // 没有父节点，直接添加
            AddChild(node);
            _log.Debug($"Added UI page: {child.Key}");
        }
        else if (currentParent != this)
        {
            // 已有其他父节点，重新挂载
            _log.Debug($"Reparenting UI page from {currentParent.Name} to UiRoot: {child.Key}");
            node.Reparent(this);
        }
        else
        {
            // 已经是UiRoot的子节点，无需操作
            _log.Debug($"UI page already in UiRoot: {child.Key}");
        }
    }

    /// <summary>
    /// 从UI根节点移除UI页面
    /// </summary>
    /// <param name="child">要移除的UI页面行为对象</param>
    public void RemoveUiPage(IUiPageBehavior child)
    {
        if (child.View is Node node)
            RemoveChild(node);
    }

    /// <summary>
    /// Godot节点就绪时的回调方法
    /// 初始化UI层设置、绑定路由根节点，并切换到游戏主菜单状态
    /// </summary>
    public override void _Ready()
    {
        // 设置UI层级为UI根层
        Layer = UiLayers.UiRoot;

        var router = this.GetSystem<IUiRouter>()!;
        router.BindRoot(this);
        this.SendEvent<UiRootReadyEvent>();
    }

    public struct UiRootReadyEvent;
}