using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Tile : MonoBehaviour
{
	public static float GearSize = 1.5f;
	[SerializeField] private Color _baseColor, _offsetColor;
	[SerializeField] private SpriteRenderer _renderer;
	[SerializeField] private GameObject _highlight;
	[SerializeField] private GameObject _gearPrefab;
	private GameObject _gear;

	public void Init(bool isOffset)
	{
		_renderer.color = isOffset ? _offsetColor : _baseColor;
	} 

	void OnMouseEnter()
	{
		_highlight.SetActive(true);
	}

	void OnMouseExit()
	{
		_highlight.SetActive(false);
	}
	private void OnMouseOver()
	{
		if (Input.GetMouseButtonDown(0) && _gear == null)
		{
				_gear = Instantiate(_gearPrefab, transform.position, Quaternion.identity, transform);
			_gear.transform.localScale = new(GearSize,GearSize,1);
		}
		else if (Input.GetMouseButtonDown(1) && _gear != null)
		{
			Destroy(_gear);
			_gear = null;
		}
	}
}
