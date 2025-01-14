using System;
using TMPro;
using UnityEngine;

public class Bookmark : MonoBehaviour
{
    public MenuController Controller { get; set; }
	public Action<MenuController> ToPage { get; set; }
	public TMP_Text TextField { get; set; }
	void OnMouseOver()
	{
		if (Input.GetMouseButtonDown(0))
		{
			ToPage.Invoke(Controller);
		}
	}
}
