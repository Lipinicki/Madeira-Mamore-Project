using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_InventoryDisplay : MonoBehaviour
{
    public Inventory playerInventory;
    public GameObject ItemPrefab;
    public Transform contentParent;

	public void OnEnable()
	{
        Initialize(playerInventory);
	}

	public void Initialize(Inventory inventory)
    {
        foreach (Transform child in contentParent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Item item in inventory.GetItems().Values)
        {
            var itemDisplay = Instantiate(ItemPrefab).GetComponent<UI_ItemDisplay>();
            itemDisplay.transform.SetParent(contentParent, false);
            itemDisplay.Initialize(item);
        }
    }
}
