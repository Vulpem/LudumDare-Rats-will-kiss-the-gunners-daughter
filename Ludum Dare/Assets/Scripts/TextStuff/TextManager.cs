using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DAY_STATE
{
    DAY_DAWN,
    DAY_CHOOSE_CREW,
    DAY_ASK_CREW,
    DAY_NIGHT
}

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
    public DAY_STATE day_state = DAY_STATE.DAY_DAWN;

    public MakeTextAppear textDisplay;
    public Actions[] actions;

    float clickDelay = 0.25f;
    float delayCounter = 0.0f;

    float advanceTimer = -0.1f;
    bool wantToAdvance = false;

    int actionMade = 0;

    Answer activeAnswer;

    // Use this for initialization
    void Start () {
        clickDelay = 0.25f;
        delayCounter = 0.0f;

        advanceTimer = -0.1f;
        wantToAdvance = false;

        actionMade = 0;
        talkingWith = -1;
        SecurityCheck();
    }

    // Update is called once per frame
    void Update()
    {
        delayCounter += Time.deltaTime;

        switch (day_state)
        {
            case DAY_STATE.DAY_DAWN: Dawn();  break;
            case DAY_STATE.DAY_CHOOSE_CREW: ChooseCrew(); break;
            case DAY_STATE.DAY_ASK_CREW: AskCrew(); break;
            case DAY_STATE.DAY_NIGHT: day_state = DAY_STATE.DAY_DAWN; break;
        }
       
    }

    void Dawn()
    {
        foreach(Character pnj in characters)
        {
            pnj.active = false;
            pnj.activeLastTurn = !pnj.active;
        }
        day_state = DAY_STATE.DAY_CHOOSE_CREW;
    }

    void ChooseCrew()
    {
        int active = 0;
        foreach(Character pnj in characters)
        {
            if(pnj.active == true)
            {
                active++;
            }
        }

        if (active == 3)
        {
            day_state = DAY_STATE.DAY_ASK_CREW;
        }
    }

    void AskCrew()
    {
         ManageInputAskCrew();

        if(turn_state == TURN_STATE.STATE_ACTION)
        {
            MakeActionAnimation();
        }
        else if(turn_state == TURN_STATE.STATE_ANSWER)
        {
           
        }
        else if (turn_state == TURN_STATE.STATE_RESULT)
        {
            MakeResult();
        }

        bool dayOver = true;
        foreach(Character pnj in characters)
        {
            if(pnj.active == true)
            {
                dayOver = false;
                break;
            }
        }
        if(dayOver == true)
        {
            day_state = DAY_STATE.DAY_NIGHT;
        }

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
    void ManageInputAskCrew()
    {
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
            if (advanceTimer < 0.15f)
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
            if (day_state == DAY_STATE.DAY_ASK_CREW)
            {
                if (talkingWith == -1 && turn_state == TURN_STATE.STATE_WAITING)
                {
                    delayCounter = 0.0f;
                    StartedTalking(pnj);
                }
            }
            else if (day_state == DAY_STATE.DAY_CHOOSE_CREW)
            {
                pnj.active = true;
            }
        }
    }

    //Began talking with someone
    void StartedTalking(Character pnj)
    {
        if (pnj.active)
        {
            turn_state = TURN_STATE.STATE_TEXT;
            talkingWith = pnj.characterN;
            talkingWith_name = characters[talkingWith].name;
            CreateText(pnj, pnj.bubbles[pnj.activeBubble].text);
        }
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
                actionMade = actionN;
                turn_state = TURN_STATE.STATE_ACTION;
            }
        }
    }

    void MakeActionAnimation()
    {

        turn_state = TURN_STATE.STATE_ANSWER;
        MakeAnswer();
    }

    void MakeAnswer()
    {
        Character pnj = characters[talkingWith];
        switch (actionMade)
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

    void MakeResult()
    {
        foreach(Result res in activeAnswer.result)
        {
           if (res.whom != WHO_AFFECTS.OTHERS)
            {
                characters[talkingWith].Stats[(int)(res.stat)] += res.amount;
                Mathf.Clamp(characters[talkingWith].Stats[(int)(res.stat)], 0, 30);
            }

            if (res.whom != WHO_AFFECTS.ME)
            {
                foreach(Character pnj in characters)
                {
                    if(pnj.characterN != talkingWith)
                    {
                        pnj.Stats[(int)(res.stat)] += res.amount;
                        Mathf.Clamp(pnj.Stats[(int)(res.stat)], 0, 30);
                    }
                }
            }
        }

        characters[talkingWith].active = false;

        turn_state = TURN_STATE.STATE_WAITING;
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

            if(turn_state == TURN_STATE.STATE_ANSWER)
            {
                turn_state = TURN_STATE.STATE_RESULT;
            }
        }
    }
}