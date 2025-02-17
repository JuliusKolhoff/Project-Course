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
	private List<float> savedResistances = new();
	private int openedParallelschaltungIndex = 0;
	private int closedParallelschaltungIndex = 0;
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
		float Rges = FindRGesamt(gameObject);
		//Debug.Log($"Rgesamt: {Rges.Item1 + Rges.Item2}");
		BoardObject obj = GetComponent<BoardObject>();
		obj.Vin = _volt;
		obj.Ampere = _volt / Rges;
		CalculateRIU(gameObject);
		if (float.IsNaN(Rges))
		{
			Controller.RgesText.text = "Kein Geschlossener Stromkreislauf";
		}else if(Rges == 0)
		{
			Controller.RgesText.text = "Kurzschluss";
		}
		else
		{
			Controller.RgesText.text = "RGes: " + Rges;
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
		List<GameObject> otherObjs = FilterConnections(obj);
		
		if (otherObjs.Count == 1)
		{
			GameObject otherObject = otherObjs[0];
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
			//otherObjs.ForEach(x => Debug.Log(x.name));
			otherObjs.Sort(delegate (GameObject x1, GameObject x2)
			{
				float rGes1 = x1.GetComponentInParent<BoardObject>().Rges;
				float rGes2 = x2.GetComponentInParent<BoardObject>().Rges;
				return rGes1.CompareTo(rGes2);
			});
			foreach (GameObject otherObject in otherObjs)
			{ 
				if(otherObject == _plus)
				{
					return true;
				}
			}
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
	private List<GameObject> FilterConnections(GameObject obj)
	{
		List<GameObject> otherObjs = new();
		BoardObject boardObject = obj.GetComponentInParent<BoardObject>() ?? obj.GetComponent<BoardObject>();
		GameObject minusConnector = boardObject.Minus;
		List<Wire> minusConnections = boardObject.MinusConnections;
		foreach (Wire connection in minusConnections)
		{
			GameObject otherObject = connection.GetOtherObject(minusConnector);

			if (otherObject.GetComponentInParent<BoardObject>().Resistance == 0 && otherObject != _plus)
			{
				foreach (GameObject follower in FilterConnections(otherObject))
				{
					if (!otherObjs.Contains(follower))
						otherObjs.Add(follower);
				}
				
			}
			else if (!float.IsNaN(otherObject.GetComponentInParent<BoardObject>().Rges))
			{
				if(!otherObjs.Contains(otherObject))
					otherObjs.Add(otherObject);
			}
			
		}
		return otherObjs;
	}
	private void ClearValues()
	{
		Controller.ClearValues();
		savedResistances = new();
		openedParallelschaltungIndex = 0;
		closedParallelschaltungIndex = 0;
	}
	private void LogResistance(GameObject obj,float resistance)
	{
		//if (obj.GetComponent<BatteryController>() == null)
		//	Debug.Log($"{obj.transform.parent.name}: Resistance: {resistance}");
		//else
		//	Debug.Log($"{obj.name}: Resistance: {resistance}");
		
		
	}
	private List<GameObject> GetFollowingConnections(GameObject obj)
	{
		List<GameObject> otherObjs = new();
		BoardObject boardObject = obj.GetComponentInParent<BoardObject>() ?? obj.GetComponent<BoardObject>();
		GameObject minusConnector = boardObject.Minus;
		List<Wire> minusConnections = boardObject.MinusConnections;
		foreach (Wire connection in minusConnections)
		{
			GameObject otherObject = connection.GetOtherObject(minusConnector);
			if (otherObject.GetComponentInParent<BoardObject>().Resistance == 0 && otherObject != _plus)
			{
				foreach (GameObject follower in GetFollowingConnections(otherObject))
				{
					if (!otherObjs.Contains(follower))
						otherObjs.Add(follower);
				}
			}
			else
			{
				if (!otherObjs.Contains(otherObject))
					otherObjs.Add(otherObject);
			}

		}
		return otherObjs;
	}
	private List<GameObject> GetPreviousConnections(GameObject obj)
	{
		List<GameObject> otherObjs = new();
		BoardObject boardObject = obj.GetComponentInParent<BoardObject>() ?? obj.GetComponent<BoardObject>();
		GameObject plusConnector = boardObject.Plus;
		List<Wire> plusConnections = boardObject.PlusConnections;
		foreach (Wire connection in plusConnections)
		{
			GameObject otherObject = connection.GetOtherObject(plusConnector);
			if (otherObject.GetComponentInParent<BoardObject>().Resistance == 0 && otherObject != _minus)
			{
				foreach (GameObject follower in GetPreviousConnections(otherObject))
				{
					if (!otherObjs.Contains(follower))
						otherObjs.Add(follower);
				}
			}
			else if(ComesFromBatterie(otherObject) || otherObject == _minus)
			{
				if (!otherObjs.Contains(otherObject))
					otherObjs.Add(otherObject);
			}

		}
		return otherObjs;
	}
	private bool ComesFromBatterie(GameObject obj)
	{
		BoardObject boardObject = obj.GetComponentInParent<BoardObject>() ?? obj.GetComponent<BoardObject>();
		foreach (Wire connection in boardObject.PlusConnections)
		{
			GameObject otherObject = connection.GetOtherObject(boardObject.Plus);
			if( otherObject == _minus || ComesFromBatterie(otherObject))
			{
				return true;
			}
		}
		return false;
	}
	
	public float FindRGesamt(GameObject obj)
	{
		BoardObject boardObject = obj.GetComponentInParent<BoardObject>() ?? GetComponent<BoardObject>();
		List<GameObject> previousObjects = GetPreviousConnections(obj);
		int plusConnectionCount = previousObjects.Count;
		List<GameObject> followingObjs = GetFollowingConnections(obj);
		Debug.Log("Obj: " + obj.name +"Following Obj: ");
		followingObjs.ForEach(x => Debug.Log(x.name));
		if (followingObjs.Count == 0)
		{
			string name = obj.transform.parent?.name ?? obj.transform.name;
			Debug.Log($"Nicht im Kreislauf: {name}");
			return ReturnResisitances(obj, float.NaN, plusConnectionCount);
		}
		else if (followingObjs.Count == 1)
		{
			GameObject otherObject = followingObjs[0];
			if (otherObject == _plus)
			{
				return ReturnResisitances(obj, boardObject.Resistance,plusConnectionCount);
			}
			else
			{
				float Rgesamt = FindRGesamt(otherObject);
				if (float.IsNaN(Rgesamt))
				{
					return ReturnResisitances(obj, float.NaN, plusConnectionCount);
				}
				else
				{
					BoardObject otherBoardObject = otherObject.GetComponent<BoardObject>() ?? otherObject.GetComponentInParent<BoardObject>();
					if (otherBoardObject.IsMarked())
					{
						return ReturnResisitances(obj, float.NaN, plusConnectionCount);
					}
					Rgesamt += boardObject.Resistance;
					return ReturnResisitances(obj, Rgesamt, plusConnectionCount);
				}
				
			}
		}
		else
		{
			float[] resitances = new float[followingObjs.Count];
			openedParallelschaltungIndex++;
			int i = 0;
			foreach (GameObject otherObject in followingObjs)
			{
				BoardObject bObj = otherObject.GetComponent<BoardObject>() ?? otherObject.GetComponentInParent<BoardObject>();
				if (otherObject == _plus)
				{
					Debug.Log("Plus");
					return ReturnResisitances(obj, boardObject.Resistance, plusConnectionCount);
				}
				if (bObj.IsMarked())
				{
					Debug.Log("marked");
					return ReturnResisitances(obj, float.NaN, plusConnectionCount);
				}
				bObj.SetMark(true);
			}
			foreach (GameObject otherObject in followingObjs)
			{
				BoardObject bObj = otherObject.GetComponent<BoardObject>() ?? otherObject.GetComponentInParent<BoardObject>();
				
				float resitance = FindRGesamt(otherObject);
				resitances[i] = resitance;
				i++;
            }
			foreach (GameObject otherObject in followingObjs)
			{
				BoardObject bObj = otherObject.GetComponent<BoardObject>() ?? otherObject.GetComponentInParent<BoardObject>();
				bObj.SetMark(false);
			}
			float inverseSum = 0;
			bool oneNotNaN = false;
			bool oneIsZero = false;
            foreach (float resistance in resitances)
            {
				if (!float.IsNaN(resistance) && !oneIsZero)
				{
					oneNotNaN = true;
					if(resistance == 0)
					{
						oneIsZero = true;
					}
					inverseSum += 1f / resistance;
					
				}
			}
			openedParallelschaltungIndex--;
			float savedResistance = savedResistances.ElementAtOrDefault(openedParallelschaltungIndex);
			Debug.Log(openedParallelschaltungIndex + ": " + savedResistance);
			if (openedParallelschaltungIndex < savedResistances.Count)
			{
				savedResistances.RemoveAt(openedParallelschaltungIndex);
			}
			if (oneIsZero)
			{
				return ReturnResisitances(obj, boardObject.Resistance + savedResistance, plusConnectionCount);
			}
			if (oneNotNaN)
			{
				float finalResistance = 1f / inverseSum + boardObject.Resistance;
				return ReturnResisitances(obj, finalResistance + savedResistance, plusConnectionCount);
			}
			else
			{
				return ReturnResisitances(obj, float.NaN, plusConnectionCount);
			}
			
        }
	}
	private float ReturnResisitances(GameObject obj,float values,int plusConnectionCount)
	{
		if(obj.GetComponentInParent<BoardObject>() != null)
		{
			//Debug.Log(obj.GetComponentInParent<BoardObject>().gameObject.transform.name + ";" + values.Item1);
			obj.GetComponentInParent<BoardObject>().Rges = values;
		}
		if (float.IsNaN(values) || values == 0)
		{
			LogResistance(obj, values);
			return values;
		}
		if(plusConnectionCount < 2 || obj == this.gameObject)
		{
			LogResistance(obj, values);
			return values;
		}
		else
		{
			LogResistance(obj, values);
			
			if (obj.GetComponentInParent<BoardObject>().IsFirstVisit())
			{
				closedParallelschaltungIndex++;
				savedResistances.Add(values);
			}
			return 0;
		}
	}
}


