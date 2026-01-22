namespace EchoesOfThePit.scripts.enums;

/// <summary>
/// 物品类型枚举，定义背包中可存储的物品种类
/// </summary>
public enum ItemType
{
    /// <summary>
    /// 空类型，表示无物品
    /// </summary>
    None = 0,
    
    /// <summary>
    /// 药水类物品，可使用消耗
    /// </summary>
    Potion,
    
    /// <summary>
    /// 钥匙类物品，剧情道具
    /// </summary>
    Key,
    
    /// <summary>
    /// 宝石类物品，收集品
    /// </summary>
    Gem,
    
    /// <summary>
    /// 文档类物品，可阅读查看内容
    /// </summary>
    Document
}
