using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class ItemButtonEvent : UnityEvent<UI_ItemButtonDisplay> { }

public class UI_ItemButtonDisplay : MonoBehaviour, ISelectHandler, ISubmitHandler, IPointerClickHandler, IDeselectHandler
{
	[Header("Button Visuals")]
	[SerializeField] private TextMeshProUGUI _name;
	[SerializeField] private Image _selectionImage;

	[Space(10)]
	[Header("Button Events")]
	[SerializeField] private ItemButtonEvent _onSelectEvent;
	[SerializeField] private ItemButtonEvent _onDeselectEvent;
	[SerializeField] private ItemButtonEvent _onSubmitEvent;
	[SerializeField] private ItemButtonEvent _onClickEvent;

	[Space(10)]
	[Header("Item data")]
	[SerializeField] private Item _item;

	// Public properties
	public string Name { get => _name.text; set => _name.text = value; }

	public ItemButtonEvent OnClickEvent { get => _onClickEvent; }
	public ItemButtonEvent OnSelectEvent { get => _onSelectEvent; }
	public ItemButtonEvent OnSubmitEvent { get => _onSubmitEvent; }
	public ItemButtonEvent OnClickEvent1 { get => _onClickEvent; }

	public Item Item { get => _item; }

	public static Action<Item> onButtonClick;

	public void Initialize(Item item)
	{
		if (item == null) return;

		_item = item;

		if (_name != null)
			_name.text = item.Name;
	}

	public void ShowSelectionVisuals(bool active)
	{
		Color finalColor = _selectionImage.color;

		if (active)
		{
			finalColor.a = 1f;
			_selectionImage.color = finalColor;
		}
		else
		{
			finalColor.a = 0f;
			_selectionImage.color = finalColor;
		}
	}

	public void Click()
	{
		onButtonClick?.Invoke(_item);
	}

	// Set focus to this button

	public void ObtainSelectionFocus()
	{
		EventSystem.current.SetSelectedGameObject(gameObject);

		_onSelectEvent?.Invoke(this);
	}

	// Callbacks for the EventSystem events

	public void OnSelect(BaseEventData eventData)
	{
		_onSelectEvent?.Invoke(this);
	}

	public void OnSubmit(BaseEventData eventData)
	{
		_onSubmitEvent?.Invoke(this);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		_onClickEvent?.Invoke(this);
	}

	public void OnDeselect(BaseEventData eventData)
	{
		_onDeselectEvent?.Invoke(this);
	}
}

