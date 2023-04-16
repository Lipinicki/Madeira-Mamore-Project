using System;
using UnityEngine;

public class CollectableItemBehaviour : MonoBehaviour, IInteractable, ICollectable
{
	[SerializeField] string itemName;
	[SerializeField] Inventory itensInventoryData;
	[SerializeField] Inventory playerInventory;

	public static Action<Item> onCollectedEvent;

	public void Interact()
	{
		var temp = this as ICollectable;
		temp.Collect(playerInventory);
	}

	void ICollectable.Collect(Inventory inventory)
	{
		var item = itensInventoryData.GetKeyPairValue(itemName); 
		if (item.Value == null) 
		{
			Debug.Log($"Error! item: ({itemName}) does not exist on the {itensInventoryData.name} collection.");
			return;
		}

		if (inventory.GetItems().ContainsKey(item.Key)) return;

		inventory.AddItem(item.Key, item.Value);
		onCollectedEvent?.Invoke(item.Value);

		gameObject.SetActive(false);
	}
}