using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "IventoryData")]
public class Inventory : ScriptableObject
{
	[SerializedDictionary("(Key)Nome", "Item")] 
	public SerializedDictionary<string, Item> Items;

	public void AddItem(string itemName, Item item)
	{
		Items.Add(itemName,item);
	}

	public KeyValuePair<string, Item> GetItem(string itemName)
	{
		if (!Items.ContainsKey(itemName)) return new KeyValuePair<string, Item>("", null);

		return new KeyValuePair<string, Item>(itemName, Items[itemName]);
	}

	public KeyValuePair<string, Item> GetFirstItem()
	{
		if (Items.Count == 0) return new KeyValuePair<string, Item>("", null);

		return Items.First();
	}

	public void ClearInventory()
	{
		Items.Clear();
	}

}
