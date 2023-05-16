using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Item
{
	public string Name;
	[Multiline]
	public string Text;
	public Sprite Sprite;

	public Item(string name, string text, Sprite sprite)
	{
		Name = name;
		Text = text;
		Sprite = sprite;
	}
}
