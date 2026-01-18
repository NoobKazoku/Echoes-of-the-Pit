using GFramework.Core.Abstractions.model;

namespace EchoesOfThePit.scripts.setting.interfaces;

/// <summary>
/// 定义应用程序设置模型的接口，继承自IModel基础接口
/// 提供对图形设置和音频设置的访问
/// </summary>
public interface ISettingsModel: IModel
{
    /// <summary>
    /// 获取图形设置配置对象
    /// </summary>
    GraphicsSettings Graphics { get; }
    
    /// <summary>
    /// 获取音频设置配置对象
    /// </summary>
    AudioSettings Audio { get; }
    
    /// <summary>
    /// 获取本地化设置配置对象
    /// </summary>
    LocalizationSettings Localization { get; }
    
    /// <summary>
    /// 获取当前设置的数据对象
    /// </summary>
    /// <returns>返回包含所有设置数据的SettingsData对象</returns>
    SettingsData GetSettingsData();
}
