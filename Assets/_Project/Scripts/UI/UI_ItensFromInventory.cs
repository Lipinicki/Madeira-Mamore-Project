using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ItensFromInventory : MonoBehaviour
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private GameObject contentItemPrefab;
    [SerializeField] private Transform contentParent;
    [SerializeField] private Ui_ItemInfo itemInfo;

	public void Start()
	{
        var itemName = playerInventory.GetFirstItem().Key;

        AddItemToInventoryList(itemName);
        itemInfo.ChangeItemInfo(itemName);
	}

	public void AddItemToInventoryList(string itemName)
    {
        var obj = Instantiate(contentItemPrefab, contentParent);
        var uiButton = obj.GetComponent<UI_ItemButton>();
        uiButton.SetInventoryManagerReference(this);
    }

    public void ShowItemInfo(string itemName)
    {
        itemInfo.ChangeItemInfo(itemName);
    }
}
