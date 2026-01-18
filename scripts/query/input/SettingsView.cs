using EchoesOfThePit.scripts.setting;

namespace EchoesOfThePit.scripts.query.input;

/// <summary>
/// 表示游戏设置视图的数据模型，包含音频和显示设置信息
/// </summary>
public sealed class SettingsView
{
    public SettingsData SettingsData { get; init; } = new();
}

