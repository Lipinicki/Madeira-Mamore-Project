using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ui_ItemInfo : MonoBehaviour
{
    [SerializeField] private string ItemName;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private Image uiItemImage;
    [SerializeField] private TextMeshProUGUI uiDescription;

	private void OnValidate()
	{
		SetItemInfo();
	}

	public void ChangeItemInfo(string itemName)
	{
		ItemName = itemName;
		SetItemInfo();
	}

	private void SetItemInfo()
	{
		if (playerInventory == null || uiItemImage == null || uiDescription == null) return;

		var item = playerInventory.GetItem(ItemName);
		if (item.Value == null)
		{
			Debug.Log($"Error! item: ({ItemName}) does not exist on the {playerInventory.name} collection.");
			return;
		}

		uiItemImage.sprite = item.Value.Sprite;
		uiDescription.text = item.Value.Text;
	}
}
