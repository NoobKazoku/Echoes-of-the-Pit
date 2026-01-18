using EchoesOfThePit.scripts.enums.audio;

namespace EchoesOfThePit.scripts.events.audio;

/// <summary>
/// 表示音频场景发生变化时触发的事件记录
/// </summary>
/// <param name="Scene">发生变化后的音频场景</param>
public sealed record AudioSceneChangedEvent(AudioScene Scene);
