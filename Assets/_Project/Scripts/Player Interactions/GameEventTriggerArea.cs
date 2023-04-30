using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GameEventTriggerArea : MonoBehaviour
{
    [SerializeField] private GameEvent EventToTrigger;
    private const string kPlayerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(kPlayerTag))
        {
            EventToTrigger?.Raise();
        }
    }
}
