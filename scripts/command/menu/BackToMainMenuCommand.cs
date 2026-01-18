using EchoesOfThePit.scripts.command.menu.input;
using EchoesOfThePit.scripts.core.state.impls;
using GFramework.Core.Abstractions.state;
using GFramework.Core.command;
using GFramework.Core.extensions;

namespace EchoesOfThePit.scripts.command.menu;

/// <summary>
/// 返回主菜单命令类，用于处理从当前场景返回到主菜单的逻辑
/// </summary>
/// <param name="input">命令输入参数，包含执行命令所需的节点信息</param>
public sealed class BackToMainMenuCommand(BackToMainMenuCommandInput input)
    : AbstractCommand<BackToMainMenuCommandInput>(input)
{
    /// <summary>
    /// 执行返回主菜单命令的具体逻辑
    /// </summary>
    /// <param name="input">命令输入参数，包含执行命令所需的节点信息</param>
    protected override void OnExecute(BackToMainMenuCommandInput input)
    {
        // 获取场景树并恢复暂停状态，然后切换到主菜单场景
        var tree = input.Node.GetTree();
        tree.Paused = false;

        this.GetSystem<IStateMachineSystem>()!
            .ChangeTo<MainMenuState>();
    }
}