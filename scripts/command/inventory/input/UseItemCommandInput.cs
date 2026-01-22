using GFramework.Core.Abstractions.command;

namespace EchoesOfThePit.scripts.command.inventory.input;

/// <summary>
/// 使用物品命令输入类，用于传递使用物品所需的参数
/// </summary>
public sealed class UseItemCommandInput : ICommandInput
{
    /// <summary>
    /// 获取或设置要使用物品的格子索引
    /// </summary>
    public int SlotIndex { get; set; }
}
