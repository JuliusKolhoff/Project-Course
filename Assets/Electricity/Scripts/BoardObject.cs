using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoardObject : MonoBehaviour
{
	public List<Wire> PlusConnections = new();
	public List<Wire> MinusConnections = new();
	private float _resistance;
	public float Vin,Vuse;
	public float Ampere;
	public float Rges;
	public float Resistance 
	{
		get => _resistance;
		set {
			_resistance = value;
			if(GetComponentInChildren<TMP_Text>() != null)
			{
				GetComponentInChildren<TMP_Text>().text = value + "Ω";
			}
		}
	}
	public GameObject Plus, Minus;
	private int visits = 0;
	public void ResetVisits()
	{
		visits = 0;
	}
	public bool IsFirstVisit()
	{
		if(visits == 0)
		{
			visits++;
			return true;
		}
		return false;
	}
	public void ResetValues()
	{
		Vin = 0;
		Vuse = 0;
		Ampere = 0;
	}
}
