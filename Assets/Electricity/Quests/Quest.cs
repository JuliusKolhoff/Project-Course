using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Quest : MonoBehaviour
{

	/*
        Quest 1: Lasse eine Lampe leuchten -> 2.Lampe
        Quest 2: Lasse zwei Lampen mit der Gleiche Ampere Anzahl leucheten -> Info
        Quest 3: Lasse zwei Lampen mit gleicher Volt Anzahl -> Resistor
        Quest 4: Lasse eine Lampe mit halber Volt Zahl Leuchten -> Schalter
        Quest 5: Lasse Lampe leuchten und nicht leuchten -> Info
        Quest 6: Lasse Lampe bei eingeschaltetem Schalter nicht leuchten bei ausgeschaltetem schon -> infinite alles
    */
	private string[] QuestDescription =
	{
		"Quest 1:  Lasse eine Lampe leuchten \n	-> Schaltet die 2.Lampe frei",
		"Quest 2:  Lasse zwei Lampen mit der Gleiche Ampere Anzahl leucheten \n	-> Schaltet neue Informationen frei",
		"Quest 3:  Lasse zwei Lampen mit gleicher Volt Anzahl\n	-> Schaltet einen Resistor frei",
		"Quest 4:  Lasse eine Lampe mit halber Volt Zahl Leuchten \n ->	Schaltet einen Schalter frei",
		"Quest 5:  Lasse Lampe leuchten und nicht leuchten \n -> Schaltet neue Informationen frei",
		"Quest 6:  Lasse Lampe bei eingeschaltetem Schalter nicht leuchten bei ausgeschaltetem schon \n	-> infinite alles"
	};
    public Func<Quest,bool>[] quests =
    {
        x => {return x.Q1(x.boardController); },
		x => {return x.Q2(x.boardController); },
		x => {return x.Q3(x.boardController); }
	};
    public BoardController boardController;
	private int currentQuest = 0;
	[SerializeField] private TMP_Text _questTextField;  
	public void Start()
	{
		_questTextField.text = QuestDescription[currentQuest];
	}
	public void Update()
	{
		bool completed = quests[currentQuest]?.Invoke(this) ?? false;
		if (completed)
		{
			Debug.Log(QuestDescription[currentQuest]);
			quests[currentQuest] = null;
			currentQuest++;
			_questTextField.text = QuestDescription[currentQuest];
		}
	}

	private bool Q1(BoardController boardController)
	{
		return CheckQuest<LampController>(
			boardController,
			x => { return x.IsGlowing(); },
			x => { return x.Count >= 1; },
			x => { x.Unlock(BoardController.PlacableObject.Lampe, 1); }
			);
	}
	//Quest 2: Lasse zwei Lampen mit der Gleiche Ampere Anzahl wie die Batterie leucheten -> Nichts
	private bool Q2(BoardController boardController)
	{
		return CheckQuest<LampController>(
			boardController,
			x => { return x.IsGlowing(); },
			x => { 
				if (x.Count == 2) 
				{
					return x[0].gameObject.GetComponent<BoardObject>().Ampere == boardController.GetBattery().gameObject.GetComponent<BoardObject>().Ampere
					&& x[1].gameObject.GetComponent<BoardObject>().Ampere == boardController.GetBattery().gameObject.GetComponent<BoardObject>().Ampere; 
				}
				return false; 
			},
			x => {}
			);
	}
	//Quest 3: Lasse zwei Lampen mit gleicher Volt Anzahl -> Resistor
	private bool Q3(BoardController boardController)
	{
		return CheckQuest<LampController>(
			boardController,
			x => { return x.IsGlowing(); },
			x => {
				if (x.Count == 2)
				{
					return x[0].gameObject.GetComponent<BoardObject>().Vuse == boardController.GetBattery().gameObject.GetComponent<BoardObject>().Vin
					&& x[1].gameObject.GetComponent<BoardObject>().Vuse == boardController.GetBattery().gameObject.GetComponent<BoardObject>().Vin;
				}
				return false;
			},
			x => { x.Unlock(BoardController.PlacableObject.Resistor, 1); }
			);
	}
	//Quest 1: Lasse eine Lampe leuchten -> 2 Lampen
	private bool Quest1(BoardController boardController)
    {
        foreach (var item in boardController.objects)
        {
            LampController lamp = item.GetComponent<LampController>();

			if (lamp != null && lamp.IsGlowing())
            {
                return true;
            }
        }
        boardController.Unlock(BoardController.PlacableObject.Lampe, 1);
        return false;
    }
	//Quest 2: Lasse zwei Lampen mit der Gleiche Ampere Anzahl leucheten -> 2 Lampen
	private bool Quest2(BoardController boardController)
	{
		List<BoardObject> lamps = new();
		foreach (var item in boardController.objects)
		{
			LampController lamp = item.GetComponent<LampController>();

			if (lamp != null && lamp.IsGlowing())
			{
				lamps.Add(item.GetComponent<BoardObject>());
			}
		}
		if (lamps.Count >= 2)
		{
			//Alle Lampem müssen die gleiche Volt Anzahl haben
			float amprere = lamps.First().Ampere;
			foreach (BoardObject obj in lamps)
			{
				if (obj.Ampere != amprere)
				{
					return false;
				}
			}
			boardController.Unlock(BoardController.PlacableObject.Lampe, 2);
			return true;
		}
		return false;
	}
	//Quest 3: Lasse zwei Lampen mit gleicher Volt Anzahl -> Resistor
	private bool Quest3(BoardController boardController)
	{
		List<BoardObject> lamps = new();
		foreach (var item in boardController.objects)
		{
			LampController lamp = item.GetComponent<LampController>();

			if (lamp != null && lamp.IsGlowing())
			{
				lamps.Add(item.GetComponent<BoardObject>());
			}
		}
		if (lamps.Count >= 2)
		{
			//Alle Lampem müssen die gleiche Volt Anzahl haben
			float Vuse = lamps.First().Vuse;
			foreach(BoardObject obj in lamps)
			{
				if(obj.Vuse != Vuse)
				{
					return false;
				}
			}
			boardController.Unlock(BoardController.PlacableObject.Lampe, 2);
			return true;
		}
		return false;
	}

	private bool CheckQuest<T>(BoardController boardController,Func<T,bool> filter,Func<List<T>,bool> condition,Action<BoardController> action)
	{
		List<T> items = new();
		foreach (var item in boardController.objects)
		{
			T obj = item.GetComponent<T>();

			if (obj != null && filter.Invoke(obj))
			{
				items.Add(obj);
			}
		}
		if (condition.Invoke(items))
		{
			action.Invoke(boardController);
			return true;
		}
		return false;
	}
	//private bool Q2(BoardController boardController)
	//{
	//	return CheckQuest<LampController>(
	//		boardController,
	//		x => { return x.IsGlowing(); },
	//		x => {
	//			if (x.Count < 2)
	//				return false;
	//			BoardObject obj = x.First().gameObject.GetComponent<BoardObject>();
	//			float ampere = obj.Ampere;
	//			foreach (var item in x)
	//			{
	//				BoardObject otherObj = item.gameObject.GetComponent<BoardObject>();
	//				if (otherObj.Ampere != ampere)
	//				{
	//					return false;
	//				}
	//			}
	//			return true;

	//		},
	//		x => { }
	//		);
	//}
}
