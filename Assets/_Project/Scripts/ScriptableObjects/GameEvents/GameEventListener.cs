using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    public GameEvent Event;

    // This is the response of the listener to the raised Game Event
    [SerializeField] private UnityEvent Response;

	private void OnEnable()
	{
		Event?.Register(this);
	}

	private void OnDisable()
	{
		Event?.Unregister(this);
	}

	// Responds to the raised event
	public void OnEventRaised()
    {
		Response?.Invoke();
    }
}
