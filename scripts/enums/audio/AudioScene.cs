namespace EchoesOfThePit.scripts.enums.audio;

/// <summary>
/// 音频场景枚举，定义了游戏中不同场景下的音频状态
/// </summary>
public enum AudioScene
{
    /// <summary>
    /// 无音频场景
    /// </summary>
    None,
    /// <summary>
    /// 启动场景 - 对应启动动画/Logo界面的音频状态
    /// </summary>
    Boot,
    /// <summary>
    /// 主菜单场景 - 对应主菜单界面的音频状态
    /// </summary>
    MainMenu,
    /// <summary>
    /// 游戏中场景 - 对应游戏进行过程中的音频状态
    /// </summary>
    InGame,
    /// <summary>
    /// 暂停场景 - 对应游戏暂停时的音频状态
    /// </summary>
    Pause,
    /// <summary>
    /// 游戏结束场景 - 对应游戏结束界面的音频状态
    /// </summary>
    GameOver,
}

