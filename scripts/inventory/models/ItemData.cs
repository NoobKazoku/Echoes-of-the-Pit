using EchoesOfThePit.scripts.enums;
using Godot;

namespace EchoesOfThePit.scripts.inventory.models;

/// <summary>
/// 物品数据资源类，用于定义物品模板
/// 继承自Godot的Resource类，可在编辑器中创建和配置
/// </summary>
[GlobalClass]
public partial class ItemData : Resource
{
    /// <summary>
    /// 物品唯一标识符
    /// </summary>
    [Export]
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// 物品显示名称
    /// </summary>
    [Export]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 物品描述文本
    /// </summary>
    [Export(PropertyHint.MultilineText)]
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// 物品类型
    /// </summary>
    [Export]
    public ItemType ItemType { get; set; } = ItemType.None;
    
    /// <summary>
    /// 物品图标纹理
    /// </summary>
    [Export]
    public Texture2D? Icon { get; set; }
    
    /// <summary>
    /// 最大堆叠数量，默认为1（不可堆叠）
    /// </summary>
    [Export]
    public int MaxStack { get; set; } = 1;
    
    /// <summary>
    /// 文档内容，仅当ItemType为Document时有效
    /// </summary>
    [Export(PropertyHint.MultilineText)]
    public string DocumentContent { get; set; } = string.Empty;
    
    /// <summary>
    /// 检查该物品是否可堆叠
    /// </summary>
    public bool IsStackable => MaxStack > 1;
    
    /// <summary>
    /// 检查该物品是否为文档类型
    /// </summary>
    public bool IsDocument => ItemType == ItemType.Document;
}
