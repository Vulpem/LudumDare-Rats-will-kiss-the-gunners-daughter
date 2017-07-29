using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum TODAYS_QUESTION
{
    WHOS_TRAITOR,
    WHO_LIES,
    WHO_TO_TRUST,
    WOULD_YOU_LIE_TO_ME,
    LAST_DAY
}

public class TextManager : MonoBehaviour {

    [Header(" -- List of characters -- ")]
    public Character[] CharacterGOs;
    public SortedDictionary<TYPES, Character> characters;

    [Header(" -- Character you're currently talking with -- ")]
    public TYPES talkingWith;
    public string talkingWith_name;

    [Header("More stuff")]
    public TODAYS_QUESTION question;
    public int day = 0;

    public MakeTextAppear textDisplay;
    public Actions[] actions;

    public EventManager eventManager;

    public int power = 2;

    float clickDelay = 0.25f;
    float delayCounter = 0.0f;

    float advanceTimer = -0.1f;
    bool wantToAdvance = false;

    public bool blockInteraction = false;

    PLAYER_ACTIONS actionMade = 0;

    // Use this for initialization
    void Start () {
        characters = new SortedDictionary<TYPES, Character>();
        clickDelay = 0.25f;
        delayCounter = 0.0f;

        advanceTimer = -0.1f;
        wantToAdvance = false;

        actionMade = 0;
        talkingWith = TYPES.none;
        SecurityCheck();

        GenerateCharacters();
    }

    // Update is called once per frame
    void Update()
    {
        if (blockInteraction == false)
        {
            delayCounter += Time.deltaTime;
            ManageInputAskCrew();
        }
        HideActions();
    }

    void HideActions()
    {
        if (talkingWith == TYPES.none || textDisplay.working == true || characters[talkingWith].doneForToday == true)
        {
            foreach (Actions c in actions)
            {
                c.gameObject.SetActive(false);
            }
        }
        else
        {
            foreach (Actions c in actions)
            {
                c.gameObject.SetActive(true);
            }
        }
    }

    void GenerateCharacters()
    {
        foreach (Character c in CharacterGOs)
        {
            TYPES type = new TYPES();

           type = (TYPES)(UnityEngine.Random.Range(0, (float)(TYPES.none)));

            while (characters.ContainsKey(type) || type == TYPES.none)
            {
                type++;
                if (type >= TYPES.none)
                {
                    type = 0;
                }
            }

            c.type = type;

            //TODO, to load speech bubbles
            for(int q = 0; q <= (int)(TODAYS_QUESTION.LAST_DAY); q++)
            {
                SpeechBubble bubble = new SpeechBubble();
                bubble.text = "Huh, what's up captain?";
                bubble.busy = "Can't you see I'm busy now?";
                bubble.answers.Add(PLAYER_ACTIONS.AGRESSION, "You won't get anything from me with violence. Not even <brute> scares me.");
                bubble.answers.Add(PLAYER_ACTIONS.BRIVE, "I have no need for money. Try giving it to <stingy>.");
                bubble.answers.Add(PLAYER_ACTIONS.PEACEFUL, "Sorry, i have nothing to say. Try asking <comrade>.");

                c.bubbles.Add((TODAYS_QUESTION)(q), bubble);
            }

            characters.Add(type, c);
        }
    }

    void BeginDay()
    {
        day++;
        power = 2;
        foreach (Character pnj in CharacterGOs)
        {
            if(pnj.doneForToday ? pnj.active = false : pnj.active = true);
            pnj.activeLastFrame = true;
            pnj.doneForToday = false;
        }
        if(day >= 2 && question != TODAYS_QUESTION.LAST_DAY)
        {
            blockInteraction = true;
            eventManager.LaunchEvent();
        }
    }

    //Sets everything at start
    void SecurityCheck()
    {
        int n = 0;
        foreach (Character pnj in CharacterGOs)
        {
            pnj.manager = this;
            pnj.characterN = n;
            n++;
        }
        n = 0;
        foreach (Actions act in actions)
        {
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
                Minimize();
            }
        }
    }

    //A character has recieved a click
    public void ClickedOnMe(GameObject go, int button)
    {
        if (delayCounter > clickDelay && blockInteraction == false)
        {
            if (textDisplay.working == false)
            {
                Character pnj = go.GetComponent<Character>();
                delayCounter = 0.0f;
                StartedTalking(pnj);
            }
        }
    }

    //Began talking with someone
    void StartedTalking(Character pnj)
    {
        if(pnj.type == talkingWith)
        {
            return;
        }

       foreach(Character t in CharacterGOs)
        {
            t.active = false;
        }
        pnj.active = true;
        talkingWith = pnj.type;
        talkingWith_name = pnj.name;

        if (pnj.doneForToday == false)
        {
            CreateText(pnj, pnj.bubbles[question].text);
        }
        else
        {
            CreateText(pnj, pnj.bubbles[question].busy);
        }
    }

    //Stopped talking with someone
    void StoppedTalking()
    {
        talkingWith = TYPES.none;
        talkingWith_name = "No one";
        textDisplay.Clean();
        foreach (Character pnj in CharacterGOs)
        {
            if (pnj.doneForToday ? pnj.active = false : pnj.active = true);
        }
    }

    void MakeAction(PLAYER_ACTIONS actionN)
    {
        if (delayCounter > clickDelay && blockInteraction == false)
        {
            if (characters[talkingWith].doneForToday == false)
            {
                delayCounter = 0.0f;
                actionMade = actionN;
                MakeAnswer();
            }
            else
            {
                StoppedTalking();
            }
        }
    }

    void MakeAnswer()
    {
        if (talkingWith != TYPES.none)
        {
            Character pnj = characters[talkingWith];
            pnj.doneForToday = true;

            if (pnj.type == TYPES.sea_wolf && actionMade != PLAYER_ACTIONS.PEACEFUL)
            {
                pnj.angryCount++;
                CreateText(pnj, pnj.bubbles[question].answers[actionMade]);
            }
            else
            {
                CreateText(pnj, pnj.bubbles[question].answers[actionMade]);
            }
        }
    }

    void CreateText(Character characterTalking, string text)
    {
        string tmp = characterTalking.name;
        tmp += " : ";
        tmp += text;

        tmp = tmp.Replace("<rioter>", characters[TYPES.rioter].name);
        tmp = tmp.Replace("<brute>", characters[TYPES.brute].name);
        tmp = tmp.Replace("<sea wolf>", characters[TYPES.sea_wolf].name);
        tmp = tmp.Replace("<stingy>", characters[TYPES.stingy].name);
        tmp = tmp.Replace("<comrade>", characters[TYPES.comrade].name);

        textDisplay.Begin(tmp);
    }

    void Minimize()
    {
        if(textDisplay.working == false && talkingWith != TYPES.none)
        {
            delayCounter = 0.0f;
            StoppedTalking();
        }
    }
}
 