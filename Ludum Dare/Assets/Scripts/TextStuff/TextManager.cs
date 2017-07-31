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
    LAST_DAY
}

public enum DAY_STATE
{
    ONE_SKULL,
    TWO_SKULL_NOTE,
    THREE_NOTE,
    FOUR_NOTE_NOTE,
    FIVE_DOOR,
    SIX_TALKING,
    SEVEN_CHOOSE_ACTION,
    EIGHT_ANSWER,
    NINE_NIGHT_EVENT
}

public class TextManager : MonoBehaviour {

    Dictionary<TODAYS_QUESTION, string> questions;
    Dictionary<TODAYS_QUESTION, bool> questionsAsked;
    string[] angrySeaWolf;
    string noPower;
    LANGUAGE language;
    public DAY_STATE state;

    [HideInInspector]
    public string txtRoute;

    float clickDelay = 0.25f;
    float delayCounter = 0.0f;

    float advanceTimer = -0.1f;
    bool wantToAdvance = false;

    PLAYER_ACTIONS actionMade = 0;

    [Header("Things that need to be set up Manually")]

    public GameObject dismissPNJ;
    public GameObject dismissNight;
    public CardsManager cardManager;
    public CardsManager skullManager;
    [Tooltip("List of characters")]
    public Character[] CharacterGOs;
    [HideInInspector]
    public SortedDictionary<TYPES, Character> characters;

    public MakeTextAppear questionDisplay;
    public MakeTextAppear textDisplay;
    public Button[] actions;

    public EventManager eventManager;
    public GameObject FadeInOut;

    public GameObject talkingPos;
    public GameObject restingPos;

    [Header("Debug Info")]
    public int talkingWithN;
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
        VariableStorage.Gen();
        if (VariableStorage.ints.ContainsKey("lang"))
        {
            switch (VariableStorage.ints["lang"])
            {
                case (int)(LANGUAGE.SPANISH): txtRoute = "txts/Sp/"; break;
                case (int)(LANGUAGE.CATALAN): txtRoute = "txts/Cat/"; break;
                default: txtRoute = "txts/En/"; break;
            }
            language = (LANGUAGE)VariableStorage.ints["lang"];
        }
        else
        {
            txtRoute = "txts/En/";
            language = LANGUAGE.ENGLISH;
        }
        state = DAY_STATE.ONE_SKULL;
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
        LoadDialogues();

        blockInteraction = true;
        BeginDay();
        day = 1;
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Character pnj in CharacterGOs)
        {
            pnj.gameObject.transform.position = restingPos.transform.position;
        }
        if (talkingWith != TYPES.none)
        {
            characters[talkingWith].transform.position = talkingPos.transform.position;
        }

        if(state == DAY_STATE.SIX_TALKING && textDisplay.working == false)
        {
            state = DAY_STATE.SEVEN_CHOOSE_ACTION;
        }

        if(state == DAY_STATE.EIGHT_ANSWER && textDisplay.working == false)
        {
            if (talkingWithN >= characters.Count - 1)
            {
                dismissNight.SetActive(true);
            }
            else
            {
                dismissPNJ.SetActive(true);
            }
        }
        else
        {
            dismissNight.SetActive(false);
            dismissPNJ.SetActive(false);
        }

        if(Input.GetMouseButtonDown(0))
        {
            textDisplay.Skip();
        }

        HideActions();
    }

    public void StartNightEvent()
    {
        questionDisplay.Clean();
        textDisplay.Clean();
        //TODO
    }

    public void ClickedOnSkull()
    {
        if(state >= DAY_STATE.ONE_SKULL && state != DAY_STATE.NINE_NIGHT_EVENT && blockInteraction == false)
        {
            blockInteraction = true;
            skullManager.OnClickCard();
            if (state == DAY_STATE.ONE_SKULL)
            {
                state = DAY_STATE.TWO_SKULL_NOTE;
            }
        }
    }

    public void CloseSkull()
    {
        if(state >= DAY_STATE.TWO_SKULL_NOTE)
        {
            blockInteraction = false;
            skullManager.OnExitButton();
            if (state == DAY_STATE.TWO_SKULL_NOTE)
            {
                state = DAY_STATE.THREE_NOTE;
            }
        }
    }

    public void ClickedOnParchement()
    {
        if(state >= DAY_STATE.THREE_NOTE && state != DAY_STATE.NINE_NIGHT_EVENT && blockInteraction == false)
        {
            blockInteraction = true;
            cardManager.OnClickCard();
            if (state == DAY_STATE.THREE_NOTE)
            {
                state = DAY_STATE.FOUR_NOTE_NOTE;
            }
        }
    }

    public void CloseParchement()
    {
        if (state >= DAY_STATE.FOUR_NOTE_NOTE)
        {
            blockInteraction = false;
            cardManager.OnExitButton();
            if (state == DAY_STATE.FOUR_NOTE_NOTE)
            {
                state = DAY_STATE.FIVE_DOOR;
                questionDisplay.Begin(questions[question]);
            }
        }
    }

    public void ClickedOnDoor()
    {
        if(state == DAY_STATE.FIVE_DOOR && blockInteraction == false)
        {
            talkingWith = CharacterGOs[talkingWithN].type;
            state = DAY_STATE.SIX_TALKING;
            CreateText(characters[talkingWith].name, characters[talkingWith].bubbles[question].text);
        }
    }

    public void MakeAction(int act)
    {
        if(state == DAY_STATE.SEVEN_CHOOSE_ACTION && blockInteraction == false)
        {
            state = DAY_STATE.EIGHT_ANSWER;
            actionMade = (PLAYER_ACTIONS)act;
            MakeAnswer();
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
        if (state != DAY_STATE.SEVEN_CHOOSE_ACTION || blockInteraction == true)
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
               
                if (phrases.Count > 6)
                {
                    string lang = LanguageToString();
                    Debug.LogError("One of the txts has more phrases than it should! Written in " + lang + "\n" + dirtyTexts[0] + dirtyTexts[1] + dirtyTexts[2]);
                }
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

            questions.Add(TODAYS_QUESTION.WHOS_TRAITOR, phrases[1]);
            questions.Add(TODAYS_QUESTION.WHO_LIES, phrases[2]);
            questions.Add(TODAYS_QUESTION.WHO_TO_TRUST, phrases[3]);
            questions.Add(TODAYS_QUESTION.WOULD_YOU_LIE_TO_ME, phrases[4]);
            questions.Add(TODAYS_QUESTION.LAST_DAY, phrases[5]);

            noPower = phrases[6];

            angrySeaWolf[0] = phrases[7];
            angrySeaWolf[1] = phrases[8];
            angrySeaWolf[2] = phrases[9];

            if(phrases.Count > 10)
            {
                string lang = LanguageToString();
                Debug.LogError("General dialogues have more phrases than they should.\nText written in " + lang);
            }
        }
    }

    string LanguageToString()
    {
        switch (language)
        {
            case LANGUAGE.SPANISH: return "spanish";
            case LANGUAGE.CATALAN: return "catalan";
            default: return "english";
        }
    }

    void EndDay()
    {
        day++;
        textDisplay.Clean();
        FadeInOut.SetActive(true);
       // BeginDay();
    }

    public void BeginDay()
    {
        talkingWithN = 0;
        power = 2;
        state = DAY_STATE.ONE_SKULL;

        ChooseTodayQuestion();

        if (day >= 2 && question != TODAYS_QUESTION.LAST_DAY)
        {
            blockInteraction = true;
            eventManager.LaunchEvent();
        }
        else
        {
            EndedEvent();
        }
    }

    public void EndedEvent()
    {
        blockInteraction = false;
    }

    //Sets everything at start
    void SecurityCheck()
    {
        foreach (Character pnj in CharacterGOs)
        {
            pnj.manager = this;
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

    }

    //Stopped talking with someone
    void StoppedTalking()
    {
        talkingWith = TYPES.none;
        talkingWith_name = "No one";
        textDisplay.Clean();
    }

    void MakeAnswer()
    {
        if (talkingWith != TYPES.none && state == DAY_STATE.EIGHT_ANSWER)
        {
            Character pnj = characters[talkingWith];

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

    public void ExitCharacter()
    {
        if (state == DAY_STATE.EIGHT_ANSWER && textDisplay.working == false)
        {
            talkingWith = TYPES.none;
            talkingWithN++;
            if (talkingWithN >= characters.Count)
            {
                //TODO
            }
            else
            {
                state = DAY_STATE.FIVE_DOOR;
            }
            textDisplay.Clean();
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
 