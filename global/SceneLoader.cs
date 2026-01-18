using EchoesOfThePit.scripts.core.loader;
using GFramework.Godot.extensions;
using Godot;

namespace EchoesOfThePit.global;

/// <summary>
/// 场景加载器，负责管理游戏场景的加载、替换和卸载
/// </summary>
public partial class SceneLoader : Node, ISceneLoader
{
    [Export] private NodePath _gameRootPath = new("/root/GameRoot");

    private Node GameRoot => GetNode(_gameRootPath);

    /// <summary>
    /// 当前加载的场景节点
    /// </summary>
    public Node? Current { get; private set; }

    /// <summary>
    /// 替换当前场景为指定路径的新场景
    /// </summary>
    /// <param name="scenePath">新场景的资源路径</param>
    public void Replace(string scenePath)
    {
        // 1. 卸载旧场景
        if (Current != null)
        {
            Current.QueueFreeX();
            Current = null;
        }

        // 2. 加载新场景
        var packed = GD.Load<PackedScene>(scenePath);
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
}
