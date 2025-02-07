using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public struct BookChapter
    {
		public string name;
        public int start;
        public int end;
        public List<GameObject[]> pages;
        public GameObject bookmark;
	}
   
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private GameObject _bookmarkPrefab;
	[SerializeField] private GameObject _textPrefab;
	[SerializeField] private Transform _canvas;
    [SerializeField] private GameObject _nextPageButton,_previousPageButton;

	private readonly List<BookChapter> _chapters = new();
	private readonly List<GameObject[]> _allPages = new();

    private int currentPage = 0;
    
    public void ToStartOfChapter(BookChapter chapter)
    {
        ChangePage(chapter.start);
    }
    private BookChapter GenerateGearChapter(string path)
    {
		int startOfNextChapter;
		if(_chapters.Count == 0)
		{
			startOfNextChapter = 0;
		}
		else
		{
			startOfNextChapter = _chapters.Last().end + 1;
		}
		BookChapter chapter = new()
		{
			pages = new(),
			name = "Gears",
			start = startOfNextChapter
		};
		chapter.pages.Add(new GameObject[12]);
		currentPage = chapter.start;
		int index = 0;
		int yIndex = 0;
		GameObject button;

		List<Level> levels = Resources.LoadAll<Level>(path).ToList();
		levels.Sort(delegate (Level x1, Level x2) {
			int num1 = int.Parse(x1.name.Split("_")[0]);
			int num2 = int.Parse(x2.name.Split("_")[0]);
			return num1.CompareTo(num2);
		});
		foreach (Level level in levels)
		{
			if(yIndex > 5)
			{
				yIndex = 0;
			}
			if (index > 11)
			{
				currentPage++;
				index = 0;
				yIndex = 0;
				chapter.pages.Add(new GameObject[12]);
			}

			button = Instantiate(_buttonPrefab, new Vector3(0, 0, 0), Quaternion.identity, _canvas);
			if(index < 6)
			{
				button.transform.localPosition = new(-100, Mathf.Lerp(90, -120, yIndex / 6f), 0);
			}
			else
			{
				button.transform.localPosition = new(100, Mathf.Lerp(90, -120, yIndex / 6f), 0);
			}
			button.name = level.name;
			button.GetComponent<ButtonHandler>().sceneToLoad = "Gears";
			button.GetComponentInChildren<TMP_Text>().text = level.name.Split("_")[1];
			button.SetActive(false);

			chapter.pages[currentPage][index] = button;
			index++;
			yIndex++;
		}
		chapter.end = chapter.start + chapter.pages.Count - 1;
		currentPage = 0;
		return chapter;
	}
	private BookChapter GenerateElectricityChapter()
	{
		int startOfNextChapter;
		if (_chapters.Count == 0)
		{
			startOfNextChapter = 0;
		}
		else
		{
			startOfNextChapter = _chapters.Last().end + 1;
		}
		BookChapter chapter = new()
		{
			pages = new(),
			name = "Electricity",
			start = startOfNextChapter
		};
		currentPage = chapter.start;
		chapter.pages.Add(new GameObject[2]);

		GameObject button = Instantiate(_buttonPrefab, new Vector3(0, 0, 0), Quaternion.identity, _canvas);
		button.transform.localPosition = new(100, Mathf.Lerp(90, -120, 2.5f / 6f), 0);
		button.name = "Electricity";
		button.GetComponent<ButtonHandler>().sceneToLoad = "Electricity";
		button.GetComponent<ButtonHandler>().menuValue = 2f;
		button.GetComponentInChildren<TMP_Text>().text = "Spielen!";
		button.SetActive(false);

		chapter.pages[0][0] = button;

		GameObject textObj = Instantiate(_textPrefab, new Vector3(0, 0, 0), Quaternion.identity, _canvas);
		textObj.GetComponent<TMP_Text>().text = "Klicke auf den Knopf rechts, um in die Welt der Elekritzität abzutauchen!";
		textObj.SetActive(false);
		
		textObj.transform.localPosition = new Vector3(-100, 0, 0);
		chapter.pages[0][1] = textObj;



		chapter.end = chapter.start + chapter.pages.Count - 1;

		return chapter;
	}
	BookChapter GenerateExplenationChapter(string path,string name)
	{
		int startOfNextChapter;
		if (_chapters.Count == 0)
		{
			startOfNextChapter = 0;
		}
		else
		{
			startOfNextChapter = _chapters.Last().end + 1;
		}
		BookChapter chapter = new()
		{
			pages = new(),
			name = name,
			start = startOfNextChapter
		};
		TextAsset[] texts = Resources.LoadAll<TextAsset>(path + "/Texts");
		chapter.pages.Add(new GameObject[2]);
		int i = 0;
		currentPage = 0;
		foreach (TextAsset textAsset in texts)
		{
			GameObject textObj = Instantiate(_textPrefab, new Vector3(0, 0, 0), Quaternion.identity, _canvas);
			textObj.GetComponent<TMP_Text>().text = textAsset.text;
			textObj.SetActive(false);
			chapter.pages[currentPage][i] = textObj;
			if (i == 0)
			{
				textObj.transform.localPosition = new Vector3(-100, 0, 0);
				i++;
			}
			else
			{
				textObj.transform.localPosition = new Vector3(100, 0, 0);
				chapter.pages.Add(new GameObject[2]);
				currentPage++;
				i = 0;
			}
			

		}
		chapter.end = chapter.start + chapter.pages.Count - 1;

		currentPage = 0;
		return chapter;
	}
	void Start()
    {
		_chapters.Add(GenerateGearChapter("Levels/"));
		_chapters.Add(GenerateElectricityChapter());
		_chapters.Add(GenerateExplenationChapter("Pages/Gears","Gear-\nExplanations"));
		_chapters.Add(GenerateExplenationChapter("Pages/Electricity","Electricity-\nExplanations"));
		for (int i = 0;i<_chapters.Count;i++)
		{
			BookChapter chapter = _chapters[i];
			chapter.bookmark = CreateBookmarkObject(chapter);
		}
		currentPage = 0;
		//texts = Resources.LoadAll<TextAsset>("Pages/");
		//      _text.text = texts[currentPage].text;
		_chapters.ForEach(x => _allPages.AddRange(x.pages));

		_previousPageButton.SetActive(false);
        ToggleActivatedButtons(0, true);
    }

	private GameObject CreateBookmarkObject(BookChapter chapter)
	{
		int index = _chapters.IndexOf(chapter);
		float bookmarkWidth = _bookmarkPrefab.transform.localScale.x;
		float distanceBetweenBookmarks = 5f;
		GameObject bookmarkObj = Instantiate(_bookmarkPrefab, new Vector3(0,0,0), Quaternion.identity, _canvas);
		bookmarkObj.transform.localPosition = new Vector3(40 + index * (bookmarkWidth + distanceBetweenBookmarks), 135, 0);
		Bookmark bookmark = bookmarkObj.GetComponent<Bookmark>();
		bookmark.ToPage = (x) => { x.ChangePage(chapter.start); };
		bookmarkObj.GetComponentInChildren<TMP_Text>().text = chapter.name;
		bookmark.Controller = this;
		return bookmarkObj;
	}

	public void NextPage()
    {
        ChangePage(currentPage + 1);
    }
	public void PrevoiusPage()
	{
		ChangePage(currentPage - 1);
	}
	public void ChangePage(int page)
    {
		ToggleActivatedButtons(currentPage, false);
		ToggleActivatedButtons(page, true);
		currentPage = page;
		//if (currentPage < texts.Count())
		//{
		//	_text.text = texts[currentPage].text;
		//}
		if (currentPage == 0)
		{
			_previousPageButton.SetActive(false);
		}
		else
        {
			_previousPageButton.SetActive(true);
		}
		if (currentPage == _allPages.Count - 1)
		{
			_nextPageButton.SetActive(false);
        }
        else
        {
			_nextPageButton.SetActive(true);
		}
	}
    private void ToggleActivatedButtons(int page,bool active)
    {
        if(page < _allPages.Count)
        {
			for (int i = 0; i < _allPages[page].Length; i++)
			{
				_allPages[page][i]?.SetActive(active);
			}
		}
	}
}
