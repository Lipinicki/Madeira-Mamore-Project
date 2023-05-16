using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_AutoScrollToSelection : MonoBehaviour
{
    [SerializeField] private RectTransform _viewPort;
    [SerializeField] private RectTransform _content;
    [SerializeField] private float _transitionDuration = 0.2f;
    
    private TransitionHelper _transitionHelper = new TransitionHelper();

	private void Update()
	{
		if (_transitionHelper.InProgress)
        {
            Debug.Log("<color=red>In Progress!</color>");
            _transitionHelper.Update();
            _content.transform.localPosition = _transitionHelper.PosCurrent;
        }
	}

    public void HandleOnSelectionChange(GameObject gObj)
    {
        float viewportTopBorderY = GetBorderTopYLocal(_viewPort.gameObject);
        float viewportBottonBorderY = GetBorderBottonYLocal(_viewPort.gameObject);

        //top
        float targetTopBorderY = GetBorderTopYRelative(gObj);
        float targetTopYWithViewpotOffset = targetTopBorderY + viewportTopBorderY;

        //botton
        float targetBottonBorderY = GetBorderBottonYRelative(gObj);
        float targetBottonYWithViewportOffset = targetBottonBorderY - viewportBottonBorderY;

        //top differece
        float topDiff = targetTopYWithViewpotOffset - viewportTopBorderY;
        if (topDiff > 0f)
        {
			MoveContentObjectYByAmount((topDiff * 100) + GetVerticalLayoutGroup().padding.top);
        }

        //botton difference
        float bottonDiff = targetBottonYWithViewportOffset - viewportBottonBorderY;
        if (bottonDiff < 0f) 
        {
            MoveContentObjectYByAmount((bottonDiff * 100) - GetVerticalLayoutGroup().padding.bottom);
        }
    }

    private float GetBorderTopYLocal(GameObject gObj)
    {
        Vector3 pos = gObj.transform.localPosition / 100f; // Because the obj is top left aligned

        return pos.y;
    }

    private float GetBorderBottonYLocal(GameObject gObj)
    {
        Vector2 rectSize = gObj.GetComponent<RectTransform>().rect.size * 0.01f;
        Vector3 pos = gObj.transform.localPosition / 100f;
        pos.y -= rectSize.y;

        return pos.y;
    }

    private float GetBorderTopYRelative(GameObject gObj)
    {
        float contentY = _content.transform.localPosition.y / 100f;
        float targetBorderUpYLocal = GetBorderTopYLocal(gObj);
        float targetBorderUpYRelative = targetBorderUpYLocal + contentY;

        return targetBorderUpYRelative;
    }

    private float GetBorderBottonYRelative(GameObject gObj)
    {
        float contentY = _content.transform.localPosition.y / 100f;
        float targetBorderBottonYLocal = GetBorderBottonYLocal(gObj);
        float targetBorderBottonYRelative = targetBorderBottonYLocal + contentY;

        return targetBorderBottonYRelative;
    }

    private void MoveContentObjectYByAmount(float amount)
    {
        Vector2 posScrollFrom = _content.transform.localPosition;
        Vector2 posScrollTo = posScrollFrom;
        posScrollTo.y -= amount;

        _transitionHelper.TransitionFromTo(posScrollFrom, posScrollTo, _transitionDuration);
    }

    private VerticalLayoutGroup GetVerticalLayoutGroup()
    {
        VerticalLayoutGroup verticalLayoutGroup = _content.GetComponent<VerticalLayoutGroup>();

        return verticalLayoutGroup;
    }

    // Helper Class for dealling with scroll transition
	private class TransitionHelper
    {
        private float _duration         = 0f;           // Local time from which the transition will be completed in, from start to finish
        private float _timeElapsed      = 0f;           // Keeps track of the time elapsed during the transition
        private float _progress         = 0f;           // Total progress of the transition from 0f to 1f

        private bool _inProgress        = false;

        private Vector2 _posCurrent;
        private Vector2 _posFrom;
        private Vector2 _posTo;

		public bool InProgress { get => _inProgress; }

		public Vector2 PosCurrent { get => _posCurrent; }

        public void Update()
        {
            Tick();

            CalculatePosition();
        }

        public void Clear()
        {
            _duration       = 0f;
            _timeElapsed    = 0f;
            _progress       = 0f;

            _inProgress     = false;
        }

        public void TransitionFromTo(Vector2 posFrom, Vector2 posTo, float duration) 
        {
            Clear();

            _posFrom        = posFrom;
            _posTo          = posTo;
            _duration       = duration;

            _inProgress     = true;
        }

		private void CalculatePosition()
		{
            _posCurrent.x = Mathf.Lerp(_posFrom.x, _posTo.x, _progress);
            _posCurrent.y = Mathf.Lerp(_posFrom.y, _posTo.y, _progress);
		}

		private void Tick()
		{
			if (_inProgress == false) return;

            _timeElapsed += Time.deltaTime;
            _progress = _timeElapsed / _duration;
            if (_progress > 1f) _progress = 1f;

            if (_progress >= 1f)
            {
                TransitionComplete();
            }
		}

		private void TransitionComplete()
		{
			_inProgress = false;
		}
	}
}
