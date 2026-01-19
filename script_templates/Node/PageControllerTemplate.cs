// meta-name: UI页面控制器类模板
// meta-description: 负责管理UI页面场景的生命周期和架构关联
using Godot;
using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.Game.Abstractions.ui;
using GFramework.Godot.ui;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;


[ContextAware]
[Log]
public partial class _CLASS_ :_BASE_,IController,IUiPageBehaviorProvider,IUiPage
{
    /// <summary>
    /// 节点准备就绪时的回调方法
    /// 在节点添加到场景树后调用
    /// </summary>
    public override void _Ready()
    {
        
    }
    /// <summary>
    ///  Ui Key的字符串形式 todo：请指定
    /// </summary>
    private static string UiKeyStr => "";
	/// <summary>
    /// 页面行为实例的私有字段
    /// </summary>
	private IUiPageBehavior? _page;
    
    /// <summary>
    /// 获取页面行为实例，如果不存在则创建新的CanvasItemUiPageBehavior实例
    /// </summary>
    /// <returns>返回IUiPageBehavior类型的页面行为实例</returns>
    public IUiPageBehavior GetPage()
    {
        _page ??= new CanvasItemUiPageBehavior<_BASE_>(this,UiKeyStr);
        return _page;
    }
    public override void _Ready()
    {
		// 当页面功能完善后该代码可以考虑注释
	    CallDeferred(nameof(CheckIfInStack));
    }
    /// <summary>
    /// 检查当前UI是否在路由栈顶，如果不在则将页面推入路由栈，请注意这个方法主要用于在开发过程中，启动当前场景时将当前UI推入路由栈
    /// 页面功能完善后该代码可以考虑注释
    /// </summary>
    private void CheckIfInStack()
    {
	    if (!_uiRouter.IsTop(UiKeyStr))
	    {
		    _uiRouter.Push(GetPage());
	    }
    }
    /// <summary>
    /// 页面进入时调用的方法
    /// </summary>
    /// <param name="param">页面进入参数，可能为空</param>
    public void OnEnter(IUiPageEnterParam? param)
    {
        
    }
	/// <summary>
    /// 页面退出时调用的方法
    /// </summary>
    public void IUiPage.OnExit()
    {
        
    }


    /// <summary>
    /// 页面暂停时调用的方法
    /// </summary>
    public void IUiPage.OnPause()
    {
        
    }

    /// <summary>
    /// 页面恢复时调用的方法
    /// </summary>
    public void IUiPage.OnResume()
    {
        
    }

    /// <summary>
    /// 页面显示时调用的方法
    /// </summary>
    public void IUiPage.OnShow()
    {
       
    }

    /// <summary>
    /// 页面隐藏时调用的方法
    /// </summary>
    public void IUiPage.OnHide()
    {
        
    }
}