using EchoesOfThePit.scripts.component;
using EchoesOfThePit.scripts.core.ui;
using EchoesOfThePit.scripts.data;
using EchoesOfThePit.scripts.enums.ui;
using EchoesOfThePit.scripts.events.data;
using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.Game.Abstractions.ui;
using GFramework.Godot.extensions;
using GFramework.Godot.ui;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using ConfirmationDialog = EchoesOfThePit.scripts.component.ConfirmationDialog;

namespace EchoesOfThePit.scripts.load_menu;

[ContextAware]
[Log]
public partial class LoadMenu : Control, IController, IUiPageBehaviorProvider, ISimpleUiPage
{
    /// <summary>
    /// 页面行为实例的私有字段
    /// </summary>
    private IUiPageBehavior? _page;

    private SaveStorageUtility _saveStorageUtility = null!;

    private IUiRouter _uiRouter = null!;
    private ConfirmationDialog ConfirmationDialog => GetNode<ConfirmationDialog>("ConfirmationDialog");
    private Button BackButton => GetNode<Button>("%BackButton");
    private VBoxContainer SlotContainer => GetNode<VBoxContainer>("%SlotContainer");

    /// <summary>
    ///  Ui Key的字符串形式
    /// </summary>
    private static string UiKeyStr => nameof(UiKey.LoadMenu);

    public void OnEnter(IUiPageEnterParam? param)
    {
        RefreshAllSlots();
    }


    /// <summary>
    /// 获取页面行为实例，如果不存在则创建新的CanvasItemUiPageBehavior实例
    /// </summary>
    /// <returns>返回IUiPageBehavior类型的页面行为实例</returns>
    public IUiPageBehavior GetPage()
    {
        _page ??= new CanvasItemUiPageBehavior<Control>(this, UiKeyStr);
        return _page;
    }

    public override void _Ready()
    {
        _saveStorageUtility = this.GetUtility<SaveStorageUtility>()!;
        _uiRouter = this.GetSystem<IUiRouter>()!;
        InitializeSlots();
        SetupEventHandlers();
        CallDeferred(nameof(CheckIfInStack));
    }

    /// <summary>
    /// 检查当前UI是否在路由栈顶，如果不在则将页面推入路由栈
    /// </summary>
    private void CheckIfInStack()
    {
        if (!_uiRouter.IsTop(UiKeyStr))
        {
            _uiRouter.Push(GetPage());
        }
    }

    private void InitializeSlots()
    {
        var slotContainer = SlotContainer;
        for (var i = 0; i < slotContainer.GetChildCount(); i++)
        {
            if (slotContainer.GetChild(i) is not SaveSlotItem slotItem) continue;

            var saveData = _saveStorageUtility.Load(i);

            slotItem.Initialize(i, saveData, isLoadMode: true);
            slotItem
                .RegisterEvent<ActionPressedEvent>(e => OnSlotActionPressed(e.Slot))
                .UnRegisterWhenNodeExitTree(this);
            slotItem
                .RegisterEvent<DeletePressedEvent>(e => OnSlotDeletePressed(e.Slot))
                .UnRegisterWhenNodeExitTree(this);
        }
    }

    private void SetupEventHandlers()
    {
        BackButton.Pressed += OnBackPressed;
    }

    private void RefreshAllSlots()
    {
        var slotContainer = SlotContainer;
        for (var i = 0; i < slotContainer.GetChildCount(); i++)
        {
            if (slotContainer.GetChild(i) is not SaveSlotItem slotItem) continue;

            var slotIndex = i;
            var saveData = _saveStorageUtility.Load(slotIndex);
            slotItem.Refresh(saveData);
        }
    }

    private void OnSlotActionPressed(int slot)
    {
        var slotItem = GetSlotItem(slot);
        if (slotItem is not { HasSave: true }) return;

        ConfirmationDialog.ShowDialog($"确认要读取槽位 {slot + 1} 的存档吗？", () => LoadGame(slot));
    }

    private void OnSlotDeletePressed(int slot)
    {
        var slotItem = GetSlotItem(slot);
        if (slotItem == null || !slotItem.HasSave) return;

        ConfirmationDialog.ShowDialog($"确认要删除槽位 {slot + 1} 的存档吗？", () => DeleteGame(slot));
    }

    private void LoadGame(int slot)
    {
        var saveData = _saveStorageUtility.Load(slot);
        ApplyGameData(saveData);

        _log.Debug($"游戏已从槽位 {slot + 1} 读取");
    }

    private void ApplyGameData(GameSaveData saveData)
    {
    }

    private void DeleteGame(int slot)
    {
        _saveStorageUtility.Delete(slot);
        RefreshAllSlots();
        _log.Debug($"槽位 {slot + 1} 的存档已删除");
    }

    private void OnBackPressed()
    {
        _uiRouter.Pop();
    }

    private SaveSlotItem? GetSlotItem(int slot)
    {
        var slotContainer = SlotContainer;
        if (slot < 0 || slot >= slotContainer.GetChildCount()) return null;
        return slotContainer.GetChild(slot) as SaveSlotItem;
    }
}