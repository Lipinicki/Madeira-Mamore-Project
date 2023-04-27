using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
	private List<GameEventListener> listeners = new List<GameEventListener>();

	// Used to raise the event.
	// It's main use is for raising it from any kind of object in the game
	// without having to care of who or what is listening (e.g. buttons).
	// The GameListeners should decide what will they do with that.
	public void Raise()
	{
		for (int i = 0; i < listeners.Count; i++)
		{
			listeners[i].OnEventRaised();
		}
	}

	public void Register(GameEventListener listener) => listeners.Add(listener);

	public void Unregister(GameEventListener listener) => listeners.Remove(listener);
}
