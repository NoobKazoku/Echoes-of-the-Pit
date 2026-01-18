using EchoesOfThePit.scripts.core.constants;
using EchoesOfThePit.scripts.enums.ui;
using GFramework.Core.Abstractions.state;
using GFramework.Core.extensions;
using GFramework.Core.state;
using GFramework.Game.Abstractions.ui;

namespace EchoesOfThePit.scripts.core.state.impls;

/// <summary>
/// 主菜单状态
/// 负责管理主菜单界面的显示和隐藏逻辑
/// </summary>
public class MainMenuState : ContextAwareStateBase
{
    /// <summary>
    /// 状态进入时的处理方法
    /// </summary>
    /// <param name="from">从哪个状态切换过来，可能为空</param>
    public override void OnEnter(IState? from)
    {
        // 推送主菜单UI到界面栈中，显示主菜单界面
        this.GetSystem<IUiRouter>()!.Push(nameof(UiKey.MainMenu));
    }

    /// <summary>
    /// 状态退出时的处理方法
    /// </summary>
    /// <param name="to">将要切换到的目标状态，可能为空</param>
    public override void OnExit(IState? to)
    {
        // 从界面栈中弹出当前UI，隐藏主菜单界面
        this.GetSystem<IUiRouter>()!.Pop();
    }
    
    /// <summary>
    /// 判断是否可以切换到下一个状态
    /// </summary>
    /// <param name="target">目标状态</param>
    /// <returns>始终返回true，表示可以切换到任意状态</returns>
    public override bool CanTransitionTo(IState target) => true;
}
