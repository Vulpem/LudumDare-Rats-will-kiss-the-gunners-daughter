using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TURN_STATE
{
    STATE_WAITING,
    STATE_TEXT,
    STATE_ACTION,
    STATE_ANSWER,
    STATE_RESULT,
    STATE_LAST
}

[System.Serializable]
public class Name
{
    public string name;
    [HideInInspector]
    public bool chosen = false;
}

public class TextManager : MonoBehaviour {

    [Header(" -- List of characters -- ")]
    public Character[] characters;

    [Header(" -- Character you're currently talking with -- ")]
    public int talkingWith;
    public string talkingWith_name;

    [Header(" ")]
    [Header(" -- Designers, do not go below this point! -- ")]
    public TURN_STATE turn_state = TURN_STATE.STATE_WAITING;
    public MakeTextAppear textDisplay;
    public MakeTextAppear[] textOptions;

    float clickDelay = 0.25f;
    float delayCounter = 0.0f;

    // Use this for initialization
    void Start () {
        talkingWith = -1;
        SecurityCheck();
    }
	
	// Update is called once per frame
	void Update ()
    {
            if (talkingWith != -1)
            {
                talkingWith_name = characters[talkingWith].name;
            }
            else
            {
                talkingWith_name = "No one";
            }

        delayCounter += Time.deltaTime;
            ManageInput();        
	}

    void SecurityCheck()
    {
        int n = 0;
        foreach ( Character pnj in characters)
        {
            pnj.manager = this;
            pnj.characterN = n;
            n++;
        }
    }

    void ManageInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonUp(0))
        {
            if (delayCounter > clickDelay)
            {
                if (textDisplay.working)
                {
                    textDisplay.Skip();
                    delayCounter = 0.0f;
                }
                else
                {
                    Advance();
                }
            }
        }
    }

    public void ClickedOnMe(GameObject go, int button)
    {
        if (delayCounter > clickDelay)
        {
            Character pnj = go.GetComponent<Character>();
            if (pnj != null)
            {
                if (talkingWith == -1)
                {
                    delayCounter = 0.0f;
                    talkingWith = pnj.characterN;
                    CreateText(pnj, pnj.bubbles[0].text);
                }
            }
        }
    }

    void CreateText(Character character, string text)
    {
        string tmp = character.name;
        tmp += " : ";
        tmp += text;

        textDisplay.Begin(tmp);
    }

    void Advance()
    {
        if(textDisplay.working == false && talkingWith != -1)
        {
            delayCounter = 0.0f;
            textDisplay.Clean();
            talkingWith = -1;
        }
    }
}