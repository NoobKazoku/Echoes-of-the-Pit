using System;
using System.Collections.Generic;
using EchoesOfThePit.scripts.inventory.interfaces;
using GFramework.Core.model;

namespace EchoesOfThePit.scripts.inventory;

public class InventoryManager : AbstractModel, IInventoryManager
{
	private readonly Dictionary<string, int> _inventory = new(StringComparer.Ordinal);

	public void AddItem(string itemId, int count = 1)
	{
		if (string.IsNullOrEmpty(itemId) || count <= 0) return;

		if (_inventory.ContainsKey(itemId))
		{
			_inventory[itemId] += count;
		}
		else
		{
			_inventory[itemId] = count;
		}
	}

	public void RemoveItem(string itemId, int count = 1)
	{
		if (string.IsNullOrEmpty(itemId) || count <= 0) return;

		if (!_inventory.ContainsKey(itemId)) return;

		_inventory[itemId] -= count;

		if (_inventory[itemId] <= 0)
		{
			_inventory.Remove(itemId);
		}
	}

	public bool HasItem(string itemId)
	{
		return _inventory.ContainsKey(itemId) && _inventory[itemId] > 0;
	}

	public int GetItemCount(string itemId)
	{
		return _inventory.TryGetValue(itemId, out var count) ? count : 0;
	}

	public IDictionary<string, int> GetAllItems()
	{
		return new Dictionary<string, int>(_inventory, StringComparer.Ordinal);
	}

	public void Clear()
	{
		_inventory.Clear();
	}

	public void LoadFromData(Dictionary<string, int> inventoryData)
	{
		_inventory.Clear();
		foreach (var item in inventoryData)
		{
			if (item.Value > 0)
			{
				_inventory[item.Key] = item.Value;
			}
		}
	}

	protected override void OnInit()
	{
	}
}
