using EchoesOfThePit.scripts.move_manager.models;
using EchoesOfThePit.scripts.move_manager.systems;
using GFramework.Core.Abstractions.architecture;
using GFramework.Game.architecture;

namespace EchoesOfThePit.scripts.move_manager;

/// <summary>
/// 移动系统模块类，负责安装和注册移动相关的模型和系统
/// </summary>
public class MovementModule : AbstractModule
{
    /// <summary>
    /// 安装方法，用于向游戏架构注册移动相关的模型和系统
    /// </summary>
    /// <param name="architecture">游戏架构接口实例，用于注册模型和系统</param>
    public override void Install(IArchitecture architecture)
    {
        // 注册移动相关的模型
        architecture.RegisterModel(new PlayerPositionModel());
        architecture.RegisterModel(new GridStateModel());
        architecture.RegisterModel(new GridMapModel());
        
        // 注册移动系统
        architecture.RegisterSystem(new MovementSystem());
        architecture.RegisterSystem(new PathfindingSystem());
    }
}
