using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

enum LANGUAGE
{
    ENGLISH = 0,
    SPANISH = 1,
    CATALAN = 2
}

public enum TODAYS_QUESTION
{
    WHOS_TRAITOR,
    WHO_LIES,
    WHO_TO_TRUST,
    WOULD_YOU_LIE_TO_ME,
    LAST_DAY,
    First_MESSAGE
}

public class TextManager : MonoBehaviour {

    Dictionary<TODAYS_QUESTION, string> questions;
    Dictionary<TODAYS_QUESTION, bool> questionsAsked;
    string[] angrySeaWolf;
    string noPower;

    [HideInInspector]
    public string txtRoute;

    bool questionableDecisions = false;

    bool dayOver = false;

    float clickDelay = 0.25f;
    float delayCounter = 0.0f;

    float advanceTimer = -0.1f;
    bool wantToAdvance = false;

    PLAYER_ACTIONS actionMade = 0;

    [Header("Things that need to be set up Manually")]

    [Tooltip("List of characters")]
    public Character[] CharacterGOs;
    [HideInInspector]
    public SortedDictionary<TYPES, Character> characters;

    public MakeTextAppear textDisplay;
    public Button[] actions;

    public EventManager eventManager;
    public GameObject FadeInOut;

    [Header("Debug Info")]
    public TYPES talkingWith;
    public string talkingWith_name;
    [Range(0, 5)]
    public int power = 2;
    public TODAYS_QUESTION question;
    public int day = 0;
    public bool blockInteraction = false;

    // Use this for initialization
    void Awake()
    {
        switch (VariableStorage.ints["lang"])
        {
            case (int)(LANGUAGE.SPANISH): txtRoute = "txts/Sp/"; break;
            case (int)(LANGUAGE.CATALAN): txtRoute = "txts/Cat/"; break;
            default: txtRoute = "txts/En/"; break;
        }
    }

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
        blockInteraction = true;
        BeginDay();
        question = TODAYS_QUESTION.First_MESSAGE;
        day = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(question == TODAYS_QUESTION.First_MESSAGE)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (textDisplay.working)
                {
                    textDisplay.Skip();
                }
                else
                {
                    if (day < 5)
                    {
                        int n = UnityEngine.Random.Range(0, 5);
                        while (questionsAsked.ContainsKey((TODAYS_QUESTION)(n)))
                        {
                            n++;
                            if (n >= (int)TODAYS_QUESTION.LAST_DAY) { n = 0; }
                        }
                        question = (TODAYS_QUESTION)n;
                        questionsAsked.Add(question, true);
                    }
                    else
                    {
                        question = TODAYS_QUESTION.LAST_DAY;
                    }
                    blockInteraction = false;
                    textDisplay.Clean();
                    delayCounter = 0.0f;
                }
            }
        }

        if (blockInteraction == false)
        {
            delayCounter += Time.deltaTime;
            ManageInputAskCrew();

            if(textDisplay.UIText.text.Length < 2 && textDisplay.working == false && dayOver == false)
            {
                CreateText("", questions[question]);
            }
        }

        HideActions();

        if (dayOver == false)
        {
            bool endDay = true;
            if (textDisplay.UIText.text.Length > 3 || talkingWith != TYPES.none)
            {
                endDay = false;
            }
            foreach (Character pnj in CharacterGOs)
            {
                if (pnj.doneForToday == false) { endDay = false; }
            }
            if (endDay)
            {
                EndDay();
            }
        }
    }

    void ChooseTodayQuestion()
    {
        if (day < 5)
        {
            int n = UnityEngine.Random.Range(0, 5);
            while (questionsAsked.ContainsKey((TODAYS_QUESTION)(n)))
            {
                n++;
                if (n >= (int)TODAYS_QUESTION.LAST_DAY) { n = 0; }
            }
            question = (TODAYS_QUESTION)n;
            questionsAsked.Add(question, true);
        }
        else
        {
            question = TODAYS_QUESTION.LAST_DAY;
        }
    }

    void HideActions()
    {
        if (talkingWith == TYPES.none || textDisplay.working == true || characters[talkingWith].doneForToday == true || questionableDecisions == true)
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

           c.SetName(GetComponent<NameBank>().GetName());

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
        bank[(int)TODAYS_QUESTION.WHOS_TRAITOR] = Resources.LoadAll<TextAsset>(txtRoute + "Dialogue/whos_traitor");
        bank[(int)TODAYS_QUESTION.WHO_LIES] = Resources.LoadAll<TextAsset>(txtRoute + "Dialogue/who_lies");
        bank[(int)TODAYS_QUESTION.WHO_TO_TRUST] = Resources.LoadAll<TextAsset>(txtRoute + "Dialogue/who_to_trust");
        bank[(int)TODAYS_QUESTION.WOULD_YOU_LIE_TO_ME] = Resources.LoadAll<TextAsset>(txtRoute + "Dialogue/would_you_lie_to_me");
        bank[(int)TODAYS_QUESTION.LAST_DAY] = Resources.LoadAll<TextAsset>(txtRoute + "Dialogue/last_day");

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
                        phrases.Add(str.Substring(0, str.Length - 1));
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

        {
            angrySeaWolf = new string[3];

            TextAsset[] general = Resources.LoadAll<TextAsset>(txtRoute + "Dialogue");
            string[] dirtyTexts = general[0].text.Split('\n');
            List<string> phrases = new List<string>();
            foreach (string str in dirtyTexts)
            {
                if (str[0] != '#' && str.Length > 2)
                {
                    phrases.Add(str.Substring(0, str.Length - 1));
                }
            }

            questions = new Dictionary<TODAYS_QUESTION, string>();

            questions.Add(TODAYS_QUESTION.First_MESSAGE, phrases[0]);

            questions.Add(TODAYS_QUESTION.WHOS_TRAITOR, phrases[1]);
            questions.Add(TODAYS_QUESTION.WHO_LIES, phrases[2]);
            questions.Add(TODAYS_QUESTION.WHO_TO_TRUST, phrases[3]);
            questions.Add(TODAYS_QUESTION.WOULD_YOU_LIE_TO_ME, phrases[4]);
            questions.Add(TODAYS_QUESTION.LAST_DAY, phrases[5]);

            noPower = phrases[6];

            angrySeaWolf[0] = phrases[7];
            angrySeaWolf[1] = phrases[8];
            angrySeaWolf[2] = phrases[9];
        }
    }

    void EndDay()
    {
        dayOver = true;
        day++;
        textDisplay.Clean();
        FadeInOut.SetActive(true);
       // BeginDay();
    }

    public void BeginDay()
    {
        dayOver = false;
        power = 2;
        foreach (Character pnj in CharacterGOs)
        {
            if (pnj.angryCount < 2)
            {
                pnj.SetActive(true);
                pnj.doneForToday = false;
            }
            else
            {
                pnj.SetActive(false);
                pnj.doneForToday = true;
            }
        }

        ChooseTodayQuestion();

        if (day >= 2 && question != TODAYS_QUESTION.LAST_DAY)
        {
            blockInteraction = true;
            eventManager.LaunchEvent();
        }
        else
        {
            CreateText("", questions[TODAYS_QUESTION.First_MESSAGE]);
            EndedEvent();
        }
    }

    public void EndedEvent()
    {
        blockInteraction = false;
        CreateText("", questions[question]);
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
        if (delayCounter > clickDelay && blockInteraction == false && question != TODAYS_QUESTION.First_MESSAGE)
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

        questionableDecisions = false;

       foreach (Character t in CharacterGOs)
        {
            t.SetActive(false);
        }
        pnj.SetActive(true);
        talkingWith = pnj.type;
        talkingWith_name = pnj.name;

        if (pnj.doneForToday == false)
        {
            CreateText(pnj.name, pnj.bubbles[question].text);
        }
        else
        {
            if (pnj.angryCount < 2)
            {
                CreateText(pnj.name, pnj.bubbles[question].busy);
            }
            else
            {
                CreateText(pnj.name, angrySeaWolf[2]);
            }
        }
    }

    //Stopped talking with someone
    void StoppedTalking()
    {
        questionableDecisions = false;
        talkingWith = TYPES.none;
        talkingWith_name = "No one";
        textDisplay.Clean();
        foreach (Character pnj in CharacterGOs)
        {
            if(pnj.doneForToday) { pnj.SetActive(false); }
            else { pnj.SetActive(true); }
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
                if (actionMade != PLAYER_ACTIONS.PEACEFUL && power <= 0)
                {
                    CreateText("", noPower);
                    questionableDecisions = true;
                }
                else
                {
                    MakeAnswer();
                }
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

            if (actionMade != PLAYER_ACTIONS.PEACEFUL)
            {
                power--;
            }

            if (pnj.type == TYPES.sea_wolf && actionMade != PLAYER_ACTIONS.PEACEFUL)
            {
                switch (pnj.angryCount)
                {
                    case 0: CreateText(pnj.name, angrySeaWolf[0]); break;
                    case 1: CreateText(pnj.name, angrySeaWolf[1]); break;
                    default: CreateText(pnj.name, angrySeaWolf[2]); break;
                }
                pnj.angryCount++;
                
            }
            else
            {
                CreateText(pnj.name, pnj.bubbles[question].answers[actionMade]);
            }
        }
    }

    void CreateText(string characterTalking, string text)
    {
        string tmp;
        if (characterTalking.Length > 1)
        {
            tmp = characterTalking;
            tmp += " : ";
            tmp += text;
        }
        else
        {
            tmp = text;
        }

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
 