using System;
using System.Collections.Generic;
using System.Text.Json;
using EchoesOfThePit.scripts.config.interfaces;
using GFramework.Core.model;
using GFramework.SourceGenerators.Abstractions.logging;
using Godot;

namespace EchoesOfThePit.scripts.config;

[Log]
/// <summary>
/// 配置模型（负责从 JSON 加载游戏表数据），实现 `IConfigModel`。
/// 在模型初始化时会尝试从 `res://data/game_data.json` 读取并反序列化为 `ConfigData`。
/// </summary>
public partial class ConfigModel : AbstractModel, IConfigModel
{
    private ConfigData _config = new ConfigData();
    private const string ConfigPath = "res://assets/data/";

    protected override void OnInit()
    {
        LoadConfig();
    }

    public ConfigData GetConfig() => _config;

    /// <summary>
    /// 从指定文件读取 JSON 文本
    /// </summary>
    private string ReadJsonFile(string fileName)
    {
        try
        {
            var resPath = ConfigPath + fileName;
            if (FileAccess.FileExists(resPath))
            {
                using var file = FileAccess.Open(resPath, FileAccess.ModeFlags.Read);
                if (file != null)
                {
                    return file.GetAsText();
                }
            }
            else
            {
                _log.Warn($"File not found: {resPath}");
            }
        }
        catch (Exception e)
        {
            _log.Warn($"Godot FileAccess read failed for {fileName}: {e.Message}");
        }

        return string.Empty;
    }

    /// <summary>
    /// 加载并反序列化 JSON 文件
    /// </summary>
    private T LoadJsonData<T>(string fileName, JsonSerializerOptions options) where T : new()
    {
        var json = ReadJsonFile(fileName);
        if (string.IsNullOrEmpty(json))
        {
            _log.Error($"Failed to load {fileName}");
            return new T();
        }

        try
        {
            var result = JsonSerializer.Deserialize<T>(json, options);
            return result ?? new T();
        }
        catch (JsonException ex)
        {
            _log.Error($"Failed to deserialize {fileName}: {ex.Message}");
            return new T();
        }
    }

    /// <summary>
    /// 加载所有配置数据
    /// </summary>
    private void LoadConfig()
    {
        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // 读取角色列表
            _config.Characters = LoadJsonData<List<CharacterData>>("characters.json", options);

            // 读取敌人列表
            _config.Enemies = LoadJsonData<List<EnemyData>>("enemies.json", options);

            // 读取道具列表
            _config.Items = LoadJsonData<List<ItemData>>("items.json", options);

            // 记录加载结果
            LogLoadingResults();
        }
        catch (Exception ex)
        {
            _log.Error($"Failed to load JSON files: {ex.Message}");
            _config = new ConfigData();
        }
    }

    /// <summary>
    /// 记录加载结果信息
    /// </summary>
    private void LogLoadingResults()
    {
        _log.Info($"Characters loaded: {_config.Characters.Count}");
        _log.Info($"Enemies loaded: {_config.Enemies.Count}");
        _log.Info($"Items loaded: {_config.Items.Count}");
    }
}