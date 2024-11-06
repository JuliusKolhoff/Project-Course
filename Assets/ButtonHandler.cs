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
        SceneLoader.LoadParameter = gameObject.name;
        sceneLoader.LoadNextScene(sceneToLoad,menuValue);
    }
}
