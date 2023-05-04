using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_InventoryDisplay : MonoBehaviour
{
    [Header("Inventory Data")]
	[SerializeField] private Inventory _playerInventory;

    [Space(10)]
    [Header("Item Info Display")]
    [SerializeField] private UI_ItemDisplayListenner _infoDisplay;

    [Space(10)]
    [Header("Scroll Objects")]
    [SerializeField] private RectTransform _content;
    [SerializeField] private GameObject _prefabItem;

    [Space(10)]
    [Header("Scroll View Events")]
    [SerializeField] private ItemButtonEvent _eventItemClicked;
	[SerializeField] private ItemButtonEvent _eventItemSelected;
	[SerializeField] private ItemButtonEvent _eventItemSubmited;
    [SerializeField] private ItemButtonEvent _eventItemDeselect;
    [SerializeField] private ItemButtonEvent _eventItemCancel;
    [SerializeField] private ItemButtonEvent _eventItemHoverEnter;

    [Space(10)]
    [Header("Default Selected Index")]
    [SerializeField] private int _defaultSelectedIndex = 0;
	[SerializeField] private float _firstSelectionDelay = 1f;

    [Space(10)]
    [Header("For Testing Only!")]
    [SerializeField] private int _testButtonCount = 1;  // Number of testing buttons

	public void OnEnable()
	{
        if (_testButtonCount > 0)
        {
            TestCreateItems(_testButtonCount);
            UpdateAllButtonNavigationalReferences();
        }

        // Initialization
        Initialize();

		StartCoroutine(DelayChildSelection(_defaultSelectedIndex));
	}

	public void OnDisable()
	{
		Dispose();
	}

	// Initialize the ui with the items and its informations
	public void Initialize()
	{
        // Populates the content object with the itens
        foreach (Item item in _playerInventory.GetItems().Values)
		{
			var itemDisplay = CreateItem(item.Name);
			itemDisplay.Initialize(item);
		}

		UpdateAllButtonNavigationalReferences();

		gameObject.SetActive(true);
	}

    // Dispose all information from the content object
	public void Dispose()
	{
		// Destroy all childs
		for (int i = 0; i < _content.childCount; i++)
		{
			Destroy(_content.GetChild(i).gameObject);
		}
	}

	public void SelectChild(int index)
    {
        int childCount = _content.transform.childCount;

        if (index < 0 || index >= childCount) 
        {
            return; // Out of range
        }

        GameObject childObject = _content.transform.GetChild(index).gameObject;
        UI_ItemButtonDisplay item = childObject.GetComponent<UI_ItemButtonDisplay>();
        item.ObtainSelectionFocus();
    }

    // Used to delay selection to let unity initialize components before making the selection
    public IEnumerator DelayChildSelection(int index)
    {
        yield return new WaitForSeconds(_firstSelectionDelay); // default to 1 secon, but it's cosidered a MAGIC NUMBER

        SelectChild(index);
    }

	private void UpdateAllButtonNavigationalReferences()
	{
        UI_ItemButtonDisplay[] children = _content.transform.GetComponentsInChildren<UI_ItemButtonDisplay>();

        if (children.Length < 2)
        {
            return; // Navigation will not work, the array must contain at least 2 children
        }

        UI_ItemButtonDisplay item;
        Navigation navigation;

        for (int i = 0; i < children.Length; i++)
        {
            item = children[i];

            navigation = item.gameObject.GetComponent<Button>().navigation;

            navigation.selectOnUp = GetNavigationUp(i, children.Length);
            navigation.selectOnDown = GetNavigationDown(i, children.Length);

			item.gameObject.GetComponent<Button>().navigation = navigation;
		}
	}

	private Selectable GetNavigationDown(int indexCurrent, int totalEntries)
	{
        UI_ItemButtonDisplay item;

        if (indexCurrent == totalEntries - 1)       // Last one
        {
           item = _content.transform.GetChild(0).GetComponent<UI_ItemButtonDisplay>(); // Loops navigation down
        }
        else
        {
            item = _content.transform.GetChild(indexCurrent + 1).GetComponent<UI_ItemButtonDisplay>(); // Gets next item reference
		}

        return item.GetComponent<Selectable>();
	}

	private Selectable GetNavigationUp(int indexCurrent, int totalEntries)
	{
		UI_ItemButtonDisplay item;

		if (indexCurrent == 0)
		{
			item = _content.transform.GetChild(totalEntries - 1).GetComponent<UI_ItemButtonDisplay>(); // Loops navigation down
		}
		else
		{
            item = _content.transform.GetChild(indexCurrent - 1).GetComponent<UI_ItemButtonDisplay>(); // Gets previous item reference
		}

        return item.GetComponent<Selectable>();
	}

	private void TestCreateItems(int count)
	{   
		for (int i = 0; i < count; i++)
        {
            CreateItem(new Item("Test Item", "Text", null).Name + i);
        }
	}

	private UI_ItemButtonDisplay CreateItem(string itemName)
	{
        GameObject gObj;
        UI_ItemButtonDisplay item;

        // Instantiate and set default values
        gObj = Instantiate(_prefabItem, Vector3.zero, Quaternion.identity);
        gObj.transform.SetParent(_content.transform);       // Set Parent First
        gObj.transform.localScale       = Vector3.one;            // Maintain Scale
        gObj.transform.localPosition    = Vector3.zero;
        gObj.transform.rotation         = Quaternion.Euler(Vector3.zero);
        gObj.name                       = itemName;

        // Set Item reference value
        item                            = gObj.GetComponent<UI_ItemButtonDisplay>();
        item.Name                       = itemName;

        // Add Event Listenes'
        item.OnSelectEvent.AddListener((ItemButton) => { HandleOnSelectButton(item); });
        item.OnClickEvent.AddListener((ItemButton) => { HandleOnClickButton(item); });
        item.OnSubmitEvent.AddListener((ItemButton) => { HandleOnSubmitButton(item); });
        item.OnSelectEvent.AddListener((ItemButton) => { HandleOnDeselectButton(item); });
        item.OnCancelEvent.AddListener((ItemButton) => { HandleOnCancelButton(item); });
        item.OnHoverEnterEvent.AddListener((ItemButton) => { HandleOnHoverEnterButton(item); });

        return item;
	}

	private void HandleOnSelectButton(UI_ItemButtonDisplay itemButton)
    {
        UI_AutoScrollToSelection autoScrollToSelection = GetComponent<UI_AutoScrollToSelection>();
        autoScrollToSelection.HandleOnSelectionChange(itemButton.gameObject);

        _eventItemSelected?.Invoke(itemButton);
    }

	private void HandleOnDeselectButton(UI_ItemButtonDisplay item)
	{
        _eventItemDeselect?.Invoke(item);
	}

	private void HandleOnClickButton(UI_ItemButtonDisplay itemButton)
	{
		_infoDisplay.Activate(itemButton.Item);

		_eventItemClicked?.Invoke(itemButton);
	}

	private void HandleOnSubmitButton(UI_ItemButtonDisplay itemButton)
	{
        _infoDisplay.Activate(itemButton.Item);

        _eventItemSubmited?.Invoke(itemButton);
	}

	private void HandleOnCancelButton(UI_ItemButtonDisplay itemButton)
    {
        _eventItemCancel?.Invoke(itemButton);
    }

    private void HandleOnHoverEnterButton(UI_ItemButtonDisplay itemButton)
    {
        _eventItemHoverEnter?.Invoke(itemButton);
    }
}
