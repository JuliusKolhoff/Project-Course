using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class BoardController : MonoBehaviour
{
	public List<GameObject> objects;
	public Dictionary<PlacableObject, List<GameObject>> PlacedObjects = new();
	private List<GameObject> wires;
	[SerializeField] private GameObject _battery, _wire, _resistor, _lamp, _switch;
	[SerializeField] private GameObject _board;
	[SerializeField] private Button _calculateButton;
	[SerializeField] private LayerMask mask,uiMask;
	public GameObject CurrentObj { get; set; }
	private PlacableObject _currentObject;
	private Dictionary<PlacableObject, int> _unlockedObjects = new();
	private TMP_Text _currentObjtext;
	public TMP_Text RgesText;
	private float _resistance;
	private BatteryController _batteryController;
	private GameObject movingObj = null;
	private Vector3 movingObjOriginalPos;
	[SerializeField] private GameObject _noneButton, _resistorButton, _lampButton, _switchButton, _batteryButton;
	void Start()
	{
		int i = 0;
		while (Enum.IsDefined(typeof(PlacableObject), i))
		{
			PlacedObjects.Add((PlacableObject)i, new List<GameObject>());
			_unlockedObjects.Add((PlacableObject)i, 0);
			i++;
		}
		//Unlock None to make it selectable
		Unlock(PlacableObject.Bewegen, 1);
		//Needed Material for first Quest
		Unlock(PlacableObject.Batterie, 1);
		Unlock(PlacableObject.Lampe, 1);
		objects = new();
		wires = new();
		_currentObjtext = GetComponent<TMP_Text>();
		_currentObject = PlacableObject.Batterie;
		_calculateButton.interactable = false;
		CalculateButtonValues();
	}
	public enum PlacableObject
	{
		Bewegen,
		Batterie,
		Resistor,
		Schalter,
		Lampe
	}
	public void MakeNoneCurrentObj() => SelectPlaceable(PlacableObject.Bewegen);
	public void MakeBatteryCurrentObj() => SelectPlaceable(PlacableObject.Batterie);
	public void MakeResistorCurrentObj() => SelectPlaceable(PlacableObject.Resistor);
	public void MakeSwitchCurrentObj() => SelectPlaceable(PlacableObject.Schalter);
	public void MakeLampCurrentObj() => SelectPlaceable(PlacableObject.Lampe);
	public BatteryController GetBattery()
	{
		return _batteryController;
	}
	public void ChangeResistance(string value)
	{
		if (value.Replace(" ", "") != "")
			_resistance = int.Parse(value.Replace(" ", ""));
	}
	public void Unlock(PlacableObject obj, int amount)
	{
		_unlockedObjects[obj] += amount;
		CalculateButtonValues();
		//throw new NotImplementedException("TODO: Unlock Methode muss noch Implementiert werden!!!");
	}
	public void ResetBoard()
	{
		_batteryController = null;
		_calculateButton.interactable = false;
		foreach (PlacableObject key in PlacedObjects.Keys)
		{
			PlacedObjects[key].Clear();
		}
		GameObject item;
		while ((item = objects.FirstOrDefault()) != null)
		{
			Destroy(item);
			objects.Remove(item);
		}
		while ((item = wires.FirstOrDefault()) != null)
		{
			Destroy(item);
			wires.Remove(item);
		}
		CalculateButtonValues();
	}
	public void CalculateCircuit()
	{
		_batteryController.CalculateValues();
	}
	public void ClearValues()
	{
		foreach (GameObject obj in objects)
		{
			obj.GetComponentInChildren<BoardObject>().ResetValues();
		}
	}
	private void SelectPlaceable(PlacableObject obj)
	{
		_currentObject = obj;
		_currentObjtext.text = _currentObject.ToString();
	}
	private void SelectNextPlaceable()
	{
		do
		{
			_currentObject++;
			if (!Enum.IsDefined(typeof(PlacableObject), _currentObject))
			{
				_currentObject = 0;
			}
		}
		while (CalculateRemainingObjects(_currentObject) <= 0);
		_currentObjtext.text = _currentObject.ToString();
	}
	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			SelectNextPlaceable();
		}
		if (Input.GetMouseButtonDown(0))
		{
			if (_currentObject != PlacableObject.Bewegen)
			{
				Vector3 position = Camera.allCameras[0].ScreenToWorldPoint(Input.mousePosition);
				position.z = 0;
				// Create a ray from the camera through the mouse position in 2D space
				Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Vector2 rayDirection = Vector2.zero; // In 2D, usually a direction isn't needed if you're just checking a point

				RaycastHit2D hit = Physics2D.Raycast(mousePosition, rayDirection, 100f, mask);
				if (hit.collider != null && hit.collider.gameObject == _board)
				{
					switch (_currentObject)
					{
						case PlacableObject.Batterie: PlaceBattery(position); break;
						case PlacableObject.Resistor: PlaceResistor(position); break;
						case PlacableObject.Lampe: PlaceLamp(position); break;
						case PlacableObject.Schalter: PlaceSwitch(position); break;
						case 0: throw new ArgumentOutOfRangeException($"Cannot Place Object{_currentObject}");
					};
				}
				CalculateButtonValues();
				if (CalculateRemainingObjects(_currentObject) <= 0)
				{
					SelectNextPlaceable();
				}
			}
			else
			{
				// Create a ray from the camera through the mouse position in 2D space
				Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Vector2 rayDirection = Vector2.zero; // In 2D, usually a direction isn't needed if you're just checking a point

				RaycastHit2D hit = Physics2D.Raycast(mousePosition, rayDirection, 100f, mask);
				if (hit.collider != null)
				{
					GameObject hitObject = hit.collider.gameObject;
					hit = Physics2D.Raycast(mousePosition, rayDirection, 100f, uiMask);
					GameObject uiObject = null;
					if (hit.collider != null)
					{
						uiObject = hit.collider.gameObject;
					}
					
					if (movingObj != null && hitObject == _board && uiObject?.transform.parent?.name != "Quest")
					{
						SetLayerAllChildren(movingObj.transform, LayerMask.NameToLayer("Default"));
						movingObj = null;
					}
					else if (movingObj == null && (hitObject.GetComponentInChildren<BoardObject>() != null || hitObject.GetComponentsInParent<BoardObject>() != null ) && hitObject != _board)
					{
							movingObj = hitObject;
							while (movingObj.transform.parent != null)
							{
								movingObj = hitObject.transform.parent.gameObject;
							}
							movingObjOriginalPos = hitObject.transform.position;
						
						
						SetLayerAllChildren(movingObj.transform, LayerMask.NameToLayer("Ignore Raycast"));
					}
					else if((hitObject != _board || uiObject?.transform.parent?.name == "Quest") && movingObj != null)
					{
						movingObj.transform.position = movingObjOriginalPos;
						SetLayerAllChildren(movingObj.transform, LayerMask.NameToLayer("Default"));
						movingObj = null;
					}
				}


			}




		}
		if (Input.GetMouseButtonDown(1))
		{
			// Create a ray from the camera through the mouse position in 2D space
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 rayDirection = Vector2.zero; // In 2D, usually a direction isn't needed if you're just checking a point

			RaycastHit2D hit = Physics2D.Raycast(mousePosition, rayDirection, 100f, mask);

			if (hit.collider != null && hit.collider.GetComponent<WireAcceptor>() != null)
			{
				if (CurrentObj != null)
				{
					GameObject hitObject = hit.collider.gameObject;
					//Cannot connect two ends of the same charge and the same object
					if (hitObject != CurrentObj && hitObject.transform.parent != CurrentObj.transform.parent && hitObject.GetComponent<WireAcceptor>().IsPositive() != CurrentObj.GetComponent<WireAcceptor>().IsPositive())
					{
						GameObject obj = Instantiate(_wire);
						Wire wire = obj.GetComponent<Wire>();
						wire.Obj1 = CurrentObj;
						wire.Obj2 = hitObject;
						hitObject.GetComponent<WireAcceptor>().AddWire(wire);
						CurrentObj.GetComponent<WireAcceptor>().AddWire(wire);
						CurrentObj = null;
						wires.Add(obj);
					}

				}
				else
				{
					CurrentObj = hit.collider.gameObject;
				}



			}

		}
		if (_currentObject == PlacableObject.Bewegen && movingObj != null)
		{
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 rayDirection = Vector2.zero; // In 2D, usually a direction isn't needed if you're just checking a point

			RaycastHit2D hit = Physics2D.Raycast(mousePosition, rayDirection, 100f, mask);
			if (hit.collider != null) {
				GameObject hitObject = hit.collider.gameObject;
				if(hitObject != _board && hitObject.GetComponentInChildren<BoardObject>() == null && hitObject.GetComponentsInParent<BoardObject>() == null)
				{
					movingObj.transform.position = movingObjOriginalPos;
					SetLayerAllChildren(movingObj.transform, LayerMask.NameToLayer("Default"));
					movingObj = null;
				}
				else
				{
					if (Input.GetKeyDown("r"))
					{
						movingObj.transform.Rotate(new(0, 0, -90));
						if (movingObj.transform.rotation.z == 360 || movingObj.transform.rotation.z == -360)
						{
							movingObj.transform.eulerAngles = new(0, 0, 0);
						}

					}
					movingObj.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					movingObj.transform.position = new(movingObj.transform.position.x, movingObj.transform.position.y, 0);
				}
			}
			else
			{
				movingObj.transform.position = movingObjOriginalPos;
				SetLayerAllChildren(movingObj.transform, LayerMask.NameToLayer("Default"));
				movingObj = null;
			}
			
		}
	}
	private int CalculateRemainingObjects(PlacableObject obj)
	{
		return _unlockedObjects[obj] - PlacedObjects[obj].Count;
	}
	private void CalculateButtonValues()
	{
		string text = _batteryButton.GetComponentInChildren<TMP_Text>().text;
		_batteryButton.GetComponentInChildren<TMP_Text>().text = text.Remove(text.IndexOf(":")) + ": " + CalculateRemainingObjects(PlacableObject.Batterie);
		_batteryButton.SetActive(CalculateRemainingObjects(PlacableObject.Batterie) > 0);

		text = _resistorButton.GetComponentInChildren<TMP_Text>().text;
		_resistorButton.GetComponentInChildren<TMP_Text>().text = text.Remove(text.IndexOf(":")) + ": " + CalculateRemainingObjects(PlacableObject.Resistor);
		_resistorButton.SetActive(CalculateRemainingObjects(PlacableObject.Resistor) > 0);

		text = _lampButton.GetComponentInChildren<TMP_Text>().text;
		_lampButton.GetComponentInChildren<TMP_Text>().text = text.Remove(text.IndexOf(":")) + ": " + CalculateRemainingObjects(PlacableObject.Lampe);
		_lampButton.SetActive(CalculateRemainingObjects(PlacableObject.Lampe) > 0);

		text = _switchButton.GetComponentInChildren<TMP_Text>().text;
		_switchButton.GetComponentInChildren<TMP_Text>().text = text.Remove(text.IndexOf(":")) + ": " + CalculateRemainingObjects(PlacableObject.Schalter);
		_switchButton.SetActive(CalculateRemainingObjects(PlacableObject.Schalter) > 0);

	}
	private void PlaceBattery(Vector3 position)
	{
		if (_batteryController == null
							&& position.x - _battery.transform.localScale.x / 2 > _board.transform.position.x - _board.transform.localScale.x / 2
							&& position.x + _battery.transform.localScale.x / 2 < _board.transform.position.x + _board.transform.localScale.x / 2
							&& position.y - _battery.transform.localScale.y / 2 > _board.transform.position.y - _board.transform.localScale.y / 2
							&& position.y + _battery.transform.localScale.y / 2 < _board.transform.position.y + _board.transform.localScale.y / 2)
		{
			GameObject obj = Instantiate(_battery, position, Quaternion.identity);
			obj.GetComponent<BatteryController>().Controller = this;
			objects.Add(obj);
			_batteryController = obj.GetComponent<BatteryController>();
			_calculateButton.interactable = true;
			PlacedObjects[_currentObject].Add(obj);

		}
	}
	private void PlaceResistor(Vector3 position)
	{
		if (position.x - _resistor.transform.localScale.x > _board.transform.position.x - _board.transform.localScale.x / 2
			&& position.x + _resistor.transform.localScale.x < _board.transform.position.x + _board.transform.localScale.x / 2
			&& position.y - _resistor.transform.localScale.y / 2 > _board.transform.position.y - _board.transform.localScale.y / 2
			&& position.y + _resistor.transform.localScale.y / 2 < _board.transform.position.y + _board.transform.localScale.y / 2)
		{
			GameObject obj = Instantiate(_resistor, position, Quaternion.identity);
			obj.GetComponent<BoardObject>().Resistance = _resistance;
			obj.name = objects.Count.ToString();
			objects.Add(obj);
			PlacedObjects[_currentObject].Add(obj);

		}
	}
	private void PlaceLamp(Vector3 position)
	{
		GameObject obj = Instantiate(_lamp, position, Quaternion.identity);
		obj.GetComponent<BoardObject>().Resistance = 2;
		obj.name = objects.Count.ToString();
		objects.Add(obj);
		PlacedObjects[_currentObject].Add(obj);
	}
	private void PlaceSwitch(Vector3 position)
	{
		GameObject obj = Instantiate(_switch, position, Quaternion.identity);
		obj.name = objects.Count.ToString();
		objects.Add(obj);
		PlacedObjects[_currentObject].Add(obj);
	}
	void SetLayerAllChildren(Transform root, int layer)
	{
		var children = root.GetComponentsInChildren<Transform>(includeInactive: true);
		foreach (var child in children)
		{
			child.gameObject.layer = layer;
		}
	}
}
