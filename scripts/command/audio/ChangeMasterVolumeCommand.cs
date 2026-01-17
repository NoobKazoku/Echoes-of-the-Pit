using GFramework.Core.Abstractions.command;
using GFramework.Core.command;
using GFramework.Core.extensions;
using GFrameworkGodotTemplate.scripts.command.audio.input;
using GFrameworkGodotTemplate.scripts.setting.interfaces;

namespace GFrameworkGodotTemplate.scripts.command.audio;

/// <summary>
/// 更改主音量命令类，用于处理主音量更改操作
/// </summary>
/// <param name="input">主音量更改命令输入参数</param>
public sealed class ChangeMasterVolumeCommand(ChangeMasterVolumeCommandInput input)
    : AbstractCommand<ChangeMasterVolumeCommandInput>(input)
{
    /// <summary>
    /// 执行主音量更改命令
    /// </summary>
    /// <param name="input">主音量更改命令输入参数，包含新的音量值</param>
    protected override void OnExecute(ChangeMasterVolumeCommandInput input)
    {
        var model = this.GetModel<ISettingsModel>()!;
        model.Audio.MasterVolume = input.Volume;

        this.GetSystem<ISettingsSystem>()!.ApplyAudio();
    }
}