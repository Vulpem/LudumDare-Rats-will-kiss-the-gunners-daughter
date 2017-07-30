using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

    public class StartSceneManager : MonoBehaviour {

    public void LoadGame(int language)
    {
        if (VariableStorage.ints.ContainsKey("lang"))
        {
            VariableStorage.ints["lang"] = language;
        }
        else
        {
            VariableStorage.ints.Add("lang", language);
        }

        SceneManager.LoadScene(1, LoadSceneMode.Single);        
    }
}
