using GFramework.Core.Abstractions.model;

namespace EchoesOfThePit.scripts.config.interfaces;

/// <summary>
/// 定义数据获取的接口，继承自IModel基础接口
/// 提供对数据的访问
/// </summary>
public interface IConfigModel : IModel
{
    ConfigData GetConfig();
}
