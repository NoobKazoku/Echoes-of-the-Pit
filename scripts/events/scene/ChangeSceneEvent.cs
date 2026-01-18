namespace EchoesOfThePit.scripts.events.scene;

/// <summary>
/// 表示一个场景切换事件，用于触发从当前场景切换到指定目标场景的操作
/// </summary>
/// <remarks>
/// 此类用于在游戏场景之间进行切换，通过SceneKey属性指定目标场景
/// </remarks>
public sealed class ChangeSceneEvent
{
    /// <summary>
    /// 获取或设置要切换到的目标场景的键值
    /// </summary>
    /// <value>场景的唯一标识符</value>
    public required string SceneKey { get; init; }
}