using EchoesOfThePit.scripts.inventory.models;
using GFramework.Core.Abstractions.command;

namespace EchoesOfThePit.scripts.command.inventory.input;

/// <summary>
/// 添加物品命令输入类，用于传递添加物品到背包所需的参数
/// </summary>
public sealed class AddItemCommandInput : ICommandInput
{
    /// <summary>
    /// 获取或设置要添加的物品数据
    /// </summary>
    public required ItemData Item { get; set; }
    
    /// <summary>
    /// 获取或设置要添加的物品数量，默认为1
    /// </summary>
    public int Amount { get; set; } = 1;
}
