using EchoesOfThePit.scripts.command.inventory.input;
using EchoesOfThePit.scripts.inventory.systems;
using GFramework.Core.command;
using GFramework.Core.extensions;

namespace EchoesOfThePit.scripts.command.inventory;

/// <summary>
/// 添加物品命令类，用于执行向背包添加物品的操作
/// </summary>
/// <param name="input">添加物品命令输入参数</param>
public sealed class AddItemCommand(AddItemCommandInput input)
    : AbstractCommand<AddItemCommandInput>(input)
{
    /// <summary>
    /// 执行添加物品命令的具体逻辑
    /// </summary>
    /// <param name="input">添加物品命令输入参数，包含要添加的物品数据和数量</param>
    protected override void OnExecute(AddItemCommandInput input)
    {
        var inventorySystem = this.GetSystem<IInventorySystem>()!;
        inventorySystem.AddItem(input.Item, input.Amount);
    }
}
