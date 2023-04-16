using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorOpening : MonoBehaviour
{
    [SerializeField] private UnityEvent onDoorOpenning;

    public void TriggerDoorEvent()
    {
        onDoorOpenning?.Invoke();
    }
}
