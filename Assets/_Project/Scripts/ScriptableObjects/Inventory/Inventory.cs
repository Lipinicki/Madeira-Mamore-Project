using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "Inventory", menuName = "IventoryData")]
public class Inventory : ScriptableObject
{
	[SerializeField, SerializedDictionary("(Key)Nome", "Item")] 
	private SerializedDictionary<string, Item> _items;

	public SerializedDictionary<string, Item> GetItems() => _items;

	public void AddItem(string itemName, Item item)
	{
		if (_items.ContainsKey(itemName))
		{
			Debug.Log(name + " inventory has already the: " + itemName + " already.", this);
			return;
		}

		_items.Add(itemName,item);
	}

	public Item GetItem(string itemName)
	{
		if (!_items.ContainsKey(itemName)) return null;

		return _items[itemName];
	}

	public KeyValuePair<string, Item> GetKeyPairValue(string itemName)
	{
		if (!_items.ContainsKey(itemName)) return new KeyValuePair<string, Item>("", null);

		return new KeyValuePair<string, Item>(itemName, _items[itemName]);
	}

	public KeyValuePair<string, Item> GetFirstItem()
	{
		if (_items.Count == 0) return new KeyValuePair<string, Item>("", null);

		return _items.First();
	}

	[ButtonMethod]
	public void ClearInventory()
	{
		_items = new SerializedDictionary<string, Item>();
	}
}
