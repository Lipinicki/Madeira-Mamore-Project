using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "Inventory", menuName = "IventoryData")]
public class Inventory : ScriptableObject
{
	[SerializeField, SerializedDictionary("(Key)Nome", "Item")] 
	private SerializedDictionary<string, Item> Items;

	public SerializedDictionary<string, Item> GetItems() => Items;

	public void AddItem(string itemName, Item item)
	{
		if (Items.ContainsKey(itemName))
		{
			Debug.Log(name + " inventory has already the: " + itemName + " already.", this);
			return;
		}

		Items.Add(itemName,item);
	}

	public Item GetItem(string itemName)
	{
		if (!Items.ContainsKey(itemName)) return null;

		return Items[itemName];
	}

	public KeyValuePair<string, Item> GetKeyPairValue(string itemName)
	{
		if (!Items.ContainsKey(itemName)) return new KeyValuePair<string, Item>("", null);

		return new KeyValuePair<string, Item>(itemName, Items[itemName]);
	}

	public KeyValuePair<string, Item> GetFirstItem()
	{
		if (Items.Count == 0) return new KeyValuePair<string, Item>("", null);

		return Items.First();
	}

	[ButtonMethod]
	public void ClearInventory()
	{
		Items = new SerializedDictionary<string, Item>();
	}
}
