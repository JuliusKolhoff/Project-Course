using System.Collections.Generic;
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
			Debug.Log($"Rgesamt: {Rges.Item1}");
		}
	}
	public (float,float) FindRGesamt(GameObject obj, Wire origin)
	{
		BoardObject boardObject = obj.GetComponentInParent<BoardObject>();
		List<Wire> plusConnections = boardObject.plusConnections;
		List<Wire> minusConnections = boardObject.minusConnections;
		if(origin != null)
		{
			minusConnections.Remove(origin);
		}
		if (plusConnections.Count == 0)
		{
			Debug.Log($"Nicht im Kreislauf: {obj.transform.parent.name}");
			return (-1,-1);
		}
		else if (plusConnections.Count == 1)
		{
			GameObject otherObject = plusConnections[0].GetOtherObject(obj);
			if (otherObject == _minus)
			{
				Debug.Log($"{obj.transform.parent.name}: Resistance: {boardObject.Resistance} and OverflowResistance: {0}");
				return (boardObject.Resistance,0);
			}
			else
			{
				(float, float) Rgesamt = FindRGesamt(otherObject, plusConnections[0]);
				Rgesamt.Item1 += boardObject.Resistance;
				Debug.Log($"{obj.transform.parent.name}: Resistance: {Rgesamt.Item1} and OverflowResistance: {Rgesamt.Item2}");
				return Rgesamt;
			}
		}
		else
		{
			(float,float)[] resitances = new (float, float)[plusConnections.Count];
			int i = 0;
            foreach (Wire connection in plusConnections)
            {
				resitances[i] = FindRGesamt(connection.GetOtherObject(obj), connection);
				i++;
            }
			float inverseSum = 0;
			float sum = 0;
            foreach ((float,float) resistance in resitances)
            {
				inverseSum += 1f / resistance.Item1;
				sum += resistance.Item2;
            }
			float resitance = 1f / inverseSum;
			Debug.Log($"{obj.transform.parent.name}: Resistance: {resitance} and OverflowResistance: {sum}");
			return (resitance,sum);
        }
	}
}


