using System.Threading.Tasks;
using GFramework.Core.Abstractions.command;
using GFramework.Core.command;
using GFramework.Core.extensions;
using GFrameworkGodotTemplate.scripts.command.setting.input;
using GFrameworkGodotTemplate.scripts.enums.settings;
using GFrameworkGodotTemplate.scripts.events.settings;
using GFrameworkGodotTemplate.scripts.setting;
using GFrameworkGodotTemplate.scripts.setting.interfaces;

namespace GFrameworkGodotTemplate.scripts.command.setting;

/// <summary>
/// 应用设置数据命令
/// 该命令负责将输入的设置数据应用到设置模型中，并触发系统应用所有设置
/// </summary>
/// <param name="input">应用设置数据命令的输入参数</param>
public sealed class ApplySettingsDataCommand(ApplySettingsDataCommandInput input)
    : AbstractAsyncCommand<ApplySettingsDataCommandInput>(input)
{
    /// <summary>
    /// 执行命令的逻辑
    /// 获取设置模型，将输入的设置数据应用到模型中，然后调用系统应用所有设置
    /// </summary>
    /// <param name="input">应用设置数据命令的输入参数</param>
    protected override async Task OnExecuteAsync(ApplySettingsDataCommandInput input)
    {
        var model = this.GetModel<ISettingsModel>()!;
        var data = input.Settings;

        // 应用音频设置
        model.Audio.MasterVolume = data.Audio.MasterVolume;
        model.Audio.BgmVolume = data.Audio.BgmVolume;
        model.Audio.SfxVolume = data.Audio.SfxVolume;

        // 应用图形设置
        model.Graphics.Fullscreen = data.Graphics.Fullscreen;
        model.Graphics.ResolutionWidth = data.Graphics.ResolutionWidth;
        model.Graphics.ResolutionHeight = data.Graphics.ResolutionHeight;
        this.SendEvent(new SettingsChangedEvent
        {
            Reason = SettingsChangedReason.All,
        });
        await this.GetSystem<ISettingsSystem>()!.ApplyAll().ConfigureAwait(false);
    }
}