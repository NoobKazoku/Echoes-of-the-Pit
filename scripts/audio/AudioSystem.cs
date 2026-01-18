using EchoesOfThePit.scripts.enums.audio;
using EchoesOfThePit.scripts.events.audio;
using EchoesOfThePit.scripts.events.menu;
using GFramework.Core.extensions;
using GFramework.Core.system;
using GFramework.SourceGenerators.Abstractions.logging;

namespace EchoesOfThePit.scripts.audio;

/// <summary>
/// 音频系统类，负责管理游戏中的音频状态和场景切换
/// </summary>
[Log]
public sealed partial class AudioSystem : AbstractSystem
{
    /// <summary>
    /// 音频状态模型，用于管理当前音频场景和其他音频相关状态
    /// </summary>
    private IAudioStateModel _audioStateModel = null!;

    /// <summary>
    /// 初始化音频系统，获取音频状态模型并注册主菜单进入事件监听器
    /// </summary>
    protected override void OnInit()
    {
        _audioStateModel = this.GetModel<IAudioStateModel>()!;
        _log.Info("AudioSystem 初始化完成！");

        // 注册主菜单进入事件监听器，处理音频场景切换逻辑
        this.RegisterEvent<EnterMainMenuEvent>(_ =>
        {
            _log.Info("AudioSystem: MainMenu entered");
            _audioStateModel.ChangeScene(AudioScene.MainMenu);

            this.SendEvent(new AudioSceneChangedEvent(AudioScene.MainMenu));
        });
    }
}
