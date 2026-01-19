using GFramework.Core.Abstractions.command;
using Godot;

namespace EchoesOfThePit.scripts.move_manager.commands
{
    /// <summary>
    /// 多格移动玩家命令输入参数
    /// </summary>
    public sealed class MovePlayerMultiGridCommandInput : ICommandInput
    {
        /// <summary>
        /// 用于执行操作的节点对象
        /// </summary>
        public required Node Node { get; init; }
        
        /// <summary>
        /// 目标位置
        /// </summary>
        public required Vector2 TargetPosition { get; init; }
        
        /// <summary>
        /// 路径数组
        /// </summary>
        public required Vector2[] Path { get; init; }
    }
}
