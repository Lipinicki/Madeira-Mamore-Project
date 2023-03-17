using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class LedgeController : MonoBehaviour
{
    [SerializeField] private Transform standPosition;
    [SerializeField] private Collider grabDetection;

    public Vector3 GetStandPosition()
    {
        return standPosition.position;
    }
}
