using System.Threading.Tasks;
using EchoesOfThePit.scripts.command.graphics.input;
using EchoesOfThePit.scripts.setting.interfaces;
using GFramework.Core.command;
using GFramework.Core.extensions;

namespace EchoesOfThePit.scripts.command.graphics;

/// <summary>
/// 更改分辨率命令类，用于处理分辨率更改操作
/// </summary>
/// <param name="input">分辨率更改命令输入参数</param>
public sealed class ChangeResolutionCommand(ChangeResolutionCommandInput input)
    : AbstractAsyncCommand<ChangeResolutionCommandInput>(input)
{
    /// <summary>
    /// 执行分辨率更改命令
    /// </summary>
    /// <param name="input">分辨率更改命令输入参数，包含新的宽度和高度值</param>
    protected override async Task OnExecuteAsync(ChangeResolutionCommandInput input)
    {
        var model = this.GetModel<ISettingsModel>()!;
        model.Graphics.ResolutionWidth = input.Width;
        model.Graphics.ResolutionHeight = input.Height;

        await this.GetSystem<ISettingsSystem>()!.ApplyGraphics().ConfigureAwait(false);
    }
}