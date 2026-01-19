using System;
using EchoesOfThePit.scripts.command.game;
using EchoesOfThePit.scripts.command.game.input;
using EchoesOfThePit.scripts.component;
using EchoesOfThePit.scripts.core.ui;
using EchoesOfThePit.scripts.data;
using EchoesOfThePit.scripts.enums.ui;
using EchoesOfThePit.scripts.events.data;
using EchoesOfThePit.scripts.events.menu;
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
    /// <summary>
    /// 页面行为实例的私有字段
    /// </summary>
    private IUiPageBehavior? _page;

    private SaveStorageUtility _saveStorageUtility = null!;

    private IUiRouter _uiRouter = null!;

    private ConfirmationDialog ConfirmationDialog => GetNode<ConfirmationDialog>("%ConfirmationDialog");
    private Button BackButton => GetNode<Button>("%BackButton");
    private VBoxContainer SlotContainer => GetNode<VBoxContainer>("%SlotContainer");

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
        _page ??= new CanvasItemUiPageBehavior<Control>(this, nameof(UiKey.SaveMenu));
        return _page;
    }

    public override void _Ready()
    {
        _saveStorageUtility = this.GetUtility<SaveStorageUtility>()!;
        _uiRouter = this.GetSystem<IUiRouter>()!;

        InitializeSlots();
        SetupEventHandlers();
        // 判断是否在栈顶如果不在则入栈
        if (!_uiRouter.IsTop(nameof(UiKey.SaveMenu)))
        {
            _log.Debug($"进入{nameof(UiKey.SaveMenu)}");
            _uiRouter.Push(GetPage());
        }
    }

    private void InitializeSlots()
    {
        for (var i = 0; i < SlotContainer.GetChildCount(); i++)
        {
            if (SlotContainer.GetChild(i) is not SaveSlotItem slotItem) continue;

            var saveData = _saveStorageUtility.Load(i);

            slotItem.Initialize(i, saveData, isLoadMode: false);
            this
                .RegisterEvent<ActionPressedEvent>(e => OnSlotActionPressed(e.Slot))
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
        var saveData = _saveStorageUtility.Load(0);

        var currentData = new GameSaveData
        {
            Version = saveData.Version + 1,
            SaveTime = DateTime.Now,
            SlotDescription = saveData.SlotDescription,
        };

        foreach (var item in saveData.Inventory)
        {
            currentData.Inventory[item.Key] = item.Value;
        }

        return currentData;
    }

    private void OnBackPressed()
    {
        this.SendEvent<CloseSaveMenuEvent>();
        this.SendCommand(new ResumeGameCommand(new ResumeGameCommandInput { Node = this }));
    }

    private SaveSlotItem? GetSlotItem(int slot)
    {
        if (slot < 0 || slot >= SlotContainer.GetChildCount()) return null;
        return SlotContainer.GetChild(slot) as SaveSlotItem;
    }
}