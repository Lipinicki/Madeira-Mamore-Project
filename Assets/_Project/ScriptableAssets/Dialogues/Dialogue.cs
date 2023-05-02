using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Empty Dialogue", menuName = "Dialogue System/New Dialogue", order = 0)]

[Serializable]
public class Dialogue : ScriptableObject
{
   [SerializeField] public Speech[] speechList;

   public int SpheechCount => speechList.Length;
}
