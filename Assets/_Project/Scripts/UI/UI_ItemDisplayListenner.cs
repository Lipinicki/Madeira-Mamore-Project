using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemDisplayListenner : MonoBehaviour
{
	public TextMeshProUGUI Name;
	public TextMeshProUGUI Text;
	public Image image;

	private void OnEnable()
	{
		UI_ItemDisplay.onButtonClick += OnValueChanged;
	}

	private void OnDisable()
	{
		UI_ItemDisplay.onButtonClick -= OnValueChanged;
	}

	private void OnValueChanged(Item item)
	{
		if (item == null)
		{
			Debug.Log("No item was passed to listenner: " + name);
		}

		if (Name != null)
			Name.text = item.Name;
		if (Text != null)
			Text.text = item.Text;
		if (image != null)
			image.sprite = item.Sprite;
	}
}
