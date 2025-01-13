using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour
{
    public GridManager Controller { get; set; }
    
	private void OnEnable()
	{
		Controller?.SetTilesVisible(false);
	}
	private void OnDisable()
	{
		Controller?.SetTilesVisible(true);
	}
}
