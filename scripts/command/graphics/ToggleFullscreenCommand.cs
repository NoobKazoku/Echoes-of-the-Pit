using System.Threading.Tasks;
using GFramework.Core.Abstractions.command;
using GFramework.Core.command;
using GFramework.Core.extensions;
using GFrameworkGodotTemplate.scripts.command.graphics.input;
using GFrameworkGodotTemplate.scripts.setting.interfaces;

namespace GFrameworkGodotTemplate.scripts.command.graphics;

/// <summary>
/// 切换全屏模式命令类
/// </summary>
/// <param name="input">切换全屏命令输入参数</param>
public sealed class ToggleFullscreenCommand(ToggleFullscreenCommandInput input)
    : AbstractAsyncCommand<ToggleFullscreenCommandInput>(input)
{
    /// <summary>
    /// 执行切换全屏命令
    /// </summary>
    /// <param name="input">切换全屏命令输入参数</param>
    protected override async Task OnExecuteAsync(ToggleFullscreenCommandInput input)
    {
        var model = this.GetModel<ISettingsModel>()!;
        model.Graphics.Fullscreen = input.Fullscreen;
        await this.GetSystem<ISettingsSystem>()!.ApplyGraphics().ConfigureAwait(false);
    }
}