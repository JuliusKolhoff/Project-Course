using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class BatteryController : MonoBehaviour
{
	[SerializeField] private GameObject _plus, _minus;
	[SerializeField] private float _volt = 10;
	public BoardController Controller { get; set;}
	private void Update()
	{
		//if (Input.GetButtonDown("R")){
		//	List<GameObject> path = FindBattery(new List<GameObject>{ _plus });
		//	path.ForEach(x => Debug.Log(x.name));
		//}
		CalculateValues();
		if (Input.GetKeyDown(KeyCode.R))
		{
			CalculateValues();
		}
	}
	public void CalculateValues()
	{
		ClearValues();
		(float, float) Rges = FindRGesamt(gameObject, null);
		//Debug.Log($"Rgesamt: {Rges.Item1 + Rges.Item2}");
		BoardObject obj = GetComponent<BoardObject>();
		obj.Vin = _volt;
		obj.Ampere = _volt / Sum(Rges);
		CalculateRIU(gameObject);
		if (float.IsNaN(Rges.Item1) || float.IsNaN(Rges.Item2))
		{
			Controller.RgesText.text = "Kein Geschlossener Stromkreislauf";
		}else if(Sum(Rges) == 0)
		{
			Controller.RgesText.text = "Kurzschluss";
		}
		else
		{
			Controller.RgesText.text = "RGes: " + Sum(Rges);
		}
		Controller.objects.ForEach(x =>
		{
			x.GetComponentInChildren<BoardObject>().ResetVisits();
			if (x.GetComponent<SwitchController>() != null)
			{
				x.GetComponent<SwitchController>().ResetVisits();
			}
		});

	}
	//returns if follwing Circuit is connected to battery 
	private bool CalculateRIU(GameObject obj)
	{
		BoardObject boardObject = obj.GetComponentInParent<BoardObject>() ?? obj.GetComponent<BoardObject>();
		if (float.IsNaN(boardObject.Rges))
		{
			return false;
		}
		List<Wire> minusConnections = boardObject.MinusConnections;
		GameObject minusConnector = boardObject.Minus;
		if(minusConnections.Count == 1)
		{
			GameObject otherObject = minusConnections[0].GetOtherObject(minusConnector);
			if(otherObject != _plus)
			{
				BoardObject otherBoardObject = otherObject.GetComponentInParent<BoardObject>() ?? otherObject.GetComponent<BoardObject>();
				if (float.IsNaN(otherBoardObject.Rges))
				{
					otherBoardObject.Vin = 0;
					otherBoardObject.Vuse = 0;
					otherBoardObject.Ampere = 0;
					return false;
				}
				otherBoardObject.Ampere = boardObject.Ampere;
				float ResistanceSum = otherBoardObject.Rges;
				otherBoardObject.Vin = ResistanceSum * otherBoardObject.Ampere;
				otherBoardObject.Vuse = otherBoardObject.Vin * (otherBoardObject.Resistance / ResistanceSum);
				if (CalculateRIU(otherObject))
				{
					return true;
				}
				else
				{
					otherBoardObject.Vin = 0;
					otherBoardObject.Vuse = 0;
					otherBoardObject.Ampere = 0;
					return false;
				}
			}
			else
			{
				return true;
			}
		}
		else
		{
			List<GameObject> otherObjs = new();
			foreach (Wire connection in minusConnections)
			{
				GameObject otherObject = connection.GetOtherObject(minusConnector);
				if (otherObject != _plus && !float.IsNaN(otherObject.GetComponentInParent<BoardObject>().Rges))
				{
					otherObjs.Add(otherObject);
				}
				//TODO dont return false when reaching Battery
			}
			//otherObjs.ForEach(x => Debug.Log(x.name));
			otherObjs.Sort(delegate (GameObject x1, GameObject x2)
			{
				float rGes1 = x1.GetComponentInParent<BoardObject>().Rges;
				float rGes2 = x2.GetComponentInParent<BoardObject>().Rges;
				return rGes1.CompareTo(rGes2);
			});
			if (otherObjs.Count != 0 && otherObjs.First().GetComponentInParent<BoardObject>().Rges == 0)
			{
				for (int i = 1; i < otherObjs.Count; i++)
				{
					otherObjs[i].GetComponentInParent<BoardObject>().Vin = 0;
					otherObjs[i].GetComponentInParent<BoardObject>().Vuse = 0;
					otherObjs[i].GetComponentInParent<BoardObject>().Ampere = 0;
				}
				return true;
			}
			else
			{
				foreach (GameObject otherObject in otherObjs)
				{
					GameObject nextObj = otherObject;
					BoardObject otherBoardObject = otherObject.GetComponentInParent<BoardObject>();
					//if(otherBoardObject.Resistance == 0) 
					//{
					//	while (GetNextSingleObject(otherBoardObject) == null || GetNextSingleObject(otherBoardObject).Resistance == 0)
					//	{
					//		otherBoardObject = GetNextSingleObject(otherBoardObject);
					//	}
					//}
					float ResistanceSum = otherBoardObject.Rges;
					otherBoardObject.Vin = boardObject.Vin - boardObject.Vuse;
					otherBoardObject.Vuse = otherBoardObject.Vin * (otherBoardObject.Resistance / ResistanceSum);
					otherBoardObject.Ampere = otherBoardObject.Vuse / ResistanceSum;
					if (!CalculateRIU(otherObject))
					{
						otherBoardObject.Vin = 0;
						otherBoardObject.Vuse = 0;
						otherBoardObject.Ampere = 0;
						return false;
					}

				}
				return true;
			}
		}
	}
	private BoardObject GetNextSingleObject(BoardObject obj)
	{
		if(obj.MinusConnections.Count == 0 || obj.MinusConnections.Count > 1)
		{
			return null;
		}
		return obj.MinusConnections[0].GetComponent<BoardObject>() ?? obj.MinusConnections[0].GetComponentInParent<BoardObject>();
	}
	private float Sum((float,float) value)
	{
		return value.Item1 + value.Item2;
	}
	private void ClearValues()
	{
		Controller.ClearValues();
	}
	private void LogResistance(GameObject obj,(float,float) resistance)
	{
		/*
		if (obj.GetComponent<BatteryController>() == null)
			Debug.Log($"{obj.transform.parent.name}: Resistance: {resistance.Item1} and OverflowResistance: {resistance.Item2}");
		else
			Debug.Log($"{obj.name}: Resistance: {resistance.Item1} and OverflowResistance: {resistance.Item2}");
		*/
		
	}
	public (float,float) FindRGesamt(GameObject obj, Wire origin)
	{
		BoardObject boardObject = obj.GetComponentInParent<BoardObject>() ?? GetComponent<BoardObject>();
		List<Wire> minusConnections = boardObject.MinusConnections;
		List<Wire> plusConnections = boardObject.PlusConnections;
		int plusConnectionCount = plusConnections.Count;
		GameObject minusConnector = boardObject.Minus;
		if (minusConnections.Count == 0)
		{
			string name = obj.transform.parent?.name ?? obj.transform.name;
			Debug.Log($"Nicht im Kreislauf: {name}");
			return ReturnResisitances(obj, (float.NaN, 0), plusConnectionCount);
		}
		else if (minusConnections.Count == 1)
		{
			GameObject otherObject = minusConnections[0].GetOtherObject(minusConnector);
			if (otherObject == _plus)
			{
				return ReturnResisitances(obj, (boardObject.Resistance,0),plusConnectionCount);
			}
			else
			{
				(float, float) Rgesamt = FindRGesamt(otherObject, minusConnections[0]);
				if (float.IsNaN(Rgesamt.Item1))
				{
					return ReturnResisitances(obj, (float.NaN, 0), plusConnectionCount);
				}
				else
				{
					BoardObject otherBoardObject = otherObject.GetComponent<BoardObject>() ?? otherObject.GetComponentInParent<BoardObject>();
					if (otherBoardObject.IsMarked())
					{
						return ReturnResisitances(obj, (float.NaN, 0), plusConnectionCount);
					}
					Rgesamt.Item1 += boardObject.Resistance;
					if (!otherBoardObject.IsFirstVisit())
					{
						Rgesamt.Item2 = 0;
					}
					return ReturnResisitances(obj, Rgesamt, plusConnectionCount);
				}
				
			}
		}
		else
		{
			(float,float)[] resitances = new (float, float)[minusConnections.Count];
			int i = 0;
			foreach (Wire connection in minusConnections)
			{
				GameObject otherObject = connection.GetOtherObject(minusConnector);
				BoardObject bObj = otherObject.GetComponent<BoardObject>() ?? otherObject.GetComponentInParent<BoardObject>();
				if (otherObject == _plus)
				{
					return ReturnResisitances(obj, (boardObject.Resistance, 0), plusConnectionCount);
				}
				if (bObj.IsMarked())
				{
					return ReturnResisitances(obj, (float.NaN, 0), plusConnectionCount);
				}
				bObj.SetMark(true);
			}
			foreach (Wire connection in minusConnections)
            {
				GameObject otherObject = connection.GetOtherObject(minusConnector);
				BoardObject bObj = otherObject.GetComponent<BoardObject>() ?? otherObject.GetComponentInParent<BoardObject>();
				
				(float,float) resitance = FindRGesamt(otherObject, connection);
				resitances[i] = resitance;
				i++;
            }
			foreach (Wire connection in minusConnections)
			{
				GameObject otherObject = connection.GetOtherObject(minusConnector);
				BoardObject bObj = otherObject.GetComponent<BoardObject>() ?? otherObject.GetComponentInParent<BoardObject>();
				bObj.SetMark(false);
			}
			float inverseSum = 0;
			float sum = 0;
			bool oneNotNaN = false;
			bool oneIsZero = false;
            foreach ((float,float) resistance in resitances)
            {
				if (!float.IsNaN(resistance.Item1) && !oneIsZero)
				{
					oneNotNaN = true;
					if(resistance.Item1 == 0)
					{
						oneIsZero = true;
					}
					inverseSum += 1f / resistance.Item1;
					
				}
				sum += resistance.Item2;
			}
			if (oneIsZero)
			{
				return ReturnResisitances(obj, (boardObject.Resistance + sum, 0), plusConnectionCount);
			}
			if (oneNotNaN)
			{
				float finalResistance = 1f / inverseSum + boardObject.Resistance;

				return ReturnResisitances(obj, (finalResistance + sum, 0), plusConnectionCount);
			}
			else
			{
				return ReturnResisitances(obj, (float.NaN, 0), plusConnectionCount);
			}
			
        }
	}
	private (float,float) ReturnResisitances(GameObject obj,(float,float) values,int plusConnectionCount)
	{
		if(obj.GetComponentInParent<BoardObject>() != null)
		{
			//Debug.Log(obj.GetComponentInParent<BoardObject>().gameObject.transform.name + ";" + values.Item1);
			obj.GetComponentInParent<BoardObject>().Rges = values.Item1;
		}
		if (float.IsNaN(values.Item1) || values.Item1 == 0)
		{
			LogResistance(obj, values);
			return values;
		}
		if(plusConnectionCount < 2)
		{
			LogResistance(obj, values);
			return values;
		}
		else
		{
			LogResistance(obj, (0,values.Item1));
			return (0, values.Item1);
		}
	}
}


