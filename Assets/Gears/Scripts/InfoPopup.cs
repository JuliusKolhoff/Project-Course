using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[RequireComponent(typeof(TMP_Text))]
public class InfoPopup : MonoBehaviour
{
	private TMP_Text textField;
	public GridManager Manager { get; set; }
	public bool disabledButton;
	public void Awake()
	{
		textField = GetComponent<TMP_Text>();
	}
	public void SetText(string text)
	{
		textField.text = text;
	}
	public void Destroy()
	{
		if (!Manager.HasEnabledPopup())
		{
			Manager.SetTilesVisible(true);
		}
		if(disabledButton)
		{
			Manager.SetPopupButtonEnabled(true);
		}
        Destroy(gameObject);
	}
}
