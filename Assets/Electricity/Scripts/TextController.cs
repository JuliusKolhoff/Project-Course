using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextController : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    // Start is called before the first frame update
    void Start()
    {
        BoardObject obj = GetComponent<BoardObject>();
        text.text = obj.Resistance + "Ω";
    }

}
