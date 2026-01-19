using Godot;

namespace EchoesOfThePit.scripts.move_manager.events
{
    /// <summary>
    /// 路径预测事件
    /// </summary>
    public class PathPredictedEvent
    {
        /// <summary>
        /// 预测的路径
        /// </summary>
        public Vector2[] Path { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">预测的路径</param>
        public PathPredictedEvent(Vector2[] path)
        {
            Path = path;
        }
    }
}
