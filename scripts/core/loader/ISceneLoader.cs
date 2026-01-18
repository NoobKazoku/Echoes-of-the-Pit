using GFramework.Core.Abstractions.utility;
using Godot;

namespace EchoesOfThePit.scripts.core.loader;

/// <summary>
/// 场景加载器接口，定义了场景替换、卸载和当前场景获取的基本操作
/// </summary>
public interface ISceneLoader : IUtility
{
    /// <summary>
    /// 获取当前加载的场景节点
    /// </summary>
    /// <returns>当前场景的根节点，如果无场景则返回null</returns>
    Node? Current { get; }

    /// <summary>
    /// 替换当前场景为指定路径的新场景
    /// </summary>
    /// <param name="key">要加载的场景资源路径</param>
    void Replace(string key);

    /// <summary>
    /// 卸载当前场景
    /// </summary>
    void Unload();
}