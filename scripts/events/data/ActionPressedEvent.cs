namespace EchoesOfThePit.scripts.events.data;

/// <summary>
/// 表示一个动作按键事件，当某个槽位的动作被按下时触发
/// </summary>
public class ActionPressedEvent
{
    /// <summary>
    /// 获取触发事件的槽位编号
    /// </summary>
    public int Slot { get; init; }
}