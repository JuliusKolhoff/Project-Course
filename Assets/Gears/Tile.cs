using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
	public static float GearSize = 1.5f;
	public static List<Tile> RotatedTiles = new ();

	public List<Tile> AdjacentTiles = new();
	public float Teeth { get; private set; }
	public float CurrentRotation { get; private set; }
	[SerializeField] private float CurrentTorque;

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
	public void NewGear()
	{
		_gear = Instantiate(_smallGearPrefab, transform.position, Quaternion.Euler(new Vector3(0, 0, 45)), transform);
		Teeth = 4f;
		_gear.transform.localScale = new(GearSize, GearSize, 1);
	}
	public void Rotate(float degrees, float torque)
	{
		if(HasGear())
		{
			RotatedTiles.Add(this);
			_gear.transform.Rotate(new Vector3(0, 0, degrees));
			CurrentRotation = degrees;
			CurrentTorque = torque;
			foreach (Tile tile in AdjacentTiles)
			{
				if (tile.HasGear() )
				{
					float gearRatio = Teeth / tile.Teeth;
					float appliedRotation = -degrees * gearRatio;
					float appliedTorque = torque / gearRatio;
					if (RotatedTiles.Contains(tile))
					{
						if (appliedRotation != tile.CurrentRotation)
						{
							Debug.Log("Gears Break");
						}
					}
					else
					{
						tile.Rotate(appliedRotation,appliedTorque * .99f);
					}
					
				}
			}
		}
		
	}
	public bool HasGear()
	{
		return _gear != null;
	}
	private void OnMouseOver()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (_gear == null)
			{
					if (_isOffset)
					{
						_gear = Instantiate(_gearPrefab, transform.position, Quaternion.identity, transform);
						Teeth = 8f;
					}
					else
					{
						_gear = Instantiate(_smallGearPrefab, transform.position, Quaternion.Euler(new Vector3(0, 0, 45)), transform);
						Teeth = 4f;
					}
				_gear.transform.localScale = new(GearSize, GearSize, 1);
			}
		}
		else if (Input.GetMouseButtonDown(1))
		{
			if( _gear != null)
			{
				Destroy(_gear);
				_gear = null;
			}
			
		}
	}
	
}
