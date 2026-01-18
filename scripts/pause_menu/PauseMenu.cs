using EchoesOfThePit.scripts.command.game;
using EchoesOfThePit.scripts.command.game.input;
using EchoesOfThePit.scripts.command.menu;
using EchoesOfThePit.scripts.command.menu.input;
using EchoesOfThePit.scripts.events.menu;
using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;

namespace EchoesOfThePit.scripts.pause_menu;

[ContextAware]
[Log]
public partial class PauseMenu : Control, IController
{
    /// <summary>
    /// 获取恢复游戏按钮节点
    /// </summary>
    private Button ResumeButton => GetNode<Button>("%ResumeButton");

    /// <summary>
    /// 获取保存游戏按钮节点
    /// </summary>
    private Button SaveButton => GetNode<Button>("%SaveButton");

    /// <summary>
    /// 获取加载游戏按钮节点
    /// </summary>
    private Button LoadButton => GetNode<Button>("%LoadButton");

    /// <summary>
    /// 获取选项按钮节点
    /// </summary>
    private Button OptionsButton => GetNode<Button>("%OptionsButton");

    /// <summary>
    /// 获取主菜单按钮节点
    /// </summary>
    private Button MainMenuButton => GetNode<Button>("%MainMenuButton");

    /// <summary>
    /// 获取退出游戏按钮节点
    /// </summary>
    private Button QuitButton => GetNode<Button>("%QuitButton");

    /// <summary>
    /// 节点就绪时调用的方法，用于初始化UI和设置事件处理器
    /// </summary>
    public override void _Ready()
    {
        InitializeUi();
        SetupEventHandlers();
    }

    /// <summary>
    /// 设置按钮点击事件处理器
    /// 为各个按钮绑定相应的命令发送逻辑
    /// </summary>
    private void SetupEventHandlers()
    {
        // 绑定恢复游戏按钮点击事件
        ResumeButton.Pressed += () =>
        {
            this.SendEvent<ClosePauseMenuEvent>();
            this.SendCommand(new ResumeGameCommand(new ResumeGameCommandInput { Node = this }));
        };
        // 绑定保存游戏按钮点击事件
        SaveButton.Pressed += () =>
        {
            this.SendEvent<ClosePauseMenuEvent>();
            _log.Debug("保存游戏");
            // this.SendCommand(new SaveGameCommand(new SaveGameCommandInput { Node = this }));
        };
        // 绑定加载游戏按钮点击事件
        LoadButton.Pressed += () => { _log.Debug("加载游戏"); };
        // 绑定选项按钮点击事件
        OptionsButton.Pressed += () =>
        {
            this.SendCommand(new OpenOptionsMenuCommand(new OpenOptionsMenuCommandInput()
            {
                Node = this
            }));
        };

        // 绑定返回主菜单按钮点击事件
        MainMenuButton.Pressed += () =>
        {
            this.SendEvent<ClosePauseMenuEvent>();
            this.SendCommand(new BackToMainMenuCommand(new BackToMainMenuCommandInput { Node = this }));
        };

        // 绑定退出游戏按钮点击事件
        QuitButton.Pressed += () =>
        {
            this.SendEvent<ClosePauseMenuEvent>();
            this.SendCommand(new QuitGameCommand(new QuitGameCommandInput { Node = this }));
        };
    }

    /// <summary>
    /// 初始化用户界面组件
    /// 当前为空实现，可在此方法中进行UI组件的初始化配置
    /// </summary>
    private void InitializeUi()
    {
    }
}