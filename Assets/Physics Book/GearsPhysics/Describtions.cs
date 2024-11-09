using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Describtions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DescribtionText[] texts = Resources.LoadAll<DescribtionText>("Texts/");
        foreach (DescribtionText text in texts)
        {
            Debug.Log(text.text);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
