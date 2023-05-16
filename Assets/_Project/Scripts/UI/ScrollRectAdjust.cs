using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectAdjust : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    private RectTransform contentRect;
    private List<GameObject> elements = new List<GameObject>();

    void Start()
    {
        contentRect = scrollRect.content;
        
        foreach (GameObject go in elements)
        {
            EventTrigger eventTrigger = go.GetComponent<EventTrigger>();
            Debug.Log(go.name, go);
            eventTrigger.AddListener(EventTriggerType.UpdateSelected, OnSelect);
        }
        scrollRect.horizontalNormalizedPosition = 0;
    }

    public void AddListenner(GameObject gameObject)
    {
        elements.Add(gameObject);
    }

    public void RemoveListenner(GameObject gameObject)
    {
        elements.Remove(gameObject);
    }

    private void OnDestroy()
    {
        foreach (GameObject go in elements)
        {
            EventTrigger eventTrigger = go.GetComponentInChildren<EventTrigger>();
            eventTrigger.RemoveListener(EventTriggerType.UpdateSelected, OnSelect);
        }
    }

    private void OnSelect(BaseEventData baseEventData)
    {
        RectTransform selectedRectTransform = baseEventData.selectedObject.GetComponent<RectTransform>();
        
        var height = scrollRect.GetComponent<RectTransform>().rect.height;
        var contentHeight = contentRect.rect.height;
        var overflow = (contentHeight - height) / 2f; 
        
        var lowerBorder = overflow - selectedRectTransform.offsetMin.y;
        var topBorder = -(overflow + (selectedRectTransform.offsetMax.y - contentHeight));

        if (lowerBorder > contentRect.anchoredPosition.x)
            contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, lowerBorder);
        else if (topBorder < contentRect.anchoredPosition.x)
            contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, topBorder);
    }
}
