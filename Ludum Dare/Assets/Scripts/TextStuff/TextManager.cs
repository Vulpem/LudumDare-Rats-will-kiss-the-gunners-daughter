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
    public Actions[] actions;

    float clickDelay = 0.25f;
    float delayCounter = 0.0f;

    float advanceTimer = -0.1f;
    bool wantToAdvance = false;

    Answer activeAnswer;

    // Use this for initialization
    void Start () {
        talkingWith = -1;
        SecurityCheck();
    }

    // Update is called once per frame
    void Update()
    {
        ManageInput();
    }

    //Sets everything at start
    void SecurityCheck()
    {
        int n = 0;
        foreach ( Character pnj in characters)
        {
            pnj.manager = this;
            pnj.characterN = n;
            n++;
        }
        n = 0;
        foreach (Actions act in actions)
        {
            act.action_n = n;
            act.manager = this;
            n++;
        }
    }

    //Manages player input
    void ManageInput()
    {
        delayCounter += Time.deltaTime;

        if (Input.GetMouseButtonUp(0))
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
                    wantToAdvance = true;
                    advanceTimer = 0.0f;
                }
            }
        }

        if(wantToAdvance)
        {
            if (advanceTimer < 0.25f)
            {
                advanceTimer += Time.deltaTime;
            }
            else
            {
                wantToAdvance = false;
                Advance();
            }
        }
    }

    //A character has recieved a click
    public void ClickedOnMe(GameObject go, int button)
    {
        if (delayCounter > clickDelay)
        {
            Character pnj = go.GetComponent<Character>();
            if (pnj != null)
            {
                if (talkingWith == -1 && turn_state == TURN_STATE.STATE_WAITING)
                {
                    delayCounter = 0.0f;
                    StartedTalking(pnj);
                }
            }
        }
    }

    //Began talking with someone
    void StartedTalking(Character pnj)
    {
        turn_state = TURN_STATE.STATE_TEXT;
        talkingWith = pnj.characterN;
        talkingWith_name = characters[talkingWith].name;
        CreateText(pnj, pnj.bubbles[pnj.activeBubble].text);
    }

    //Stopped talking with someone
    void StoppedTalking()
    {
        turn_state = TURN_STATE.STATE_WAITING;
        talkingWith = -1;
        talkingWith_name = "No one";
        textDisplay.Clean();
    }

    void MakeAction(int actionN)
    {
        if (delayCounter > clickDelay)
        {
            delayCounter = 0.0f;
            if (turn_state == TURN_STATE.STATE_TEXT)
            {
                turn_state = TURN_STATE.STATE_ANSWER;
                Character pnj = characters[talkingWith];
                switch (actionN)
                {
                    case 0: activeAnswer = pnj.bubbles[pnj.activeBubble].Action1; break;
                    case 1: activeAnswer = pnj.bubbles[pnj.activeBubble].Action2; break;
                    case 2: activeAnswer = pnj.bubbles[pnj.activeBubble].Action3; break;
                    case 3: activeAnswer = pnj.bubbles[pnj.activeBubble].Action4; break;
                    case 4: activeAnswer = pnj.bubbles[pnj.activeBubble].Action5; break;
                }
                talkingWith = pnj.characterN;
                talkingWith_name = pnj.name;
                CreateText(pnj, activeAnswer.text);
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
            StoppedTalking();
        }
    }
}