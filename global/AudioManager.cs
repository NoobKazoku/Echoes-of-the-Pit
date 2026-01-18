using System.Collections.Generic;
using System.Linq;
using EchoesOfThePit.scripts.audio;
using EchoesOfThePit.scripts.core.constants;
using EchoesOfThePit.scripts.enums.audio;
using EchoesOfThePit.scripts.events.audio;
using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;

namespace EchoesOfThePit.global;

[ContextAware]
[Log]
public partial class AudioManager : Node, IController
{
    /// <summary>
    /// 获取背景音乐音频流播放器节点
    /// </summary>
    private AudioStreamPlayer BgmAudioStreamPlayer => GetNode<AudioStreamPlayer>("%BgmAudioStreamPlayer");

    private readonly List<AudioStreamPlayer> _sfxPlayers = [];

    private int _sfxIndex;
    [Export] private int _maxSfxPlayerCount = 12;

    /// <summary>
    /// 背景音乐音频流
    /// </summary>
    [Export]
    public AudioStream BgmAudioStream { get; set; } = null!;

    /// <summary>
    /// 游戏中音频流
    /// </summary>
    [Export]
    public AudioStream GamingAudioStream { get; set; } = null!;


    /// <summary>
    /// UI点击音效 - 存储用户界面交互时的音效
    /// </summary>
    [Export]
    public AudioStream UiClickSfx { get; set; } = null!;


    /// <summary>
    /// 启动音频流 - 存储系统或应用启动时播放的音频资源
    /// </summary>
    [Export]
    public AudioStream BootAudioStream { get; set; } = null!;


        /// <summary>
    /// 节点准备就绪时的回调方法
    /// 在节点添加到场景树后调用
    /// </summary>
    public override void _Ready()
    {
        _log.Debug("AudioManager节点准备就绪");

        BgmAudioStreamPlayer.Bus = GameConstants.Bgm;

        var model = this.GetModel<IAudioStateModel>()!;

        // 1️⃣ 启动时对齐状态
        ApplyScene(model.CurrentScene);

        // 2️⃣ 监听后续变化
        this.RegisterEvent<AudioSceneChangedEvent>(e => { ApplyScene(e.Scene); }).UnRegisterWhenNodeExitTree(this);

        this.RegisterEvent<PlaySfxEvent>(OnPlaySfx)
            .UnRegisterWhenNodeExitTree(this);
    }

    /// <summary>
    /// 根据音频场景应用相应的背景音乐
    /// </summary>
    /// <param name="scene">要应用的音频场景</param>
    private void ApplyScene(AudioScene scene)
    {
        _log.Debug($"Apply AudioScene: {scene}");

        BgmAudioStreamPlayer.Stop();

        // 根据不同音频场景设置对应的音频流
        BgmAudioStreamPlayer.Stream = scene switch
        {
            AudioScene.MainMenu => BgmAudioStream,
            AudioScene.InGame => GamingAudioStream,
            AudioScene.Boot => BootAudioStream,
            _ => null,
        };

        if (BgmAudioStreamPlayer.Stream != null)
            BgmAudioStreamPlayer.Play();
    }



    /// <summary>
    /// 创建新的音效播放器
    /// </summary>
    /// <returns>创建的音效播放器实例</returns>
    private AudioStreamPlayer CreateSfxPlayer()
    {
        var player = new AudioStreamPlayer
        {
            Bus = GameConstants.Sfx,
        };

        AddChild(player);
        _sfxPlayers.Add(player);

        return player;
    }

    /// <summary>
    /// 获取可用的音效播放器
    /// </summary>
    /// <returns>可用的音效播放器实例，若无可用播放器则返回null</returns>
    private AudioStreamPlayer? GetAvailableSfxPlayer()
    {
        // 1️⃣ 优先找一个没在播放的
        var availablePlayer = _sfxPlayers.FirstOrDefault(player => !player.Playing);
        if (availablePlayer != null)
            return availablePlayer;

        // 2️⃣ 如果没找到，且还没到上限 → 新建
        return _sfxPlayers.Count < _maxSfxPlayerCount
            ? CreateSfxPlayer()
            :
            // 3️⃣ 已达上限 → 丢弃
            null;
    }

    /// <summary>
    /// 处理音效播放事件
    /// </summary>
    /// <param name="event">音效播放事件</param>
    private void OnPlaySfx(PlaySfxEvent @event)
    {
        var player = GetAvailableSfxPlayer();
        if (player == null)
            return; // 达到上限，直接丢弃

        player.Stop();

        player.Stream = @event.SfxType switch
        {
            SfxType.UiClick => UiClickSfx,
            _ => null,
        };

        if (player.Stream == null)
            return;

        player.PitchScale = (float)GD.RandRange(0.95f, 1.05f);
        player.Play();
    }
}