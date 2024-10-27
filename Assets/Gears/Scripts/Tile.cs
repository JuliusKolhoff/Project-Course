using System.Collections.Generic;
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
	public float MinTorque { get; private set; }
	public int Layer { get; set; }


	[SerializeField] private Color _baseColor, _offsetColor;
	[SerializeField] private SpriteRenderer _renderer;
	[SerializeField] private GameObject _highlight;
	[SerializeField] private GameObject _gearPrefab;
	[SerializeField] private GameObject _smallGearPrefab;
	[SerializeField] private GameObject _enginePrefab;
	[SerializeField] private GameObject _smallEnginePrefab;

	public bool _isOffset;
	private GameObject _gear;
	private GameObject _motor;
	private GridManager _manager;


	public void Init(bool IsOffset, GridManager Manager)
	{
		_isOffset = IsOffset;
		_manager = Manager;
		if(Layer == 1)
		{
			_renderer.enabled = false;
		}
		_renderer.color = IsOffset ? _offsetColor : _baseColor;
	}
	void OnMouseEnter()
	{
		_highlight.SetActive(true);
	}

	void OnMouseExit()
	{
		_highlight.SetActive(false);
	}
	public void DeleteGear()
	{
		if (transform.position.Equals(_manager.GoalPosition))
		{
			_manager.GoalPosition = new(0, 0);
		}
		Teeth = 0;
		MinTorque = 0;
		Destroy(_gear);
		_gear = null;
	}
	public void NewMotor(float teeth)
	{
		if (teeth != 4 && teeth != 8)
		{
			Debug.Log("Only 4 and 8 teeth motors are supported");
			return;
		}
		if (teeth == 4)
		{
			_motor = Instantiate(_smallEnginePrefab, transform.position, Quaternion.identity, transform);
			_motor.name = "SmallMotor";
		}
		else
		{
			_motor = Instantiate(_enginePrefab, transform.position, Quaternion.identity, transform);
			_motor.name = "Motor";
		}
		_motor.transform.localScale = new(GearSize, GearSize, 1);
		_manager.MotorPosition = transform.position;
		NewGear(teeth, 0);
	}
	public void DeleteMotor()
	{
		_manager.MotorPosition = new(-1, -1);
		Destroy(_motor);
		_motor = null;
	}
	public void NewGear(float teeth, float minTorque)
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
		MinTorque = minTorque;

	}	
	public bool Rotate(float degrees, float torque)
	{
		if(degrees == 0)
		{
			CurrentRotation = 0;
			return false;
		}
		if(torque < MinTorque)
		{
			CurrentTorque = 0;
			_manager.StopRotation($"Not Enough torque at gear: {this.gameObject.name} in layer {Layer};	   Only {torque}/{MinTorque} of required Torque");
			return false;
		}
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
					if (_manager.IncludeMachineEfficiency())
					{
						appliedTorque *= .99f;
					}
					if(tile.Layer != Layer)
					{
						appliedRotation = degrees;
						appliedTorque = torque;
					}
					
					if (RotatedTiles.Contains(tile))
					{
						if (appliedRotation != tile.CurrentRotation)
						{
							_manager.StopRotation($"Breaking on Gear: {this.gameObject.name} in layer {Layer}");
							return false;
						}
					}
					else
					{
						if(!tile.Rotate(appliedRotation,appliedTorque))
						{
							return false;
						}
					}
					
				}
			}
		}
		return true;
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
				if (_manager.placeMotor && _manager.MotorPosition.Equals(new(-1, -1)))
				{
					_manager.SetPlaceMotor(false);
					
					if (_isOffset)
					{
						NewMotor(8);
					}
					else
					{
						NewMotor(4);
					}
				}
				else
				{
					if (_isOffset)
					{
						NewGear(8, 0);
					}
					else
					{
						NewGear(4, 0);
					}
				}
				
				
			}
		}
		else if (Input.GetMouseButtonDown(1))
		{
			if( _gear != null)
			{
				DeleteGear();
				if (_manager.MotorPosition.Equals(transform.position))
				{
					DeleteMotor();
				}
			}
			
		}
	}
	
}
