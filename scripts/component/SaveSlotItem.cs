using System.Globalization;
using EchoesOfThePit.scripts.data;
using EchoesOfThePit.scripts.events.data;
using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;

namespace EchoesOfThePit.scripts.component;

/// <summary>
/// 存档槽位组件，用于显示单个存档槽位的信息和操作按钮
/// </summary>
[ContextAware]
[Log]
public partial class SaveSlotItem : HBoxContainer, IController
{
    private bool _isLoadMode;
    private GameSaveData? _saveData;
    private bool _showDeleteButton;

    private int _slot;

    /// <summary>
    /// 槽位标签控件
    /// </summary>
    private Label SlotLabel => GetNode<Label>("%SlotLabel");

    /// <summary>
    /// 时间标签控件
    /// </summary>
    private Label TimeLabel => GetNode<Label>("%TimeLabel");

    /// <summary>
    /// 操作按钮控件
    /// </summary>
    private Button ActionButton => GetNode<Button>("%ActionButton");

    /// <summary>
    /// 删除按钮控件
    /// </summary>
    private Button DeleteButton => GetNode<Button>("%DeleteButton");

    /// <summary>
    /// 获取槽位索引
    /// </summary>
    public int Slot => _slot;

    /// <summary>
    /// 获取存档数据
    /// </summary>
    public GameSaveData? SaveData => _saveData;

    /// <summary>
    /// 检查是否有存档数据
    /// </summary>
    public bool HasSave => _saveData is { SlotDescription.Length: > 0 };

    /// <summary>
    /// 组件准备就绪时调用，初始化事件监听器
    /// </summary>
    public override void _Ready()
    {
        ActionButton.Pressed += OnActionButtonPressed;
        DeleteButton.Pressed += OnDeleteButtonPressed;
    }

    /// <summary>
    /// 初始化存档槽位组件
    /// </summary>
    /// <param name="slot">槽位索引</param>
    /// <param name="saveData">存档数据，可能为空</param>
    /// <param name="isLoadMode">是否为加载模式</param>
    public void Initialize(int slot, GameSaveData? saveData, bool isLoadMode)
    {
        _slot = slot;
        _saveData = saveData;
        _isLoadMode = isLoadMode;
        _showDeleteButton = isLoadMode;

        UpdateDisplay();
        UpdateButtons();
    }

    /// <summary>
    /// 更新显示信息，包括槽位名称和保存时间
    /// </summary>
    private void UpdateDisplay()
    {
        SlotLabel.Text = $"槽位 {_slot + 1}";

        // 根据存档数据是否存在来设置时间显示和颜色
        if (_saveData is { SlotDescription.Length: > 0 })
        {
            TimeLabel.Text =
                _saveData.SaveTime.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            TimeLabel.Modulate = new Color(1, 1, 1);
        }
        else
        {
            TimeLabel.Text = "空槽位";
            TimeLabel.Modulate = new Color(0.7f, 0.7f, 0.7f);
        }
    }


    /// <summary>
    /// 更新按钮状态，根据存档数据和模式设置按钮文本和可用性
    /// </summary>
    private void UpdateButtons()
    {
        DeleteButton.Visible = _showDeleteButton;

        // 根据存档数据存在情况设置按钮状态
        if (_saveData is { SlotDescription.Length: > 0 })
        {
            ActionButton.Text = _isLoadMode ? "读取" : "覆盖";
            ActionButton.Disabled = false;
            DeleteButton.Disabled = false;
        }
        else
        {
            ActionButton.Text = _isLoadMode ? "读取" : "保存";
            ActionButton.Disabled = _isLoadMode;
            DeleteButton.Disabled = true;
        }
    }

    /// <summary>
    /// 刷新存档槽位显示
    /// </summary>
    /// <param name="saveData">新的存档数据</param>
    public void Refresh(GameSaveData? saveData)
    {
        _saveData = saveData;
        UpdateDisplay();
        UpdateButtons();
    }

    /// <summary>
    /// 处理操作按钮点击事件
    /// </summary>
    private void OnActionButtonPressed()
    {
        this.SendEvent(new ActionPressedEvent { Slot = _slot });
    }

    /// <summary>
    /// 处理删除按钮点击事件
    /// </summary>
    private void OnDeleteButtonPressed()
    {
        this.SendEvent(new DeletePressedEvent { Slot = _slot });
    }
}