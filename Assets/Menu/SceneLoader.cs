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
    [SerializeField] private float startMusic = 1f;
    public static string LoadParameter = "Level 1";
    public static int currentLevel = 0;
    public static int CurrentGearUnlock = 1;
	public void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("SceneLoader");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Menu", startMusic);
    }

    public void LoadNextScene(string sceneName, float menuValue)
    {
        GameObject.Find("EventSystem").SetActive(false);
		StartCoroutine(LoadScene(sceneName));
        StartCoroutine(ChangeValue(menuValue));
        DontDestroyOnLoad(this);
	}

    IEnumerator LoadScene(string sceneName)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionDuration);

		transition.SetTrigger("End");

		SceneManager.LoadScene(sceneName);
	}

    IEnumerator ChangeValue(float menuValue)
    {
        while (true)
        {
            float temp;
            var result = FMODUnity.RuntimeManager.StudioSystem.getParameterByName("Menu", out temp);
            if (temp != menuValue)
            {
                temp = Mathf.MoveTowards(temp, menuValue, 0.5f * Time.fixedDeltaTime);
            }
            else
            {
                yield break;
            }
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Menu", temp);
            yield return null;
        }
    }
}
