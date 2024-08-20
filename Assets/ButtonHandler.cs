using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string sceneToLoad;
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        SceneLoader sceneLoader = FindFirstObjectByType<SceneLoader>();
        sceneLoader.LoadNextScene(sceneToLoad);
    }
}
