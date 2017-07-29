using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SpeechBubble
{
    string name;
    public string Text;
}

[System.Serializable]
public class Character
{
    public string name;
    public string descrpition;
    public SpeechBubble[] bubbles;
    TextManager manager;
}

[System.Serializable]
public class Name
{
    public string name;
    [HideInInspector]
    public bool chosen = false;
}

[ExecuteInEditMode]
public class TextManager : MonoBehaviour {

    [Header(" -- List of characters -- ")]
    public Character[] characters;

    [Header(" -- Character you're currently talking with -- ")]
    public int talkingWith;
    public string name_talking_with;

    [Header(" ")]
    [Header(" -- Designers, do not go below this point! -- ")]
    public MakeTextAppear textDisplay;
    public MakeTextAppear[] options;

    public string[] randomNames;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Application.isPlaying)
        {
            ManageInput();
        }
        else
        {
            SetNames();
        }
	}

    void SetNames()
    {
        foreach( Character pnj in characters)
        {
            if(pnj.name == "")
            {
                pnj.name = randomNames[Random.Range(0, randomNames.Length)];
            }
        }
    }

    void ManageInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (textDisplay.working)
            {
                textDisplay.Skip();
            }
            else
            {
                Advance();
            }
        }
    }

    void Advance()
    {

    }
}