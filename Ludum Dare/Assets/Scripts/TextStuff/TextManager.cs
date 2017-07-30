using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

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
    public Button[] actions;

    public EventManager eventManager;

    public int power = 2;

    float clickDelay = 0.25f;
    float delayCounter = 0.0f;

    float advanceTimer = -0.1f;
    bool wantToAdvance = false;

    public bool blockInteraction = false;

    Dictionary<TODAYS_QUESTION, bool> questionsAsked;

    PLAYER_ACTIONS actionMade = 0;

    // Use this for initialization
    void Start () {
        questionsAsked = new Dictionary<TODAYS_QUESTION, bool>();
        questionsAsked.Add(TODAYS_QUESTION.LAST_DAY, true);

        characters = new SortedDictionary<TYPES, Character>();
        clickDelay = 0.25f;
        delayCounter = 0.0f;

        advanceTimer = -0.1f;
        wantToAdvance = false;

        actionMade = 0;
        talkingWith = TYPES.none;
        SecurityCheck();

        GenerateCharacters();

        BeginDay();
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

        bool endDay = true;
        if(textDisplay.UIText.text.Length > 3 || textDisplay.working == true )
        {
            endDay = false;
        }
        foreach(Character pnj in CharacterGOs)
        {
            if(pnj.doneForToday == false) { endDay = false; }
        }
        if(endDay)
        {
            EndDay();
        }
    }

    void HideActions()
    {
        if (talkingWith == TYPES.none || textDisplay.working == true || characters[talkingWith].doneForToday == true)
        {
            foreach (Button c in actions)
            {
                c.gameObject.SetActive(false);
            }
        }
        else
        {
            foreach (Button c in actions)
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

           c.name = GetComponent<NameBank>().GetName();
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
        LoadDialogues();
    }

    void LoadDialogues()
    {
        TextAsset[][] bank = new TextAsset[5][];
        bank[(int)TODAYS_QUESTION.WHOS_TRAITOR] = Resources.LoadAll<TextAsset>("Dialogue/whos_traitor");
        bank[(int)TODAYS_QUESTION.WHO_LIES] = Resources.LoadAll<TextAsset>("Dialogue/who_lies");
        bank[(int)TODAYS_QUESTION.WHO_TO_TRUST] = Resources.LoadAll<TextAsset>("Dialogue/who_to_trust");
        bank[(int)TODAYS_QUESTION.WOULD_YOU_LIE_TO_ME] = Resources.LoadAll<TextAsset>("Dialogue/would_you_lie_to_me");
        bank[(int)TODAYS_QUESTION.LAST_DAY] = Resources.LoadAll<TextAsset>("Dialogue/last_day");

        int n = 0;
        foreach (TextAsset[] dayBank in bank)
        {
            foreach(TextAsset file in dayBank)
            {
                string[] dirtyTexts = file.text.Split('\n');
                List<string> phrases = new List<string>();
                foreach(string str in dirtyTexts)
                {
                    if(str[0] != '#' && str.Length > 2)
                    {
                        phrases.Add(str);
                    }
                }
                TYPES type = new TYPES();
                switch (phrases[0])
                {
                    case "<rioter>": type = TYPES.rioter; break;
                    case "<brute>": type = TYPES.brute; break;
                    case "<sea_wolf>": type = TYPES.sea_wolf; break;
                    case "<stingy>": type = TYPES.stingy; break;
                    case "<comrad>": type = TYPES.comrad; break;
                }

                characters[type].bubbles[(TODAYS_QUESTION)(n)].text = phrases[1];
                characters[type].bubbles[(TODAYS_QUESTION)(n)].busy = phrases[2];
                characters[type].bubbles[(TODAYS_QUESTION)(n)].answers[PLAYER_ACTIONS.AGRESSION] = phrases[3];
                characters[type].bubbles[(TODAYS_QUESTION)(n)].answers[PLAYER_ACTIONS.PEACEFUL] = phrases[4];
                characters[type].bubbles[(TODAYS_QUESTION)(n)].answers[PLAYER_ACTIONS.BRIVE] = phrases[5];
            }
            n++;
        }
        

    }

    void EndDay()
    {
        BeginDay();
    }

    void BeginDay()
    {
        day++;
        power = 2;
        foreach (Character pnj in CharacterGOs)
        {
            pnj.active = true;
            pnj.activeLastFrame = false;
            pnj.doneForToday = false;
        }
        if(day >= 2 && question != TODAYS_QUESTION.LAST_DAY)
        {
            blockInteraction = true;
            eventManager.LaunchEvent();
        }

        if (day < 5)
        {
            int n = UnityEngine.Random.Range(0, 5);
            while(questionsAsked.ContainsKey((TODAYS_QUESTION)(n)))
            {
                n++;
                if(n >= (int)TODAYS_QUESTION.LAST_DAY) { n = 0; }
            }
            question = (TODAYS_QUESTION)n;
            questionsAsked.Add(question, true);
        }
        else
        {
            question = TODAYS_QUESTION.LAST_DAY;
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

    public void MakeAction(int actionN)
    {
        if (delayCounter > clickDelay && blockInteraction == false)
        {
            if (characters[talkingWith].doneForToday == false)
            {
                delayCounter = 0.0f;
                actionMade = (PLAYER_ACTIONS)actionN;
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
        tmp = tmp.Replace("<sea_wolf>", characters[TYPES.sea_wolf].name);
        tmp = tmp.Replace("<stingy>", characters[TYPES.stingy].name);
        tmp = tmp.Replace("<comrad>", characters[TYPES.comrad].name);

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
 