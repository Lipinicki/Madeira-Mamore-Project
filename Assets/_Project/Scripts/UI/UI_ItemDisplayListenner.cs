using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemDisplayListenner : MonoBehaviour
{
	[Header("Display Informations")]
	public TextMeshProUGUI Name;
	public TextMeshProUGUI Text;
	public Image image;

	public void Activate(Item item)
	{
		if (item == null) return;

		if (Name != null) Name.text = item.Name;
		if (Text != null) Text.text = item.Text;
		if (image != null) image.sprite = item.Sprite;

		gameObject.SetActive(true);

		Invoke(nameof(GetSelection), 0.05f);
	}

	public void Deactivate()
	{
		gameObject.SetActive(false);
	}

	private void GetSelection()
	{
		EventSystem.current.SetSelectedGameObject(gameObject);
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
