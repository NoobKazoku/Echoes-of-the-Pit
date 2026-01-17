# UiRouter Pipeline 语义修复

## 修复概述

修复了 UiRouter 中 `Replace` 和 `Clear` 方法触发的 Pipeline 多次调用问题，确保每个 UI 切换操作只触发一次完整的 BeforeChange/AfterChange Pipeline。

## 问题描述

### 修复前

#### Replace 方法行为
```
Replace(page1):
  → BeforeChange Replace          ✅ 正确
  → BeforeChange Pop              ❌ 多余
  → AfterChange Pop                ❌ 多余
  → BeforeChange Push              ❌ 多余
  → AfterChange Push                ❌ 多余
  → AfterChange Replace            ✅ 正确

Pipeline 触发次数: 5 次（应该只有 2 次）
```

#### 问题影响
1. **音效重复播放**：AudioHandler 播放 3 次音效（Replace + Pop + Push）
2. **日志混乱**：LoggingHandler 记录 4 条日志（应该只有 2 条）
3. **动画错误**：FadeHandler 无法正确实现淡入淡出
4. **语义不清**：Handler 无法区分真实的 Pop/Push 和 Replace 内部的 Pop/Push

## 解决方案

### 核心思路

**分离 UI 栈操作逻辑和 Pipeline 触发逻辑**

将 UI 切换操作分为两部分：
1. **内部方法**：只执行 UI 栈操作（不触发 Pipeline）
2. **公开方法**：使用内部方法 + 触发 Pipeline

### 新增内部方法

```csharp
/// <summary>
/// 执行Push的核心逻辑（不触发Pipeline）
/// </summary>
private void DoPushInternal(string uiKey, IUiPageEnterParam? param, UiTransitionPolicy policy)
{
    // 暂停/隐藏当前页面
    // 创建新页面
    // 进入/显示新页面
}

/// <summary>
/// 执行Pop的核心逻辑（不触发Pipeline）
/// </summary>
private void DoPopInternal(UiPopPolicy policy)
{
    // 弹出页面
    // 退出/销毁/隐藏页面
    // 恢复/显示下一页面
}

/// <summary>
/// 执行Clear的核心逻辑（不触发Pipeline）
/// </summary>
private void DoClearInternal(UiPopPolicy policy)
{
    while (_stack.Count > 0)
        DoPopInternal(policy);
}
```

### 重构后的方法

#### Push 方法
```csharp
public void Push(string uiKey, IUiPageEnterParam? param, UiTransitionPolicy policy)
{
    var @event = CreateEvent(uiKey, UiTransitionType.Push, policy, param);

    BeforeChange(@event);       // ✅ 触发 BeforeChange Pipeline
    DoPushInternal(uiKey, param, policy);  // ✅ 执行内部逻辑
    AfterChange(@event);        // ✅ 触发 AfterChange Pipeline
}
```

#### Pop 方法
```csharp
public void Pop(UiPopPolicy policy)
{
    if (_stack.Count == 0)
        return;

    var nextUiKey = _stack.Count > 1 ? ... : string.Empty;
    var @event = CreateEvent(nextUiKey, UiTransitionType.Pop);

    BeforeChange(@event);       // ✅ 触发 BeforeChange Pipeline
    DoPopInternal(policy);       // ✅ 执行内部逻辑
    AfterChange(@event);        // ✅ 触发 AfterChange Pipeline
}
```

#### Replace 方法（关键修复）
```csharp
public void Replace(string uiKey, ...)
{
    var @event = CreateEvent(uiKey, UiTransitionType.Replace, pushPolicy, param);

    BeforeChange(@event);               // ✅ 触发 BeforeChange Replace Pipeline

    DoClearInternal(popPolicy);         // ✅ 使用内部方法，不触发额外的 Pipeline
    DoPushInternal(uiKey, param, pushPolicy);  // ✅ 使用内部方法，不触发额外的 Pipeline

    AfterChange(@event);                // ✅ 触发 AfterChange Replace Pipeline
}
```

#### Clear 方法（关键修复）
```csharp
public void Clear()
{
    var @event = CreateEvent(string.Empty, UiTransitionType.Clear);

    BeforeChange(@event);               // ✅ 触发 BeforeChange Clear Pipeline

    DoClearInternal(UiPopPolicy.Destroy);  // ✅ 使用内部方法，不触发额外的 Pipeline

    AfterChange(@event);                // ✅ 触发 AfterChange Clear Pipeline
}
```

## 修复效果

### Replace 操作

#### 修复前
```
Replace UI Stack with page: key=page1
  → BeforeChange Replace
  → Pop UI Page
    → BeforeChange Pop  ← 不应该触发
    → AfterChange Pop    ← 不应该触发
  → Push UI Page
    → BeforeChange Push  ← 不应该触发
    → AfterChange Push    ← 不应该触发
  → AfterChange Replace

Pipeline 触发次数: 5 次
AudioHandler 播放音效: 3 次
LoggingHandler 记录日志: 4 条
```

#### 修复后
```
Replace UI Stack with page: key=page1
  → BeforeChange Replace
  → Clear UI Stack internal
  → Push UI Page internal
  → AfterChange Replace

Pipeline 触发次数: 2 次 ✅
AudioHandler 播放音效: 1 次 ✅
LoggingHandler 记录日志: 2 条 ✅
```

### Clear 操作

#### 修复前
```
Clear UI Stack, stackCount=2
  → BeforeChange Clear
  → Pop UI Page 1
    → BeforeChange Pop  ← 不应该触发
    → AfterChange Pop    ← 不应该触发
  → Pop UI Page 2
    → BeforeChange Pop  ← 不应该触发
    → AfterChange Pop    ← 不应该触发
  → AfterChange Clear

Pipeline 触发次数: 3 次
```

#### 修复后
```
Clear UI Stack, stackCount=2
  → BeforeChange Clear
  → Clear UI Stack internal
  → AfterChange Clear

Pipeline 触发次数: 2 次 ✅
```

## Handler 视角的变化

### AudioTransitionHandler
- **修复前**：Replace 时播放 3 次音效
- **修复后**：Replace 时只播放 1 次音效

### LoggingTransitionHandler
- **修复前**：Replace 时记录 4 条日志
- **修复后**：Replace 时只记录 2 条日志

### 未来可能的 FadeHandler
- **修复前**：无法正确实现淡入淡出（Pop/Push 会打断动画）
- **修复后**：可以在 Replace 前后实现完整的淡入淡出动画

## 破坏性变更

✅ **无破坏性变更**

所有公开 API 保持不变：
- `Push(string uiKey, IUiPageEnterParam? param, UiTransitionPolicy policy)`
- `Pop(UiPopPolicy policy)`
- `Replace(string uiKey, IUiPageEnterParam? param, UiPopPolicy popPolicy, UiTransitionPolicy pushPolicy)`
- `Clear()`

所有 Handler 继续正常工作，但行为更加正确。

## 测试验证

### 测试 Push 操作
```csharp
uiRouter.Push(UiKeys.Page1);
```

**预期日志**：
```
Push UI Page: key=page1, policy=Exclusive, stackBefore=0
BeforeChange phases started: Push
Execute pipeline: Phases=BeforeChange, From=, To=page1, Type=Push
  Executing handler: AudioTransitionHandler
    Audio: Playing UI switch sound from  to page1
  Executing handler: LoggingTransitionHandler
    UI Transition: Phases=BeforeChange, Type=Push, From=, To=page1, Policy=Exclusive
Pipeline execution completed for phases: BeforeChange
BeforeChange phases completed: Push
Create UI Page instance: ControlUiPageBehavior
Enter & Show page: ControlUiPageBehavior, stackAfter=1
AfterChange phases started: Push
Execute pipeline: Phases=AfterChange, From=, To=page1, Type=Push
  Executing handler: AudioTransitionHandler
    Audio: Playing BGM for UI: page1
  Executing handler: LoggingTransitionHandler
    UI Transition: Phases=AfterChange, Type=Push, From=, To=page1, Policy=Exclusive
Pipeline execution completed for phases: AfterChange
AfterChange phases completed: Push
```

### 测试 Replace 操作
```csharp
uiRouter.Replace(UiKeys.Page2);
```

**预期日志**：
```
Replace UI Stack with page: key=page2, popPolicy=Destroy, pushPolicy=Exclusive
BeforeChange phases started: Replace
Execute pipeline: Phases=BeforeChange, From=Page1, To=page2, Type=Replace
  Executing handler: AudioTransitionHandler
    Audio: Playing UI switch sound from Page1 to page2
  Executing handler: LoggingTransitionHandler
    UI Transition: Phases=BeforeChange, Type=Replace, From=Page1, To=page2, Policy=Exclusive
Pipeline execution completed for phases: BeforeChange
BeforeChange phases completed: Replace
Clear UI Stack internal, count=1
Pop UI Page internal: ControlUiPageBehavior, policy=Destroy, stackAfterPop=0
Destroy UI Page: ControlUiPageBehavior
UI stack is now empty
Create UI Page instance: ControlUiPageBehavior
Enter & Show page: ControlUiPageBehavior, stackAfter=1
AfterChange phases started: Replace
Execute pipeline: Phases=AfterChange, From=Page1, To=page2, Type=Replace
  Executing handler: AudioTransitionHandler
    Audio: Playing BGM for UI: page2
  Executing handler: LoggingTransitionHandler
    UI Transition: Phases=AfterChange, Type=Replace, From=Page1, To=page2, Policy=Exclusive
Pipeline execution completed for phases: AfterChange
AfterChange phases completed: Replace
```

**关键验证点**：
- ✅ 只有 2 次 Pipeline 执行（BeforeChange Replace + AfterChange Replace）
- ✅ 没有 "BeforeChange Pop" 或 "AfterChange Pop"
- ✅ 没有 "BeforeChange Push" 或 "AfterChange Push"
- ✅ 只有 1 次音效播放

### 测试 Clear 操作
```csharp
uiRouter.Clear();
```

**预期日志**：
```
Clear UI Stack, stackCount=2
BeforeChange phases started: Clear
Execute pipeline: Phases=BeforeChange, From=, To=, Type=Clear
  Executing handler: LoggingTransitionHandler
    UI Transition: Phases=BeforeChange, Type=Clear, From=, To=, Policy=Exclusive
Pipeline execution completed for phases: BeforeChange
BeforeChange phases completed: Clear
Clear UI Stack internal, count=2
Pop UI Page internal: ControlUiPageBehavior, policy=Destroy, stackAfterPop=1
Destroy UI Page: ControlUiPageBehavior
Pop UI Page internal: ControlUiPageBehavior, policy=Destroy, stackAfterPop=0
Destroy UI Page: ControlUiPageBehavior
UI stack is now empty
AfterChange phases started: Clear
Execute pipeline: Phases=AfterChange, From=, To=, Type=Clear
  Executing handler: LoggingTransitionHandler
    UI Transition: Phases=AfterChange, Type=Clear, From=, To=, Policy=Exclusive
Pipeline execution completed for phases: AfterChange
AfterChange phases completed: Clear
```

**关键验证点**：
- ✅ 只有 2 次 Pipeline 执行（BeforeChange Clear + AfterChange Clear）
- ✅ 没有 "BeforeChange Pop" 或 "AfterChange Pop"
- ✅ Pop 操作标记为 "internal"

## 架构优势

### 1. 语义正确性
- Replace 是一次 Transition，而不是 Pop + Push 的组合
- Clear 是一次 Transition，而不是多次 Pop 的组合

### 2. 性能优化
- 减少不必要的 Pipeline 执行
- 减少 Handler 的冗余计算

### 3. 未来可扩展性
- 为动画 Handler 提供清晰的执行时机
- 支持异步 Handler 的正确实现
- 为 Transition Cancel / Merge 等高级功能奠定基础

### 4. Handler 友好
- Handler 可以清晰区分不同的 Transition 类型
- 避免了 Handler 需要判断"这是真实的 Pop 还是 Replace 内部的 Pop"
- 支持 Handler 在 Replace 前后实现完整的逻辑

## 总结

这次修复解决了 UiRouter 中的一个核心架构问题：**UI 切换操作的语义混淆**。

通过分离内部操作逻辑和 Pipeline 触发逻辑，我们确保：
- 每个 UI 切换操作只触发一次完整的 BeforeChange/AfterChange Pipeline
- Handler 可以准确地识别和响应不同的 UI 切换类型
- 系统为未来的扩展（如动画、异步 Handler 等）奠定了坚实的基础

这是一个**非破坏性**的修复，所有现有代码继续正常工作，但行为更加正确和可预测。
