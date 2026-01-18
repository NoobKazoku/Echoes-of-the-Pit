using EchoesOfThePit.scripts.command.setting;
using EchoesOfThePit.scripts.command.setting.input;
using EchoesOfThePit.scripts.core;
using EchoesOfThePit.scripts.core.environment;
using EchoesOfThePit.scripts.core.state.impls;
using EchoesOfThePit.scripts.setting.interfaces;
using GFramework.Core.Abstractions.architecture;
using GFramework.Core.Abstractions.logging;
using GFramework.Core.Abstractions.properties;
using GFramework.Core.Abstractions.state;
using GFramework.Core.architecture;
using GFramework.Core.extensions;
using GFramework.Godot.logging;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;

namespace EchoesOfThePit.global;

/// <summary>
/// 游戏入口点节点类，负责初始化游戏架构和管理全局游戏状态
/// </summary>
[Log]
[ContextAware]
public partial class GameEntryPoint : Node
{
    public static IArchitecture Architecture { get; private set; } = null!;
    private ISettingsStorageUtility _settingsStorageUtility = null!;
    private ISettingsModel _settingsModel = null!;
    [Export] public bool IsDev { get; set; } = true;

    /// <summary>
    /// Godot引擎调用的节点就绪方法，在此方法中初始化游戏架构和相关组件
    /// </summary>
    public override void _Ready()
    {
        // 创建并初始化游戏架构实例
        // 配置架构的日志记录属性，设置Godot日志工厂提供程序并指定最低日志级别为调试级别
        // 然后初始化架构实例以准备游戏运行环境
        Architecture = new GameArchitecture(new ArchitectureConfiguration
        {
            LoggerProperties = new LoggerProperties
            {
                LoggerFactoryProvider = new GodotLoggerFactoryProvider
                {
                    MinLevel = LogLevel.Debug,
                },
            },
        }, IsDev ? new GameDevEnvironment() : new GameMainEnvironment());
        Architecture.Initialize();
        _settingsStorageUtility = this.GetUtility<ISettingsStorageUtility>()!;
        _settingsModel = this.GetModel<ISettingsModel>()!;
        var data = _settingsStorageUtility.Load();
        _ = this.SendCommandAsync(new ApplySettingsDataCommand(new ApplySettingsDataCommandInput
        {
            Settings = data,
        })).ConfigureAwait(false);
        _log.Info("设置已加载");
        if (!IsDev)
        {
            this.RegisterEvent<UiRoot.UiRootReadyEvent>(_ =>
            {
                // 创建并切换到游戏主菜单状态
                this.GetSystem<IStateMachineSystem>()!
                    .ChangeTo<MainMenuState>();
            });
        }

        _log.Debug("GameEntryPoint ready.");
    }
    /// <summary>
    /// 当节点从场景树中移除时调用，销毁游戏架构
    /// </summary>
    /// <returns>无返回值</returns>
    public override void _ExitTree()
    {
        _settingsStorageUtility.Save(_settingsModel.GetSettingsData());
    }
}