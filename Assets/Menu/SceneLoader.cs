using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Animator transition;
    [SerializeField] private float transitionDuration = 1f;
    public void LoadNextScene(string sceneName)
    {
        DontDestroyOnLoad(this);
		StartCoroutine(LoadScene(sceneName));
	}
    IEnumerator LoadScene(string sceneName)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionDuration);

		transition.SetTrigger("End");

		SceneManager.LoadScene(sceneName);

		
	}
}
