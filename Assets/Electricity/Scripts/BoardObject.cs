using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoardObject : MonoBehaviour
{
	public List<Wire> plusConnections = new();
	public List<Wire> minusConnections = new();
	private float _resistance;
	public float Vin,Vuse;
	public float Ampere;
	public float Resistance 
	{
		get => _resistance;
		set {
			_resistance = value;
			GetComponentInChildren<TMP_Text>().text = value + "Ω";
		}
	}
	public GameObject Plus, Minus;
	private int visits = 0;
	public bool IsLastVisit()
	{
		/*Debug.Log("Name: " + name);
		Debug.Log("Visits: " + visits);
		Debug.Log("Count: " + plusConnections.Count);*/
		visits++;
		if(visits == plusConnections.Count)
		{
			visits = 0;
			return true;
		}
		return false;
	}
}
