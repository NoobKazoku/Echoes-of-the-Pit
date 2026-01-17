# UI切换扩展点系统使用指南

## 概述

UI切换扩展点系统为UI Router提供了可扩展的副作用管线（Side-Effect Pipeline），允许在UI切换前后插入自定义逻辑，如播放音效、执行淡入淡出动画、延迟切换等。

## 核心架构

```
UiRouter (System)
├── UiTransitionPipeline (内部组件)
│   └── IUiTransitionHandler[] (Handler列表)
├── BeforeChange阶段（阻塞）
└── AfterChange阶段（不阻塞）
```

## 使用方式

### 1. 注册自定义Handler

```csharp
// 获取UiRouter实例
var uiRouter = this.GetSystem<IUiRouter>();

// 注册Handler
uiRouter.RegisterHandler(new MyCustomHandler(),
    new UiTransitionHandlerOptions(
        TimeoutMs: 3000,
        ContinueOnError: true
    ));

// 注销Handler
uiRouter.UnregisterHandler(myHandler);
```

### 2. 实现自定义Handler

#### 方式A：实现IUiTransitionHandler接口

```csharp
using System.Threading;
using System.Threading.Tasks;
using GFramework.SourceGenerators.Abstractions.logging;

[Log]
public sealed partial class MyCustomHandler : IUiTransitionHandler
{
    public int Priority => 100;
    public UiTransitionPhase Phase => UiTransitionPhase.BeforeChange;

    public bool ShouldHandle(UiTransitionEvent @event, UiTransitionPhase phase)
    {
        // 只在Push时处理
        return @event.TransitionType == UiTransitionType.Push;
    }

    public async Task HandleAsync(UiTransitionEvent @event, CancellationToken ct)
    {
        // 自定义逻辑
        await Task.Delay(500, ct);
    }
}
```

#### 方式B：继承UiTransitionHandlerBase（推荐）

```csharp
using System.Threading;
using System.Threading.Tasks;
using GFramework.SourceGenerators.Abstractions.logging;

[Log]
public sealed partial class MyCustomHandler : UiTransitionHandlerBase
{
    public override int Priority => 100;
    public override UiTransitionPhase Phase => UiTransitionPhase.BeforeChange;

    public override bool ShouldHandle(UiTransitionEvent @event, UiTransitionPhase phase)
    {
        return @event.TransitionType == UiTransitionType.Push;
    }

    public override async Task HandleAsync(UiTransitionEvent @event, CancellationToken ct)
    {
        // 自定义逻辑
        await Task.Delay(500, ct);
    }
}
```

### 3. Handler间通信

```csharp
// Handler1：设置数据
public class Handler1 : UiTransitionHandlerBase
{
    public override int Priority => 100;

    public override async Task HandleAsync(UiTransitionEvent @event, CancellationToken ct)
    {
        var animationDuration = await PlayFadeAnimationAsync();
        // 设置数据供后续Handler使用
        @event.Set("AnimationDuration", animationDuration);
    }
}

// Handler2：读取数据
public class Handler2 : UiTransitionHandlerBase
{
    public override int Priority => 200;

    public override async Task HandleAsync(UiTransitionEvent @event, CancellationToken ct)
    {
        // 获取Handler1设置的数据
        var duration = @event.Get<double>("AnimationDuration");
        // 使用数据
        await PlaySoundWithDurationAsync(duration);
    }
}
```

## 内置Handler

### LoggingTransitionHandler

记录所有UI切换事件的日志。

- **Priority**: 999（最后执行）
- **Phase**: All（所有阶段）

### AudioTransitionHandler

播放UI切换音效和背景音乐。

- **Priority**: 200
- **Phase**: All
- **BeforeChange**: 播放切换音效
- **AfterChange**: 播放目标UI的BGM

### DelayTransitionHandler（示例）

延迟UI切换，用于演示Handler的使用。

- **Priority**: 50
- **Phase**: BeforeChange
- **仅适用于**: Push操作

## API参考

### UiTransitionEvent

UI切换事件，包含完整的上下文信息。

**属性**:
- `FromUiKey`: 源UI标识符
- `ToUiKey`: 目标UI标识符
- `TransitionType`: 切换类型（Push/Pop/Replace/Clear）
- `Policy`: 切换策略
- `EnterParam`: UI进入参数

**方法**:
- `Get<T>(string key, T defaultValue)`: 获取用户数据
- `TryGet<T>(string key, out T value)`: 尝试获取用户数据
- `Set<T>(string key, T value)`: 设置用户数据
- `Has(string key)`: 检查是否存在键
- `Remove(string key)`: 移除键

### UiTransitionPhase

UI切换阶段枚举（Flags）。

- `BeforeChange = 1`: UI切换前（可阻塞）
- `AfterChange = 2`: UI切换后（不阻塞）
- `All = BeforeChange | AfterChange`: 所有阶段

### UiTransitionType

UI切换类型枚举。

- `Push`: 压入新页面
- `Pop`: 弹出当前页面
- `Replace`: 替换当前页面
- `Clear`: 清空所有页面

### UiTransitionHandlerOptions

Handler执行选项。

**属性**:
- `TimeoutMs`: 超时时间（毫秒），0表示不超时
- `ContinueOnError`: 发生错误时是否继续执行后续Handler

## 执行流程

### Push操作

```
1. BeforeChange阶段（阻塞）
   - 执行所有Phase包含BeforeChange的Handler
   - 等待所有Handler完成

2. UI切换（原有逻辑）
   - 暂停/隐藏当前UI
   - 创建新UI
   - 进入/显示新UI

3. AfterChange阶段（不阻塞）
   - 后台执行所有Phase包含AfterChange的Handler
```

### Pop/Replace/Clear操作

类似的流程，只是`TransitionType`不同。

## 最佳实践

### 1. 优先级设置

- **50-99**: 需要提前执行的逻辑（延迟、确认对话框）
- **100-199**: 视觉效果类（淡入淡出、动画）
- **200-299**: 音效类（切换音效、BGM）
- **300-999**: 其他逻辑（日志、统计、数据加载）

### 2. 阶段选择

- **BeforeChange**: 需要阻塞UI切换的操作（动画、用户确认）
- **AfterChange**: 不阻塞的操作（音效、日志、统计）
- **All**: 需要在两个阶段都执行的操作

### 3. 错误处理

```csharp
// 容错模式：继续执行
uiRouter.RegisterHandler(handler,
    new UiTransitionHandlerOptions(ContinueOnError: true));

// 严格模式：出错停止
uiRouter.RegisterHandler(handler,
    new UiTransitionHandlerOptions(ContinueOnError: false));
```

### 4. 超时控制

```csharp
// 设置超时
uiRouter.RegisterHandler(handler,
    new UiTransitionHandlerOptions(TimeoutMs: 5000));

// 不设置超时（默认）
uiRouter.RegisterHandler(handler);
```

### 5. 条件执行

```csharp
public override bool ShouldHandle(UiTransitionEvent @event, UiTransitionPhase phase)
{
    // 只在特定条件下处理
    return phase == UiTransitionPhase.BeforeChange
        && @event.TransitionType == UiTransitionType.Push
        && @event.ToUiKey == "MainMenu";
}
```

## 注意事项

1. **不要在Handler中执行长时间阻塞操作**，除非必要
2. **BeforeChange阶段会阻塞UI切换**，注意用户体验
3. **Handler间通信通过UserData**，但要注意类型安全
4. **异常处理**：默认继续执行，可配置为停止
5. **生命周期**：Handler由调用者管理，Router不负责释放

## 扩展示例

### 淡入淡出Handler

```csharp
public class FadeTransitionHandler : UiTransitionHandlerBase
{
    public override int Priority => 100;
    public override UiTransitionPhase Phase => UiTransitionPhase.BeforeChange;

    public override bool ShouldHandle(UiTransitionEvent @event, UiTransitionPhase phase)
    {
        return @event.TransitionType != UiTransitionType.Pop;
    }

    public override async Task HandleAsync(UiTransitionEvent @event, CancellationToken ct)
    {
        await FadeOutAsync(@event.FromUiKey, ct);
        @event.Set("ShouldPlaySwitchSound", true);
        await FadeInAsync(@event.ToUiKey, ct);
    }

    private async Task FadeOutAsync(string uiKey, CancellationToken ct)
    {
        // 实现淡出逻辑
    }

    private async Task FadeInAsync(string uiKey, CancellationToken ct)
    {
        // 实现淡入逻辑
    }
}
```

### 震动反馈Handler

```csharp
public class VibrateTransitionHandler : UiTransitionHandlerBase
{
    public override int Priority => 150;
    public override UiTransitionPhase Phase => UiTransitionPhase.AfterChange;

    public override bool ShouldHandle(UiTransitionEvent @event, UiTransitionPhase phase)
    {
        // 只在有震动功能的平台执行
        return OS.HasFeature("vibrate")
            && @event.TransitionType == UiTransitionType.Push;
    }

    public override Task HandleAsync(UiTransitionEvent @event, CancellationToken ct)
    {
        // 触发震动
        return Task.CompletedTask;
    }
}
```
