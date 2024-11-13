using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class BatteryController : MonoBehaviour
{
	[SerializeField] private GameObject _plus, _minus;
	public BoardController Controller { get; set;}
	private void Update()
	{
		//if (Input.GetButtonDown("R")){
		//	List<GameObject> path = FindBattery(new List<GameObject>{ _plus });
		//	path.ForEach(x => Debug.Log(x.name));
		//}
		if (Input.GetKeyDown(KeyCode.R))
		{
			(float, float) Rges = FindRGesamt(gameObject, null);
			Debug.Log($"Rgesamt: {Rges.Item1 + Rges.Item2}");
		}
	}
	private void LogResistance(GameObject obj,(float,float) resistance)
	{
		if (obj.GetComponent<BatteryController>() == null)
			Debug.Log($"{obj.transform.parent.name}: Resistance: {resistance.Item1} and OverflowResistance: {resistance.Item2}");
		else
			Debug.Log($"{obj.name}: Resistance: {resistance.Item1} and OverflowResistance: {resistance.Item2}");
	}
	public (float,float) FindRGesamt(GameObject obj, Wire origin)
	{
		BoardObject boardObject = obj.GetComponentInParent<BoardObject>();
		List<Wire> minusConnections = boardObject.minusConnections;
		List<Wire> plusConnections = boardObject.plusConnections;
		int plusConnectionCount = plusConnections.Count;
		GameObject minusConnector = boardObject.Minus;
		if (minusConnections.Count == 0)
		{
			Debug.Log($"Nicht im Kreislauf: {obj.transform.parent.name}");
			return (-1,-1);
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
				Rgesamt.Item1 += boardObject.Resistance;
				if (!otherObject.transform.parent.GetComponent<BoardObject>().IsLastVisit())
				{
					Rgesamt.Item2 = 0;
				}
				return ReturnResisitances(obj, Rgesamt,plusConnectionCount);
			}
		}
		else
		{
			(float,float)[] resitances = new (float, float)[minusConnections.Count];
			int i = 0;
			
            foreach (Wire connection in minusConnections)
            {
				GameObject otherObject = connection.GetOtherObject(minusConnector);
				Debug.Log("i: " + i);
				Debug.Log("Other Obj: " + otherObject.transform.parent.name);
				(float,float) resitance = FindRGesamt(otherObject, connection);
				resitances[i] = resitance;
				i++;
            }
			float inverseSum = 0;
			float sum = 0;
            foreach ((float,float) resistance in resitances)
            {
				inverseSum += 1f / resistance.Item1;
				sum += resistance.Item2;
            }
			float finalResistance = 1f / inverseSum + boardObject.Resistance;
			
			return ReturnResisitances(obj,(finalResistance + sum,0),plusConnectionCount);
        }
	}
	private (float,float) ReturnResisitances(GameObject obj,(float,float) values,int plusConnectionCount)
	{
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


