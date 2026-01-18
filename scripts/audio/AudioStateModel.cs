using EchoesOfThePit.scripts.enums.audio;
using GFramework.Core.model;

namespace EchoesOfThePit.scripts.audio;

/// <summary>
/// 音频状态模型类，用于管理当前音频场景状态
/// 继承自AbstractModel并实现IAudioStateModel接口
/// </summary>
public class AudioStateModel:AbstractModel,IAudioStateModel
{
    /// <summary>
    /// 获取当前音频场景，初始值为AudioScene.None
    /// </summary>
    public AudioScene CurrentScene { get; private set; } = AudioScene.None;

    /// <summary>
    /// 更改当前音频场景
    /// </summary>
    /// <param name="scene">要切换到的目标音频场景</param>
    public void ChangeScene(AudioScene scene)
    {
        // 如果目标场景与当前场景相同，则直接返回，避免不必要的更新
        if (CurrentScene == scene)
            return;

        CurrentScene = scene;
    }

    /// <summary>
    /// 初始化方法，用于子类重写以执行初始化逻辑
    /// </summary>
    protected override void OnInit()
    {

    }
}
