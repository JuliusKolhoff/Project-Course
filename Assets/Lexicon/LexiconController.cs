using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LexiconController : MonoBehaviour
{
	private List<string[]> pages = new();
	[SerializeField] private GameObject _nextPageButton, _previousPageButton;
	[SerializeField] private TMP_Text _textLeft,_textRight;
	private int currentPage = 0;
	// Start is called before the first frame update
	void Start()
	{
		int index = 0;

		pages.Add(new string[2]);
		
		currentPage = 0;
		TextAsset[] texts = Resources.LoadAll<TextAsset>("LexiconEntries/");
		foreach (TextAsset textAsset in texts)
		{
			pages[currentPage][index] = textAsset.text;
			index++;
			if(index > 1)
			{
				currentPage++;
				index = 0;
				pages.Add(new string[2]);
			}
		}
		currentPage = 0;
		_textLeft.text = pages[currentPage][0];
		_textRight.text = pages[currentPage][1];
		_previousPageButton.SetActive(false);
		if(pages.Count == 1)
		{
			_nextPageButton.SetActive(false);
		}
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
		if (currentPage == 0)
		{
			_previousPageButton.SetActive(false);
		}
	}
	public void ChangePage(int page)
	{
	
		currentPage = page;
		_textLeft.text = pages[currentPage][0];
		_textRight.text = pages[currentPage][1];
	}
	
}
