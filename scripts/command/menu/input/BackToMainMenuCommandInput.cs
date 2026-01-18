using GFramework.Core.Abstractions.command;
using Godot;

namespace EchoesOfThePit.scripts.command.menu.input;

/// <summary>
/// 返回主菜单命令的输入参数类
/// </summary>
public sealed class BackToMainMenuCommandInput : ICommandInput
{
    /// <summary>
    /// 执行命令所需的节点引用，用于获取场景树
    /// </summary>
    public required Node Node { get; init; }
}