using System;
using UnityEngine.UI;

[Serializable]
public class Item
{
	public string Name;
	public string Text;
	public Image ImageToShow;

	public Item(string name, string text, Image imageToShow)
	{
		Name = name;
		Text = text;
		ImageToShow = imageToShow;
	}
}
