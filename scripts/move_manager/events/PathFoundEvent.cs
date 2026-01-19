using Godot;

namespace EchoesOfThePit.scripts.move_manager.events
{
    /// <summary>
    /// 路径找到事件
    /// </summary>
    public class PathFoundEvent
    {
        /// <summary>
        /// 找到的路径
        /// </summary>
        public Vector2[] Path { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">找到的路径</param>
        public PathFoundEvent(Vector2[] path)
        {
            Path = path;
        }
    }
}
