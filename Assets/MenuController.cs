using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    private List<GameObject[]> pages = new();
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private Transform _canvas;
    [SerializeField] private GameObject _nextPageButton,_previousPageButton;
    [SerializeField] private TMP_Text _text;
    private TextAsset[] texts;
    private int currentPage = 0;
    // Start is called before the first frame update
    void Start()
    {
        int index = 0;
		//string[] files = Directory.GetFiles("Assets/Gears/Resources/Levels", "*.asset", SearchOption.TopDirectoryOnly);

		pages.Add(new GameObject[6]);
		//      List<Level> levels = new List<Level>();
		//      foreach (var path in files)
		//      {
		//          string resourcePath = path.Split("/").Last().Replace(".asset","");
		//          Debug.Log(resourcePath);
		//          levels.Add(Resources.Load<Level>(resourcePath));
		//      }
		List<Level> levels = Resources.LoadAll<Level>("Levels/").ToList();
		levels.Sort(delegate(Level x1,Level x2) {
            int num1 = int.Parse(x1.name.Split("_")[0]);
			int num2 = int.Parse(x2.name.Split("_")[0]);
            return num1.CompareTo(num2); 
        
        
        });
        levels.ForEach(x => Debug.Log(x.name));
		foreach (Level level in levels)
		{
			if (index > 5)
            {
                currentPage++;
                index = 0;
                pages.Add(new GameObject[6]);
            }
            GameObject button = Instantiate(_buttonPrefab,new Vector3(0,0,0),Quaternion.identity, _canvas);
            button.transform.localPosition = new(100,Mathf.Lerp(90, -120, index / 6f),0);
			button.name = level.name;
            button.GetComponent<ButtonHandler>().sceneToLoad = "Gears";
            button.GetComponentInChildren<TMP_Text>().text = level.name;
            button.SetActive(false);
            pages[currentPage][index] = button;
            index++;
        }
        currentPage = 0;
		texts = Resources.LoadAll<TextAsset>("Pages/");
        _text.text = texts[currentPage].text;
		_previousPageButton.SetActive(false);
        ToggleActivatedButtons(0, true);
    }
    public void NextPage()
    {
        ChangePage(currentPage + 1);
        _previousPageButton.SetActive(true);
        if (currentPage == pages.Count - 1)
        {
            _nextPageButton.SetActive(false);
        }
    }
	public void PrevoiusPage()
	{
		ChangePage(currentPage - 1);
        _nextPageButton.SetActive(true);
        if(currentPage == 0)
        {
            _previousPageButton.SetActive(false);
        }
	}
	public void ChangePage(int page)
    {
        ToggleActivatedButtons(currentPage, false);
		ToggleActivatedButtons(page, true);
		currentPage = page;
		_text.text = texts[currentPage].text;
	}
    private void ToggleActivatedButtons(int page,bool active)
    {
        for (int i = 0; i < pages[page].Length; i++)
		{
            pages[page][i]?.SetActive(active);

		}
	}
}
