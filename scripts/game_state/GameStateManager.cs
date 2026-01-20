using System;
using System.Collections.Generic;
using EchoesOfThePit.scripts.game_state.interfaces;
using GFramework.Core.model;

namespace EchoesOfThePit.scripts.game_state;

/// <summary>
/// 游戏状态管理器，负责管理玩家等级、经验值、当前场景和游戏标志等全局游戏状态数据
/// </summary>
public class GameStateManager : AbstractModel, IGameStateManager
{
    /// <summary>
    /// 获取或设置玩家当前等级
    /// </summary>
    public int PlayerLevel { get; set; } = 1;

    /// <summary>
    /// 获取或设置玩家当前经验值
    /// </summary>
    public int PlayerExp { get; set; }

    /// <summary>
    /// 获取或设置当前游戏场景名称
    /// </summary>
    public string CurrentScene { get; set; } = "main_menu";

    /// <summary>
    /// 获取游戏标志字典，用于存储各种布尔标志和其他类型的临时数据
    /// </summary>
    public IDictionary<string, object> GameFlags { get; } = new Dictionary<string, object>(StringComparer.Ordinal);

    /// <summary>
    /// 设置指定键的游戏标志值
    /// </summary>
    /// <param name="key">标志的键名</param>
    /// <param name="value">要设置的值</param>
    public void SetFlag(string key, object value)
    {
        if (string.IsNullOrEmpty(key)) return;
        GameFlags[key] = value;
    }

    /// <summary>
    /// 获取指定键的布尔类型标志值
    /// </summary>
    /// <param name="key">标志的键名</param>
    /// <returns>如果标志存在且为布尔类型则返回其值，否则返回false</returns>
    public bool GetFlag(string key)
    {
        if (!GameFlags.TryGetValue(key, out var value)) return false;
        if (value is bool boolValue) return boolValue;
        return true;
    }

    /// <summary>
    /// 获取指定键的泛型类型标志值
    /// </summary>
    /// <typeparam name="T">期望返回的数据类型</typeparam>
    /// <param name="key">标志的键名</param>
    /// <returns>如果标志存在且可以转换为目标类型则返回其值，否则返回默认值</returns>
    public T? GetFlag<T>(string key)
    {
        if (!GameFlags.TryGetValue(key, out var value)) return default;
        if (value is T typedValue) return typedValue;
        return default;
    }

    /// <summary>
    /// 检查指定键的游戏标志是否存在
    /// </summary>
    /// <param name="key">要检查的标志键名</param>
    /// <returns>如果标志存在返回true，否则返回false</returns>
    public bool HasFlag(string key)
    {
        return GameFlags.ContainsKey(key);
    }

    /// <summary>
    /// 清空所有游戏标志
    /// </summary>
    public void ClearFlags()
    {
        GameFlags.Clear();
    }

    /// <summary>
    /// 重置游戏状态到初始状态，包括玩家等级、经验值、当前场景和所有游戏标志
    /// </summary>
    public void Reset()
    {
        PlayerLevel = 1;
        PlayerExp = 0;
        CurrentScene = "main_menu";
        GameFlags.Clear();
    }

    /// <summary>
    /// 初始化方法，用于在对象创建时进行初始化操作
    /// </summary>
    protected override void OnInit()
    {
    }
}