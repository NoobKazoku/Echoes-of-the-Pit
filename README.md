# GFramework-Godot-Template



## 命名规范
 
1. 变量名：首字母小写，驼峰命名法
2. 常量名：全大写，下划线分隔
3. 文件夹和文件名： 全小写，下划线分隔

## 文件夹结构

- assets: 包含游戏资源，如美术、数据、字体、音乐和声音文件。
- global: 存放全局C#脚本（例如GameEntryPoint.cs、UiRoot.cs）及相关场景文件，主要用于项目的全局入口点和UI根节点。
- resource: 包含资源文件，如音频总线布局，并有着色器和主题的子目录，用于管理项目范围内的资源，如音频设置和UI主题。
- scenes: 包含主场景文件（main.tscn）和一个tests子目录，通常用于存储Godot场景文件和测试场景。
- script_templates: 包含Godot的自定义脚本模板，包括一个.editorconfig文件和用于标准化代码生成的Node子目录。
- scripts: 组织为core和module子目录，包含GDScript或其他脚本，用于核心功能和模块化组件。 

## 框架文档
https://qf.readthedocs.io/zh-cn/latest/QFramework_v1.0_Guide/01_introduction/01_introduction.html

## 许可证

### 源代码
本项目的源代码根据Apache许可证第2.0版进行许可。

### 游戏资源
所有游戏资源（包括但不限于美术、音频、字体和文本）
不受Apache许可证2.0的约束。

除非另有说明，否则所有资源均为©作者所有，未经明确许可不得使用。