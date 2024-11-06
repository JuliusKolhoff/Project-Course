using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHandler : MonoBehaviour, IPointerClickHandler
{

    [SerializeField] public string sceneToLoad;
    [SerializeField] private float menuValue;
    public void OnPointerClick(PointerEventData pointerEventData)
    {
		SceneLoader sceneLoader = FindFirstObjectByType<SceneLoader>();
		if (sceneToLoad.Equals("NextLevel"))
        {
            string[] level = SceneLoader.LoadParameter.Split();
            string nextLevel = level[0] + " " + (int.Parse(level[1]) + 1);
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
