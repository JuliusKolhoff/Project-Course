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
	[SerializeField] private GameObject _smallGearPrefab;
	private bool _isOffset;
	private GameObject _gear;

	public void Init(bool isOffset)
	{
		_isOffset = isOffset;
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
			if(_isOffset)
            {
				_gear = Instantiate(_gearPrefab, transform.position, Quaternion.identity, transform);
				_gear.transform.localScale = new(GearSize, GearSize, 1);
			} else
            {
				_gear = Instantiate(_smallGearPrefab, transform.position, Quaternion.Euler(new Vector3(0, 0, 45)), transform);
				_gear.transform.localScale = new(GearSize / 2, GearSize / 2, 1);
			}
			
		}
		else if (Input.GetMouseButtonDown(1) && _gear != null)
		{
			Destroy(_gear);
			_gear = null;
		}
	}
}
