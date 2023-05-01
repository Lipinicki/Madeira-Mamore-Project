using System.Collections.Generic;
using UnityEngine;

public class UI_InventoryDisplay : MonoBehaviour
{
    public Inventory playerInventory;
    public GameObject ItemPrefab;
    public Transform contentParent;
    public ScrollRectAdjust scrollAdjust;

    private List<GameObject> disposableItems = new List<GameObject>();

	public void OnEnable()
	{
        Initialize(playerInventory);
	}

    // Initialize the ui with the items and its informations
	public void Initialize(Inventory inventory)
    {
        // Reset buttons already on the Object
        foreach (Transform child in contentParent.transform)
        {
            Destroy(child.gameObject);
        }

        // Populates the content object with the itens
        foreach (Item item in inventory.GetItems().Values)
        {
            var itemDisplay = Instantiate(ItemPrefab).GetComponent<UI_ItemDisplay>();
            scrollAdjust.AddListenner(itemDisplay.gameObject);
            disposableItems.Add(itemDisplay.gameObject);

            itemDisplay.transform.SetParent(contentParent, false);
            itemDisplay.Initialize(item);
        }

        gameObject.SetActive(true);
    }

	public void Dispose()
	{
        // Need to unsubscribe all objects avoid memory leaks
		for (int i = 0; i < disposableItems.Count; i++)
        {
            scrollAdjust.RemoveListenner(disposableItems[i].gameObject);
        }

        gameObject.SetActive(false);
	}
}
