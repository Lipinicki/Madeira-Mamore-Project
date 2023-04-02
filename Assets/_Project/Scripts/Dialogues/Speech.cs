using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Empty Speech", menuName = "Dialogue System/New Speech", order = 1)]

[Serializable]
public class Speech : ScriptableObject
{
    public Sprite characterPortrait;
    public string characterName;
    public string speechText;
    public AudioClip speechAudio;

}
