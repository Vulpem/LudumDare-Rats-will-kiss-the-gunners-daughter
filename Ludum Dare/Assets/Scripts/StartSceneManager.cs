using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

    public class StartSceneManager : MonoBehaviour {

    public FadeManager whiteScreen;
    bool wantToLoad;

    void Start()
    {
        wantToLoad = false;
    }

    void Update()
    {
        if (wantToLoad && whiteScreen.Working() == false)
        {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
    }

    public void LoadGame(int language)
    {
        VariableStorage.Gen();
        if (VariableStorage.ints.ContainsKey("lang"))
        {
            VariableStorage.ints["lang"] = language;
        }
        else
        {
            VariableStorage.ints.Add("lang", language);
        }

        wantToLoad = true;        
    }
}
