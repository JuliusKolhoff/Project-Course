using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardController : MonoBehaviour
{
    private List<GameObject> objects;
	private List<GameObject> wires;
	[SerializeField] private GameObject _battery,_wire,_resistor,_lamp,_switch;
	[SerializeField] private GameObject _board;
	[SerializeField] private Button _calculateButton;
	public GameObject CurrentObj { get; set; }
	private CurrentObject _currentObject;
	private TMP_Text _currentObjtext;
	public TMP_Text RgesText;
	private float _resistance;
	private BatteryController _batteryController;
    void Start()
    {
        objects = new();
		wires = new();
		_currentObjtext = GetComponent<TMP_Text>();
		_currentObject = CurrentObject.Battery;
		_calculateButton.interactable = false;
	}
	private enum CurrentObject
	{
		None,
		Battery,
		Resistor,
		Switch,
		Lamp
	}
	public void ChangeResistance(string value)
	{
		if(value.Replace(" ","") != "")
			_resistance = int.Parse(value.Replace(" ", ""));
	}
	public void ResetBoard()
	{
		_batteryController = null;
		_calculateButton.interactable = false;
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
    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			_currentObject++;
			if (!Enum.IsDefined(typeof(CurrentObject), _currentObject))
			{
				_currentObject = 0;
			}
			
			_currentObjtext.text = _currentObject.ToString();
		}
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 position = Camera.allCameras[0].ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
			// Create a ray from the camera through the mouse position in 2D space
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 rayDirection = Vector2.zero; // In 2D, usually a direction isn't needed if you're just checking a point

			RaycastHit2D hit = Physics2D.Raycast(mousePosition, rayDirection, 100f);
			if(hit.collider != null && hit.collider.gameObject == _board)
			{
				GameObject obj;
				switch (_currentObject)
				{
					case CurrentObject.Battery:
						if(_batteryController == null 
							&& position.x - _battery.transform.localScale.x/2 > _board.transform.position.x - _board.transform.localScale.x/2 
							&& position.x + _battery.transform.localScale.x/2 < _board.transform.position.x + _board.transform.localScale.x /2
							&& position.y - _battery.transform.localScale.y / 2 > _board.transform.position.y - _board.transform.localScale.y / 2
							&& position.y + _battery.transform.localScale.y / 2 < _board.transform.position.y + _board.transform.localScale.y / 2)
						{
							obj = Instantiate(_battery, position, Quaternion.identity);
							obj.GetComponent<BatteryController>().Controller = this;
							objects.Add(obj);
							_batteryController = obj.GetComponent<BatteryController>();
							_calculateButton.interactable = true;
						}
						break;
					case CurrentObject.Resistor:
						if (position.x - _resistor.transform.localScale.x > _board.transform.position.x - _board.transform.localScale.x / 2
							&& position.x + _resistor.transform.localScale.x < _board.transform.position.x + _board.transform.localScale.x / 2
							&& position.y - _resistor.transform.localScale.y / 2 > _board.transform.position.y - _board.transform.localScale.y / 2
							&& position.y + _resistor.transform.localScale.y / 2 < _board.transform.position.y + _board.transform.localScale.y / 2)
						{
							obj = Instantiate(_resistor, position, Quaternion.identity);
							obj.GetComponent<BoardObject>().Resistance = _resistance;
							obj.name = objects.Count.ToString();
							objects.Add(obj);
						}
						break;
					case CurrentObject.Lamp:
						obj = Instantiate(_lamp, position, Quaternion.identity);
						obj.GetComponent<BoardObject>().Resistance = _resistance;
						obj.name = objects.Count.ToString();
						objects.Add(obj);
						break;
					case CurrentObject.Switch:
						obj = Instantiate(_switch, position, Quaternion.identity);
						obj.name = objects.Count.ToString();
						objects.Add(obj);
						break;
				}
			}
			
			
			
        }
		if (Input.GetMouseButtonDown(1))
		{
			// Create a ray from the camera through the mouse position in 2D space
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 rayDirection = Vector2.zero; // In 2D, usually a direction isn't needed if you're just checking a point

			RaycastHit2D hit = Physics2D.Raycast(mousePosition, rayDirection, 100f);

			if (hit.collider != null && hit.collider.GetComponent<WireAcceptor>() != null)
			{
				if (CurrentObj != null)
				{
					GameObject hitObject = hit.collider.gameObject;
					if (hitObject != CurrentObj)
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
	}

}
