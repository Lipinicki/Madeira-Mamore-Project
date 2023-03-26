using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemButton : MonoBehaviour
{
	private UI_ItensFromInventory inventoryManager;
	private Button button;

	private void Awake()
	{
		button = GetComponent<Button>();
	}

	private void Start()
	{
		if (EventSystem.current.currentSelectedGameObject == null)
		{
			EventSystem.current.SetSelectedGameObject(gameObject);
		}
	}

	public void SetInventoryManagerReference(UI_ItensFromInventory manager)
	{
		inventoryManager = manager;
	}

}
