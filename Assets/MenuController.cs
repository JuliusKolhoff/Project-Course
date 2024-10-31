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
    private int currentPage = 0;
    // Start is called before the first frame update
    void Start()
    {
        int index = 0;
		string[] files = Directory.GetFiles("Assets/Gears/Levels", "*.asset", SearchOption.TopDirectoryOnly);
		pages.Add(new GameObject[6]);
        List<Level> levels = new List<Level>();
        foreach (var path in files)
        {
            levels.Add(AssetDatabase.LoadAssetAtPath<Level>(path));
        }
        levels.Sort(delegate(Level x1,Level x2) {
            int num1 = int.Parse(x1.name.Split(" ")[1]);
			int num2 = int.Parse(x2.name.Split(" ")[1]);
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
            button.transform.localPosition = new(Mathf.Lerp(-300,300,(index % 3) / 2f),Mathf.Lerp(100, -100, index / 3),0);
			button.name = level.name;
            button.GetComponent<ButtonHandler>().sceneToLoad = "Gears";
            button.GetComponentInChildren<TMP_Text>().text = level.name;
            button.SetActive(false);
            pages[currentPage][index] = button;
            index++;
        }
        currentPage = 0;
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
    }
    private void ToggleActivatedButtons(int page,bool active)
    {
        for (int i = 0; i < pages[page].Length; i++)
		{
            pages[page][i]?.SetActive(active);

		}
	}
    // Update is called once per frame
    void Update()
    {
        
    }
}
