
using GFramework.Core.Abstractions.command;
using Godot;

namespace EchoesOfThePit.scripts.command.menu.input;

public sealed class OpenOptionsMenuCommandInput : ICommandInput
{
    /// <summary>
    /// 获取或设置用于执行命令的节点对象
    /// </summary>
    public required Node Node { get; init; }
}