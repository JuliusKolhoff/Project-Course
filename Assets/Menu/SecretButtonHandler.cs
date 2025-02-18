using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SecretButtonHandler : MonoBehaviour
{
    private const string CODE = "44135";
	private const string RESETCODE = "53144";
	[SerializeField]private TMP_InputField _inputField;
    public void CheckCode()
    {
        if(_inputField.text == CODE)
        {
            SceneLoader.CurrentGearUnlock = int.MaxValue;
            SceneLoader.currentLevel = 5;
			SceneLoader sceneLoader = FindFirstObjectByType<SceneLoader>();
            sceneLoader.LoadNextScene("Main Menu", 1);
		}
		if (_inputField.text == RESETCODE)
		{
			SceneLoader.CurrentGearUnlock = 1;
			SceneLoader.currentLevel = 0;
			SceneLoader sceneLoader = FindFirstObjectByType<SceneLoader>();
			sceneLoader.LoadNextScene("Main Menu", 1);
		}
	}
}
