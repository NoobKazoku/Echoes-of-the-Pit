using System;
using EchoesOfThePit.scripts.core.constants;
using EchoesOfThePit.scripts.core.ui;
using EchoesOfThePit.scripts.enums.ui;
using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.Game.Abstractions.ui;
using GFramework.Godot.ui;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;

namespace EchoesOfThePit.scripts.shop;

[ContextAware]
[Log]
public partial class Shop : Control, IController, IUiPageBehaviorProvider, ISimpleUiPage
{
    /// <summary>
    /// 页面行为实例的私有字段
    /// </summary>
    private IUiPageBehavior? _page;

	/// <summary>
	/// 页面进入时调用的方法
	/// </summary>
	/// <param name="param">页面进入参数，可能为空</param>
	public void OnEnter(IUiPageEnterParam? param)
	{
	}

    /// <summary>
    /// 获取页面行为实例，如果不存在则创建新的CanvasItemUiPageBehavior实例
    /// </summary>
    /// <returns>返回IUiPageBehavior类型的页面行为实例</returns>
    public IUiPageBehavior GetPage()
    {
        _page ??= new CanvasItemUiPageBehavior<Control>(this);
        return _page;
    }

    /// <summary>
    /// 节点准备就绪时的回调方法
    /// 在节点添加到场景树后调用
    /// </summary>
    public override void _Ready()
    {
    }
}
