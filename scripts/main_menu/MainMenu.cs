using Godot;
using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.Game.Abstractions.ui;
using GFramework.Godot.ui;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;

[ContextAware]
[Log]
public partial class MainMenu :Control,IController,IUiPageBehaviorProvider,IUiPage
{
	/// <summary>
	/// 节点准备就绪时的回调方法
	/// 在节点添加到场景树后调用
	/// </summary>
	public override void _Ready()
	{
		GetNode<Button>("%NewGame").Pressed += () =>
		{
			_log.Debug("新游戏按钮被按下");
			GD.Print("新游戏按钮被按下");
			GetTree().ChangeSceneToFile("res://scenes/shop/shop.tscn");
		};

		GetNode<Button>("%LoadGame").Pressed += () =>
		{
			_log.Debug("加载存档按钮被按下");
			GD.Print("加载存档按钮被按下");
		};

		GetNode<Button>("%Option").Pressed += () =>
		{
			_log.Debug("设置按钮被按下");
			GD.Print("设置按钮被按下");
		};

		GetNode<Button>("%Credits").Pressed += () =>
		{
			_log.Debug("制作人员名单按钮被按下");
			GD.Print("制作人员名单按钮被按下");
		};

		GetNode<Button>("%Quit").Pressed += () =>
		{
			_log.Debug("退出按钮被按下");
			GD.Print("退出按钮被按下");
			GetTree().Quit();
		};
	}
	/// <summary>
	/// 页面行为实例的私有字段
	/// </summary>
	private IUiPageBehavior? _page;
	
	/// <summary>
	/// 获取页面行为实例，如果不存在则创建新的CanvasItemUiPageBehavior实例
	/// </summary>
	/// <returns>返回IUiPageBehavior类型的页面行为实例</returns>
	public IUiPageBehavior GetPage()
	{
		_page ??= new CanvasItemUiPageBehavior<Control>(this);
		return _page;
	}
	
	/// <summary>
	/// 页面进入时调用的方法
	/// </summary>
	/// <param name="param">页面进入参数，可能为空</param>
	public void OnEnter(IUiPageEnterParam? param)
	{
		
	}
	/// <summary>
	/// 页面退出时调用的方法
	/// </summary>
	void IUiPage.OnExit()
	{
		
	}


	/// <summary>
	/// 页面暂停时调用的方法
	/// </summary>
	void IUiPage.OnPause()
	{
		
	}

	/// <summary>
	/// 页面恢复时调用的方法
	/// </summary>
	void IUiPage.OnResume()
	{
		
	}

	/// <summary>
	/// 页面显示时调用的方法
	/// </summary>
	void IUiPage.OnShow()
	{
	   
	}

	/// <summary>
	/// 页面隐藏时调用的方法
	/// </summary>
	void IUiPage.OnHide()
	{
		
	}
}
