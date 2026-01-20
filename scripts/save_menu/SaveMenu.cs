using System;
using EchoesOfThePit.scripts.component;
using EchoesOfThePit.scripts.core.constants;
using EchoesOfThePit.scripts.core.ui;
using EchoesOfThePit.scripts.data;
using EchoesOfThePit.scripts.enums.ui;
using EchoesOfThePit.scripts.events.data;
using EchoesOfThePit.scripts.game_state.interfaces;
using EchoesOfThePit.scripts.inventory.interfaces;
using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.Game.Abstractions.ui;
using GFramework.Godot.extensions;
using GFramework.Godot.ui;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using ConfirmationDialog = EchoesOfThePit.scripts.component.ConfirmationDialog;

namespace EchoesOfThePit.scripts.save_menu;

[ContextAware]
[Log]
public partial class SaveMenu : Control, IController, IUiPageBehaviorProvider, ISimpleUiPage
{
    private IGameStateManager _gameStateManager = null!;

    private IInventoryManager _inventoryManager = null!;

    /// <summary>
    /// 页面行为实例的私有字段
    /// </summary>
    private IUiPageBehavior? _page;

    private SaveStorageUtility _saveStorageUtility = null!;

    private IUiRouter _uiRouter = null!;

    private ConfirmationDialog ConfirmationDialog => GetNode<ConfirmationDialog>("%ConfirmationDialog");
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
        _gameStateManager = this.GetModel<IGameStateManager>()!;
        _inventoryManager = this.GetModel<IInventoryManager>()!;

        InitializeSlots();
        SetupEventHandlers();
        CallDeferred(nameof(CallDeferredInit));
    }


    /// <summary>
    /// 延迟初始化
    /// </summary>
    private void CallDeferredInit()
    {
        var env = this.GetEnvironment();
        if (GameConstants.Development.Equals(env.Name, StringComparison.Ordinal) && !_uiRouter.IsTop(UiKeyStr))
        {
            _uiRouter.Push(GetPage());
        }

        this
            .RegisterEvent<ActionPressedEvent>(e => OnSlotActionPressed(e.Slot))
            .UnRegisterWhenNodeExitTree(this);
    }

    private void InitializeSlots()
    {
        for (var i = 0; i < SlotContainer.GetChildCount(); i++)
        {
            if (SlotContainer.GetChild(i) is not SaveSlotItem slotItem) continue;

            var saveData = _saveStorageUtility.Load(i);

            slotItem.Initialize(i, saveData, isLoadMode: false);
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
        if (slotItem == null) return;

        if (slotItem.HasSave)
        {
            ConfirmationDialog.ShowDialog($"确认要覆盖槽位 {slot + 1} 的存档吗？", () => SaveGame(slot));
        }
        else
        {
            SaveGame(slot);
        }
    }

    private void SaveGame(int slot)
    {
        var saveData = GetCurrentGameData();
        saveData.SaveTime = DateTime.Now;
        saveData.SlotDescription = $"存档于 {DateTime.Now:yyyy-MM-dd HH:mm}";

        _saveStorageUtility.Save(slot, saveData);

        RefreshAllSlots();
        _log.Debug($"游戏已保存到槽位 {slot + 1}");
    }

    private GameSaveData GetCurrentGameData()
    {
        var previousData = _saveStorageUtility.Load(0);

        var currentData = new GameSaveData
        {
            Version = previousData.Version + 1,
            SaveTime = DateTime.Now,
            SlotDescription = $"存档于 {DateTime.Now:yyyy-MM-dd HH:mm}",
            PlayerLevel = _gameStateManager.PlayerLevel,
            PlayerExp = _gameStateManager.PlayerExp,
            CurrentScene = _gameStateManager.CurrentScene,
        };

        foreach (var item in _inventoryManager.GetAllItems())
        {
            currentData.Inventory[item.Key] = item.Value;
        }

        foreach (var flag in _gameStateManager.GameFlags)
        {
            currentData.GameFlags[flag.Key] = flag.Value;
        }

        return currentData;
    }

    private void OnBackPressed()
    {
        _uiRouter.Pop();
    }

    private SaveSlotItem? GetSlotItem(int slot)
    {
        if (slot < 0 || slot >= SlotContainer.GetChildCount()) return null;
        return SlotContainer.GetChild(slot) as SaveSlotItem;
    }
}