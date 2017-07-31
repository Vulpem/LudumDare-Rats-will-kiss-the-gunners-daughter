using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

    public class StartSceneManager : MonoBehaviour {

    public Text UI_Play;
    public Text UI_Exit;
    public Text UI_Credits;
    public FadeManager CreditScreen;

    public LANGUAGE language;
    public FadeManager whiteScreen;
    bool wantToLoad;

    void Start()
    {
        wantToLoad = false;
        language = LANGUAGE.ENGLISH;
    }

    void Update()
    {
        if (wantToLoad && whiteScreen.Working() == false)
        {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
    }

    public void SetLang(int _language)
    {
        language = (LANGUAGE)_language;
        switch (language)
        {
            case LANGUAGE.CATALAN:
                {
                    UI_Play.text = "Jugar";
                    UI_Exit.text = "Sortir";
                    UI_Credits.text = "Crédits";
                    break;
                }
            default:
                {
                    UI_Play.text = "Play";
                    UI_Exit.text = "Exit";
                    UI_Credits.text = "Credits";
                    break;
                }
        }
    }

    public void OpenCredits()
    {
        CreditScreen.In();
    }

    public void CloseCredits()
    {
        CreditScreen.Out();
    }

    public void ToggleCredits()
    {
        if(CreditScreen.gameObject.activeInHierarchy)
        {
            CreditScreen.Out();
        }
        else
        {
            CreditScreen.In();
        }
    }

    public void LoadGame()
    {
        VariableStorage.Gen();
        if (VariableStorage.ints.ContainsKey("lang"))
        {
            VariableStorage.ints["lang"] = (int)language;
        }
        else
        {
            VariableStorage.ints.Add("lang", (int)language);
        }

        wantToLoad = true;        
    }

    public void CloseApp()
    {
        Application.Quit();
    }
}
