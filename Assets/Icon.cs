using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundIcon : MonoBehaviour
{
    public void Init(Sprite sprite,Color color)
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
        GetComponent<SpriteRenderer>().color = color;
    }
}
