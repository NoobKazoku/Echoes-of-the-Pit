using System.Collections.Generic;
using GFramework.Core.Abstractions.model;

namespace EchoesOfThePit.scripts.game_state.interfaces;

public interface IGameStateManager : IModel
{
    int PlayerLevel { get; set; }
    int PlayerExp { get; set; }
    string CurrentScene { get; set; }
    IDictionary<string, object> GameFlags { get; }

    void SetFlag(string key, object value);
    bool GetFlag(string key);
    T? GetFlag<T>(string key);
    bool HasFlag(string key);
    void ClearFlags();
    void Reset();
}