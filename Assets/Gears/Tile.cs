using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
	public static float GearSize = 1.5f;
	public static Tile connectedGear;
	[SerializeField] private Color _baseColor, _offsetColor;
	[SerializeField] private SpriteRenderer _renderer;
	[SerializeField] private GameObject _highlight;
	[SerializeField] private GameObject _gearPrefab;
	[SerializeField] private GameObject _smallGearPrefab;
	private bool _isOffset;
	private GameObject _gear;
	public List<Tile> AdjacentGears = new();
	public float teeth;

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
	public void NewGear()
	{
		_gear = Instantiate(_smallGearPrefab, transform.position, Quaternion.Euler(new Vector3(0, 0, 45)), transform);
		teeth = 4f;
		_gear.transform.localScale = new(GearSize, GearSize, 1);
	}
	public void Rotate(float degrees)
	{
		if(_gear != null)
		{
			_gear.transform.Rotate(new Vector3(0, 0, degrees));
			foreach (Tile tile in AdjacentGears)
			{
				float gearRatio = teeth / tile.teeth;
				tile.Rotate(-degrees * gearRatio);
			}
		}
		
	}
	private void OnMouseOver()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Debug.Log(_gear);
			if (_gear == null)
			{
				if (connectedGear != null && Vector3.Distance(transform.position,connectedGear.transform.position) < 1.5)
				{
					if (_isOffset)
					{
						_gear = Instantiate(_gearPrefab, transform.position, Quaternion.identity, transform);
						teeth = 8f;
						_gear.transform.localScale = new(GearSize, GearSize, 1);
						connectedGear.AdjacentGears.Add(this);
					}
					else
					{
						_gear = Instantiate(_smallGearPrefab, transform.position, Quaternion.Euler(new Vector3(0, 0, 45)), transform);
						teeth = 4f;
						_gear.transform.localScale = new(GearSize, GearSize, 1);
						connectedGear.AdjacentGears.Add(this);
					}
				}
				
			}
			else
			{
				connectedGear = this;
			}
		}
		else if (Input.GetMouseButtonDown(1))
		{
			if( _gear != null)
			{
				Destroy(_gear);
				_gear = null;
			}
			if(connectedGear != null)
			{
				connectedGear = null;
			}
			
		}
	}
}
