using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
	[SerializeField] private GameObject _leftConnector, _rightConnector;
	[SerializeField] private GameObject _wirePrefab;
    private Wire _wire;
	private bool _isOn;
    void Start()
    {
		_isOn = true;
		GameObject obj = Instantiate(_wirePrefab);
		_wire = obj.GetComponent<Wire>();
		_wire.name = "SwitchConnection";
		_wire.Obj1 = _leftConnector;
		_wire.Obj2 = _rightConnector;
		_leftConnector.GetComponent<WireAcceptor>().AddWire(_wire);
		_rightConnector.GetComponent<WireAcceptor>().AddWire(_wire);
	}
    public void ToggleSwitch()
    {
        if (!_isOn)
        {
            _wire.gameObject.SetActive(true);
			_leftConnector.GetComponent<WireAcceptor>().AddWire(_wire);
			_rightConnector.GetComponent<WireAcceptor>().AddWire(_wire);
        }
        else
        {
			_wire.gameObject.SetActive(false);
			_leftConnector.GetComponent<WireAcceptor>().RemoveWire(_wire);
			_rightConnector.GetComponent<WireAcceptor>().RemoveWire(_wire);
		}
		_isOn = !_isOn;
	}
	public void ResetVisits()
	{
		BoardObject[] children = GetComponentsInChildren<BoardObject>();
        foreach (var item in children)
        {
			item.ResetVisits();
        }
    }
	private void OnDestroy()
	{
		Destroy(_wire.gameObject);
	}
	// Update is called once per frame
	void Update()
    {
        
    }
	private void OnMouseOver()
	{
		if (Input.GetMouseButtonDown(1))
		{
			Debug.Log("Toggle");
			ToggleSwitch();
		}
	}
	
	
}
