using System.Collections.Generic;
using GFramework.Core.Abstractions.model;

namespace EchoesOfThePit.scripts.inventory.interfaces;

public interface IInventoryManager : IModel
{
    void AddItem(string itemId, int count = 1);
    void RemoveItem(string itemId, int count = 1);
    bool HasItem(string itemId);
    int GetItemCount(string itemId);
    IDictionary<string, int> GetAllItems();
    void Clear();
    void LoadFromData(Dictionary<string, int> inventoryData);
}