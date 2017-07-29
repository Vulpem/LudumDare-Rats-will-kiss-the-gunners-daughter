using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public string talkingWith_name;

    [Header(" ")]
    [Header(" -- Designers, do not go below this point! -- ")]
    public MakeTextAppear textDisplay;
    public MakeTextAppear[] textOptions;


    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Application.isPlaying)
        {
            if (talkingWith != -1)
            {
                talkingWith_name = characters[talkingWith].name;
            }
            else
            {
                talkingWith_name = "No one";
            }

            ManageInput();
        }

        SecurityCheck();
	}

    void SecurityCheck()
    {
        foreach ( Character pnj in characters)
        {
            if (pnj.manager == null)
            {
                pnj.manager = this;
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

    public void ClickedOnMe(GameObject go, int button)
    {
        Character clickedChar;
        foreach(Character pnj in characters)
        {
            if(pnj == go)
            {
                clickedChar = go.GetComponent<Character>();
                break;
            }
        }

    }

    void Advance()
    {

    }
}