using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InhaltsverzeichnisButtonHandler : MonoBehaviour
{
    public int Page { get; set; }
    public MenuController Controller { get; set; }
    public void GoToPage()
    {
        Controller.ChangePage(Page);
    }
}
