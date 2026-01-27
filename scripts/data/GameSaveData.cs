using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EchoesOfThePit.scripts.data;

/// <summary>
/// 游戏存档数据类，用于存储玩家的游戏进度和状态信息
/// </summary>
public class GameSaveData
{
    /// <summary>
    /// 玩家物品栏字典，键为物品名称，值为对应数量
    /// </summary>
    public readonly Dictionary<string, int> Inventory = new();

    /// <summary>
    /// 玩家等级
    /// </summary>
    public int PlayerLevel { get; set; } = 1;

    /// <summary>
    /// 玩家经验值
    /// </summary>
    public int PlayerExp { get; set; } = 0;

    /// <summary>
    /// 当前场景名称
    /// </summary>
    public string CurrentScene { get; set; } = "main_menu";

    /// <summary>
    /// 游戏标志字典，用于存储游戏进度标记
    /// </summary>
    public Dictionary<string, object> GameFlags { get; set; } = new();

    /// <summary>
    /// 存档版本号，用于处理不同版本间的兼容性
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// 存档时间，记录最后保存的时间
    /// </summary>
    public DateTime SaveTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 槽位描述，用于在UI中显示额外信息
    /// </summary>
    public string SlotDescription { get; set; } = string.Empty;

    /// <summary>
    /// 运行时脏标记，用于标识存档数据是否被修改过
    /// </summary>
    [JsonIgnore]
    public bool RuntimeDirty { get; set; }
}