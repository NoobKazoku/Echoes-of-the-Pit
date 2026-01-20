using EchoesOfThePit.scripts.command.inventory.input;
using EchoesOfThePit.scripts.inventory.systems;
using GFramework.Core.command;
using GFramework.Core.extensions;

namespace EchoesOfThePit.scripts.command.inventory;

/// <summary>
/// 使用物品命令类，用于执行使用背包中物品的操作
/// </summary>
/// <param name="input">使用物品命令输入参数</param>
public sealed class UseItemCommand(UseItemCommandInput input)
    : AbstractCommand<UseItemCommandInput>(input)
{
    /// <summary>
    /// 执行使用物品命令的具体逻辑
    /// </summary>
    /// <param name="input">使用物品命令输入参数，包含要使用物品的格子索引</param>
    protected override void OnExecute(UseItemCommandInput input)
    {
        var inventorySystem = this.GetSystem<IInventorySystem>()!;
        inventorySystem.UseItem(input.SlotIndex);
    }
}
