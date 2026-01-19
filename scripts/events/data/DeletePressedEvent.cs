namespace EchoesOfThePit.scripts.events.data;

/// <summary>
/// 表示删除按键事件的数据类
/// </summary>
public sealed class DeletePressedEvent
{
    /// <summary>
    /// 获取或初始化槽位编号
    /// </summary>
    public int Slot { get; init; }
}