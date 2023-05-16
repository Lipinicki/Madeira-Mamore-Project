using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "Empty Speech", menuName = "Dialogue System/New Speech", order = 1)]

[Serializable]
public class Speech : ScriptableObject
{
    public Sprite characterPortrait;
    public string characterName;
    [Multiline]
    public string speechText;
    public bool hasAudio = false;
    [ConditionalField(nameof(hasAudio))] public AudioClip speechAudio;
}
