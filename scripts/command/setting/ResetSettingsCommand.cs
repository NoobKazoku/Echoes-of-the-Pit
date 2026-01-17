using System.Threading.Tasks;
using GFramework.Core.command;
using GFramework.Core.extensions;
using GFrameworkGodotTemplate.scripts.enums.settings;
using GFrameworkGodotTemplate.scripts.events.settings;
using GFrameworkGodotTemplate.scripts.setting.interfaces;

namespace GFrameworkGodotTemplate.scripts.command.setting;

/// <summary>
/// 重置设置命令类，用于将音频和图形设置恢复为默认值
/// </summary>
/// <param name="input">重置设置命令的输入参数</param>
public sealed class ResetSettingsCommand(EmptyCommandInput input)
    : AbstractAsyncCommand<EmptyCommandInput>(input)
{
    /// <summary>
    /// 执行重置设置命令的逻辑
    /// </summary>
    /// <param name="input">重置设置命令的输入参数</param>
    /// <returns>无返回值</returns>
    protected override Task OnExecuteAsync(EmptyCommandInput input)
    {
        var model = this.GetModel<ISettingsModel>()!;

        // 重置音频设置为默认值
        model.Audio.MasterVolume = 1.0f;
        model.Audio.BgmVolume = 0.8f;
        model.Audio.SfxVolume = 0.8f;
        // 重置图形设置为默认值
        model.Graphics.Fullscreen = true;
        model.Graphics.ResolutionWidth = 1920;
        model.Graphics.ResolutionHeight = 1080;
        this.SendEvent(new SettingsChangedEvent
        {
            Reason = SettingsChangedReason.All,
        });
        return this.GetSystem<ISettingsSystem>()!.ApplyAll();
    }
}
