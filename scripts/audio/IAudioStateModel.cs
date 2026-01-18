using EchoesOfThePit.scripts.enums.audio;
using GFramework.Core.Abstractions.model;

namespace EchoesOfThePit.scripts.audio;

/// <summary>
/// 音频状态模型接口，用于管理游戏中的音频场景切换和当前音频场景的获取
/// </summary>
public interface IAudioStateModel : IModel
{
    /// <summary>
    /// 切换到指定的音频场景
    /// </summary>
    /// <param name="scene">要切换到的音频场景</param>
    void ChangeScene(AudioScene scene);
    
    /// <summary>
    /// 获取当前的音频场景
    /// </summary>
    AudioScene CurrentScene { get; }
}

