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
	public float CurrentTorque { get; private set; }
	public int Level { get; set; }


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
	public void NewGear(float teeth)
	{
		if (teeth != 4 && teeth != 8)
		{
			Debug.Log("Only 4 and 8 teeth gears are supported");
			return;
		}
		if (teeth == 4)
		{
			_gear = Instantiate(_smallGearPrefab, transform.position, Quaternion.Euler(new Vector3(0, 0, 45)), transform);
			_gear.name = "SmallGear";
		}else
		{
			_gear = Instantiate(_gearPrefab, transform.position, Quaternion.Euler(new Vector3(0, 0, 45)), transform);
			_gear.name = "Gear";
		}
		_gear.transform.localScale = new(GearSize, GearSize, 1);
		Teeth = teeth;

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
				if (tile.HasGear())
				{
				
					float gearRatio = Teeth / tile.Teeth;
					float appliedRotation = -degrees * gearRatio;
					float appliedTorque = torque / gearRatio;
					if(tile.Level != Level)
					{
						appliedRotation = degrees;
						appliedTorque = torque;
					}
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
					NewGear(8);
				}
				else
				{
					NewGear(4);
				}
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
