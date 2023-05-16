using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to create scriptable objects who needs a list of objects to notify.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class CallbackHandlerSO<T> : ScriptableObject
{
    protected List<T> callbacks = new List<T>();

    public void SubscribeCallback(T callback)
    {
        callbacks.Add(callback);
    }

    public void UnsubscribeCallback(T callback)
    {
        callbacks.Remove(callback);
    }
}
