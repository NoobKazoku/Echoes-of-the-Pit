using EchoesOfThePit.scripts.command.inventory.input;
using EchoesOfThePit.scripts.inventory.systems;
using GFramework.Core.command;
using GFramework.Core.extensions;

namespace EchoesOfThePit.scripts.command.inventory;

/// <summary>
/// 移除物品命令类，用于执行从背包移除物品的操作
/// </summary>
/// <param name="input">移除物品命令输入参数</param>
public sealed class RemoveItemCommand(RemoveItemCommandInput input)
    : AbstractCommand<RemoveItemCommandInput>(input)
{
    /// <summary>
    /// 执行移除物品命令的具体逻辑
    /// </summary>
    /// <param name="input">移除物品命令输入参数，包含格子索引和移除数量</param>
    protected override void OnExecute(RemoveItemCommandInput input)
    {
        var inventorySystem = this.GetSystem<IInventorySystem>()!;
        inventorySystem.RemoveItem(input.SlotIndex, input.Amount);
    }
}
