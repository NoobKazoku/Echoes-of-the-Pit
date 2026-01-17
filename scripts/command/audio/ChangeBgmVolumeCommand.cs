using GFramework.Core.Abstractions.command;
using GFramework.Core.command;
using GFramework.Core.extensions;
using GFrameworkGodotTemplate.scripts.command.audio.input;
using GFrameworkGodotTemplate.scripts.setting.interfaces;

namespace GFrameworkGodotTemplate.scripts.command.audio;

/// <summary>
/// 更改背景音乐音量命令类，用于处理BGM音量更改操作
/// </summary>
/// <param name="input">背景音乐音量更改命令输入参数</param>
public sealed class ChangeBgmVolumeCommand(ChangeBgmVolumeCommandInput input)
    : AbstractCommand<ChangeBgmVolumeCommandInput>(input)
{
    /// <summary>
    /// 执行背景音乐音量更改命令
    /// </summary>
    /// <param name="input">背景音乐音量更改命令输入参数，包含新的音量值</param>
    protected override void OnExecute(ChangeBgmVolumeCommandInput input)
    {
        var model = this.GetModel<ISettingsModel>()!;
        model.Audio.BgmVolume = input.Volume;

        this.GetSystem<ISettingsSystem>()!.ApplyAudio();
    }
}