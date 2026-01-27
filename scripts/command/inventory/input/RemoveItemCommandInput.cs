using GFramework.Core.Abstractions.command;

namespace EchoesOfThePit.scripts.command.inventory.input;

/// <summary>
/// 移除物品命令输入类，用于传递从背包移除物品所需的参数
/// </summary>
public sealed class RemoveItemCommandInput : ICommandInput
{
    /// <summary>
    /// 获取或设置要移除物品的格子索引
    /// </summary>
    public int SlotIndex { get; set; }
    
    /// <summary>
    /// 获取或设置要移除的物品数量，默认为1
    /// </summary>
    public int Amount { get; set; } = 1;
}
