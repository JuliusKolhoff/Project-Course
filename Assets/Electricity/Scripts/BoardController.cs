using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    private List<GameObject> objects;
    [SerializeField] private GameObject _battery,_wire,_resistor;
    public GameObject CurrentObj { get; set; }
	private CurrentObject _currentObject;
	private TMP_Text _text;
	private float _resistance;
    void Start()
    {
        objects = new();
		_text = GetComponent<TMP_Text>();
		_currentObject = CurrentObject.Battery;
    }
	private enum CurrentObject
	{
		None,
		Battery,
		Resistor
	}
	public void ChangeResistance(string value)
	{
		if(value != "")
			_resistance = int.Parse(value);
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
			
			_text.text = _currentObject.ToString();
		}
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 position = Camera.current.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
			GameObject obj;
			switch (_currentObject)
			{
				case CurrentObject.Battery:
					obj = Instantiate(_battery, position, Quaternion.identity);
					obj.GetComponent<BatteryController>().Controller = this;
					objects.Add(obj);
					break;
				case CurrentObject.Resistor:
					obj = Instantiate(_resistor, position, Quaternion.identity);
					_resistor.GetComponent<BoardObject>().Resistance = _resistance;
					obj.name = objects.Count.ToString();
					objects.Add(obj);
					break;
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
					GameObject obj = Instantiate(_wire);
					Wire wire = obj.GetComponent<Wire>();
					wire.Obj1 = CurrentObj;
					wire.Obj2 = hitObject;
					hitObject.GetComponent<WireAcceptor>().AddWire(wire);
					CurrentObj.GetComponent<WireAcceptor>().AddWire(wire);
					CurrentObj = null;
				}
				else
				{
					CurrentObj = hit.collider.gameObject;
				}



			}

		}
	}

}
