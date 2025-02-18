using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
	public static float GearSize = 1.5f;
	public static List<Tile> RotatedTiles = new ();

	public List<Tile> AdjacentTiles = new();
	public float Teeth { get; private set; }
	private float _currentRotation;
	private float _currentTorque;
	private float _minTorque;
	private float _minRpm;

	public GameObject Gear { get { return _gear; }}
	public float CurrentRotation { get { return _currentRotation; }  set { _currentRotation = value; UpdateText(); } }
	public float CurrentTorque { get { return _currentTorque; } set {  _currentTorque = value; UpdateText(); } }
	public float MinTorque { get { return _minTorque; } private set { _minTorque = value; UpdateText(); } }
	public float MinRpm { get { return _minRpm; } private set {  _minRpm = value; UpdateText(); } }
	public int Layer { get; set; }


	[SerializeField] private Color _baseColor, _offsetColor, _permanentColor, _goalColor;
	[SerializeField] private SpriteRenderer _renderer;
	[SerializeField] private GameObject _highlight;
	[SerializeField] private GameObject _gearPrefab;
	[SerializeField] private GameObject _smallGearPrefab;
	[SerializeField] private GameObject _enginePrefab;
	[SerializeField] private GameObject _smallEnginePrefab;
	[SerializeField] private GameObject _infoObjPrefab;
	[SerializeField] private GameObject _infoPopupPrefab;

	public Transform Canvas { get; set; }


	public bool _isOffset;
	private bool _permanent = false;
	private GameObject _gear;
	private GameObject _motor;
	private GridManager _manager;
	private GameObject _infoTextObj;

	float lastRotation = 0;

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
	private void UpdateText()
	{
		if (_infoTextObj != null)
		{
			_infoTextObj.GetComponent<TMP_Text>().text = $"Min Rpm: {MinRpm}\nCurrentRpm: {Mathf.Abs(CurrentRotation)}\nMin Torque: {MinTorque}\nCurrentTorque: {CurrentTorque}";
		}
	}
	void OnMouseEnter()
	{
		_highlight.SetActive(true);
		if(_infoTextObj != null)
        {
			_infoTextObj.SetActive(true);
		}
	}
	public void ResetRotation()
	{
		if (HasGear())
		{
			float rotation;
			if (Teeth == 4)
			{
				rotation = 45;
			}
			else
			{
				rotation = 0;
			}
			Gear.transform.eulerAngles = new(0, 0, rotation);
		}
	}
	private void Update()
	{
		if (HasGear())
		{
			Debug.DrawRay(transform.position, DegreeToVector2(Gear.transform.rotation.eulerAngles.z, 17f), Color.green, 1);
			//Debug.DrawLine(transform.position, transform.position + (Quaternion.Euler(transform.rotation.eulerAngles) * new Vector3(1,1,1) * 10));
			float z = Gear.transform.eulerAngles.z;
			float rotation;
			if (Teeth == 4)
			{
				rotation = (Mathf.Round((z - 45) / 90f) * 90) + 45;
			}
			else
			{
				rotation = Mathf.Round(z / 45f) * 45f;
			}
			Debug.DrawRay(transform.position, DegreeToVector2(rotation, 17f), Color.red, 1);
		}
	}
	void OnMouseExit()
	{
		_highlight.SetActive(false);
		if (_infoTextObj != null)
		{
			_infoTextObj.SetActive(false);
		}
	}
	public void DeleteGear()
	{
		if (_permanent)
        {
			return;
        }
  //      foreach (Vector2 position in _manager.GoalPositions)
  //      {
		//	if (transform.position.Equals(position) && Layer == 0)
		//	{
		//		_manager.GoalPositions.Remove(position);
		//		break;
		//	}
		//}
		Teeth = 0;
		MinTorque = 0;
		MinRpm = 0;
		Destroy(_gear);
		Destroy(_infoTextObj);
		_gear = null;
		_infoTextObj = null;
	}
	public void NewMotor(float teeth)
	{
		_permanent = true;
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
		_manager.MotorTorque = _manager.MinTorque;
		NewGear(teeth, 0,0);
	}
	public void DeleteMotor()
	{
		if (_permanent)
        {
			return;
        }
		_manager.MotorPosition = new(-1, -1);
		Destroy(_motor);
		Destroy(_infoTextObj);
		_motor = null;
	}
	public void NewGear(float teeth, float minTorque,float minRpm)
    {
		NewGear(teeth, minTorque, minRpm, false);
    }
	public void NewGear(float teeth, float minTorque,float minRpm, bool isPermanent)
	{
		if (isPermanent)
        {
			_permanent = true;
        }
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
		if (_permanent)
        {
			_gear.GetComponent<SpriteRenderer>().color = _permanentColor;
        }
		foreach (Vector2 position in _manager.GoalPositions)
		{
			if (transform.position.Equals(position))
			{
				_gear.GetComponent<SpriteRenderer>().color = _goalColor;
				break;
			}
		}
		_infoTextObj = Instantiate(_infoObjPrefab, _gear.transform.position + new Vector3(0, 1.75f, 0), Quaternion.identity,transform);
		_infoTextObj.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
		_infoTextObj.SetActive(false);
		Teeth = teeth;
		MinTorque = minTorque;
		MinRpm = minRpm;
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
			CurrentTorque = torque;
			CurrentRotation = degrees;
			RotatedTiles.Add(this);
			foreach (Tile tile in RotatedTiles)
			{
				tile.ResetRotation();
			}
			//_manager.StopRotation($"Not Enough torque at gear: {this.gameObject.name} in layer {Layer};	   Only {torque}/{MinTorque} of required Torque");
			NewInfoPopup($"Nicht genug Torque(Drehmoment) bei dem Zahnrad: {this.gameObject.name} im Layer {Layer + 1};	   Nur {torque}/{MinTorque} der min. Torque");
			return false;
		}
		if (Mathf.Abs(degrees) < MinRpm)
		{
			CurrentRotation = degrees;
			CurrentTorque = torque;
			RotatedTiles.Add(this);
			foreach (Tile tile in RotatedTiles)
			{
				tile.ResetRotation();
			}
			//_manager.StopRotation($"Not Enough Rpm at gear: {this.gameObject.name} in layer {Layer};	   Only {Mathf.Abs(degrees)}/{MinRpm} of required Rpm");
			NewInfoPopup($"Nicht genug RPM(Geschwindigkeit) bei dem Zahnrad: {this.gameObject.name} im Layer {Layer + 1};	   Nur {Mathf.Abs(degrees)}/{MinRpm} der min. Rpm");
			return false;
		}
		if (HasGear())
		{
			RotatedTiles.Add(this);
			_gear.transform.Rotate(new Vector3(0, 0, degrees));
			lastRotation = Mathf.Round(degrees);
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
							//_manager.StopRotation($"Breaking on Gear: {this.gameObject.name} in layer {Layer}");
							string[] parts = gameObject.name.Split(" ");
							if (_permanent)
							{
								NewInfoPopup($"Das permanente Zahnrad an der Stelle: ({int.Parse(parts[1]) + 1}|{int.Parse(parts[2]) + 1}) im Layer {Layer + 1} ist zerbrochen: \n Level wird neugestartet");
								_manager.RestartLevel();
							}
							else
							{
								NewInfoPopup($"Das Zahnrad an der Stelle: ({int.Parse(parts[1]) + 1}|{int.Parse(parts[2]) + 1}) im Layer {Layer + 1} ist zerbrochen: \n Zahnrad wurde entfernt");
								DeleteGear();
							}
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
				if (_manager.PlaceMotor && _manager.MotorPosition.Equals(new(-1, -1)))
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
						NewGear(8, _manager.MinTorque, _manager.MinRpm);
					}
					else
					{
						NewGear(4, _manager.MinTorque, _manager.MinRpm);
					}
					if (_manager.PlaceGoal)
					{
						_manager.NewGoalPosition(transform.position);
					}
				}
				_infoTextObj.SetActive(true);
				
			}
		}
		else if (Input.GetMouseButtonDown(1))
		{
			if( _gear != null)
			{
				DeleteGear();
				if (_manager.MotorPosition.Equals(transform.position) && Layer == 0)
				{
					DeleteMotor();
				}
			}
			
		}
	}

	public void ResetRotationGradient(float dt)
	{
		ResetRotation();
		//if (HasGear())
		//{
		//	int desiredRotation;
		//	int z = Mathf.RoundToInt(Gear.transform.eulerAngles.z);
		//	if (Teeth == 4)
		//	{
		//		desiredRotation = (int)(Mathf.Round((z - 45) / 90f) * 90) + 45;
		//		//if((z-(45 * Mathf.Sign(z)))%90 == 0)
		//		//{
		//		//	return;
		//		//}
		//		//rotation = (Mathf.Round((z - 45) / 90f) * 90) + 45;
		//	}
		//	else
		//	{
		//		desiredRotation = (int)Mathf.Round(z / 90f) * 90;

		//		//if(Mathf.Abs(z)%45 == 0)
		//		//{
		//		//	return;
		//		//}
		//		//rotation = Mathf.Round(z / 90f) * 90;
		//	}
		//	desiredRotation = (int)(Mathf.Round((z - 45) / 90f) * 90) + 45;
		//	//Debug.Log($" Z: {z}     Desired: {desiredRotation}      Achieved: {z.Equals(desiredRotation)}");
		//	if (z.Equals(desiredRotation))
		//	{
		//		return;
		//	}
		//	Gear.transform.eulerAngles = new(0, 0, Mathf.Round(Gear.transform.eulerAngles.z));
		//	Gear.transform.Rotate(new(0, 0, -lastRotation));
		//}
	}
	public Vector2 DegreeToVector2(float degree, float length)
	{
		return RadianToVector2(degree * Mathf.Deg2Rad) * length;
	}
	public static Vector2 RadianToVector2(float radian)
	{
		return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
	}
	private void NewInfoPopup(string message)
	{
		GameObject obj = Instantiate(_infoPopupPrefab,Canvas);
		obj.GetComponent<InfoPopup>().SetText(message);
		obj.GetComponent<InfoPopup>().Manager = _manager;
		if (_manager.HasEnabledPopup())
		{
			_manager.SetPopupEnabled(false);
		}
		if (_manager.IsPopupButtonEnabled())
		{
			_manager.SetPopupButtonEnabled(false);
			obj.GetComponent<InfoPopup>().disabledButton = true;
		}
		_manager.SetTilesVisible(false);
	}
}

