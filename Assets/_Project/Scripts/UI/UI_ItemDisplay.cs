using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemDisplay : MonoBehaviour, ISelectHandler
{
	public TextMeshProUGUI Name;
    public TextMeshProUGUI Text;
    public Image image;
	public EventTrigger attatchedEventTrigger;

	[SerializeField] private Item item;

	public static Action<Item> onButtonClick;

	public void Initialize(Item item)
	{
		if (item == null) return;

		this.item = item;
		if (Name != null)
			Name.text = item.Name;
		if (Text != null) 
			Text.text = item.Text;
		if (image != null)
			image.sprite = item.Sprite;
	}

	public void Click()
	{
		onButtonClick?.Invoke(item);
	}

	// Callback for the OnSelect event on EventSystem
	public void OnSelect(BaseEventData eventData)
	{
		Click();
	}
}
