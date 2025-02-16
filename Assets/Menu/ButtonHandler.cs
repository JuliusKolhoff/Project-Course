using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHandler : MonoBehaviour, IPointerClickHandler
{

    public string sceneToLoad;
    public float menuValue;
    public void OnPointerClick(PointerEventData pointerEventData)
    {
		SceneLoader sceneLoader = FindFirstObjectByType<SceneLoader>();
		if (sceneToLoad.Equals("NextLevel"))
        {
            string[] level = SceneLoader.LoadParameter.Split("_");
            string nextLevel = (int.Parse(level[0]) + 1) + "_" + level[1];
            SceneLoader.LoadParameter = nextLevel;
			sceneLoader.LoadNextScene("Gears", menuValue);
        }
        else
        {
			SceneLoader.LoadParameter = gameObject.name;
			sceneLoader.LoadNextScene(sceneToLoad, menuValue);
		}
        
        
    }
}
