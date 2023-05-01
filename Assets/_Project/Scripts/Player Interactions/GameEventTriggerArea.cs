using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GameEventTriggerArea : MonoBehaviour
{
    [SerializeField] private GameEvent eventToTrigger;
    [SerializeField] private bool triggerOnlyOnce = true;
    
    private int timesWasTriggered = 0;
    private const string kPlayerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if(triggerOnlyOnce && timesWasTriggered >= 1) return;

        if (other.CompareTag(kPlayerTag))
        {
            eventToTrigger?.Raise();
            timesWasTriggered++;
        }
    }
}
