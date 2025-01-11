using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bookmark : MonoBehaviour
{
    [SerializeField] private MenuController _controller;
	[SerializeField] private BookmarkType page;
	enum BookmarkType
	{
		Gears,
		Electricity,

	}
	void OnMouseOver()
	{
		if (Input.GetMouseButtonDown(0))
		{
			switch (page)
			{
				case BookmarkType.Electricity: _controller.GoToElectricityPage(); break;
				case BookmarkType.Gears: _controller.GoToGearPage(); break;
			}
		}
	}
}
