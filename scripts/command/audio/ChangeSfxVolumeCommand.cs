using GFramework.Core.Abstractions.command;
using GFramework.Core.command;
using GFramework.Core.extensions;
using GFrameworkGodotTemplate.scripts.command.audio.input;
using GFrameworkGodotTemplate.scripts.setting.interfaces;

namespace GFrameworkGodotTemplate.scripts.command.audio;

/// <summary>
/// 更改音效音量命令类，用于处理SFX音量更改操作
/// </summary>
/// <param name="input">音效音量更改命令输入参数</param>
public sealed class ChangeSfxVolumeCommand(ChangeSfxVolumeCommandInput input)
    : AbstractCommand<ChangeSfxVolumeCommandInput>(input)
{
    /// <summary>
    /// 执行音效音量更改命令
    /// </summary>
    /// <param name="input">音效音量更改命令输入参数，包含新的音量值</param>
    protected override void OnExecute(ChangeSfxVolumeCommandInput input)
    {
        var model = this.GetModel<ISettingsModel>()!;
        model.Audio.SfxVolume = input.Volume;

        this.GetSystem<ISettingsSystem>()!.ApplyAudio();
    }
}