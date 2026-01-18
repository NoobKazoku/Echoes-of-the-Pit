using EchoesOfThePit.scripts.core.loader;
using EchoesOfThePit.scripts.events.scene;
using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using GFramework.Godot.scene;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;

namespace EchoesOfThePit.global;

/// <summary>
/// 场景加载器，负责管理游戏场景的加载、替换和卸载
/// </summary>
[Log]
[ContextAware]
public partial class SceneLoader : Node, ISceneLoader, IController
{
    [Export] private NodePath _gameRootPath = new("/root/GameRoot");
    private GodotSceneRegistry _sceneRegistry = null!;

    private Node GameRoot => GetNode(_gameRootPath);

    /// <summary>
    /// 当前加载的场景节点
    /// </summary>
    public Node? Current { get; private set; }


    /// <summary>
    /// 替换当前场景为指定键对应的新场景
    /// </summary>
    /// <param name="key">场景注册表中的键值</param>
    public void Replace(string key)
    {
        _log.Info("Replace scene: {0}", key);
        // 1. 卸载旧场景
        if (Current != null)
        {
            Current.QueueFreeX();
            Current = null;
        }

        // 2. 加载新场景
        var packed = _sceneRegistry.Get(key);
        var scene = packed.Instantiate();
        // 3. 挂到 GameRoot
        GameRoot.AddChild(scene);
        Current = scene;
    }

    /// <summary>
    /// 卸载当前场景
    /// </summary>
    public void Unload()
    {
        Current.QueueFreeX();
        Current = null;
    }

    public override void _Ready()
    {
        _sceneRegistry = this.GetUtility<GodotSceneRegistry>()!;
        // 监听 ChangeSceneEvent
        this.RegisterEvent<ChangeSceneEvent>(e => Replace(e.SceneKey)).UnRegisterWhenNodeExitTree(this);
        // 监听 UnloadSceneEvent
        this.RegisterEvent<UnloadSceneEvent>(_ => Unload()).UnRegisterWhenNodeExitTree(this);
    }
}