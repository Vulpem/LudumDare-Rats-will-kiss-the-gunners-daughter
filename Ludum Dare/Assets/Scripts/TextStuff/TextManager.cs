using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum LANGUAGE
{
    ENGLISH = 0,
    SPANISH = 1,
    CATALAN = 2
}

public enum TODAYS_QUESTION
{
    WHOS_TRAITOR = 0,
    WHO_LIES,
    WHO_TO_TRUST,
    WOULD_YOU_LIE_TO_ME,
    MOST_LOYAL,
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
    NINE_NIGHT_EVENT,
    NIGHT,
    KILL
}

public class TextManager : MonoBehaviour {

    bool endGame = false;
    bool loadMenu = false;
    float countEnd = 0.0f;
    public GameObject WinScreen;
	public GameObject LoseScreen;
    public Text WinScreenText;
    Dictionary<TODAYS_QUESTION, string> questions;
    Dictionary<TODAYS_QUESTION, bool> questionsAsked;
    string[] angrySeaWolf;
    [HideInInspector]
    public string winText;
    [HideInInspector]
    public string lostText;

    public MusicManager music;
    [Header(" --- KillScreen --- ")]
    public GameObject KillPanel;
    public GameObject KillText;
    public GameObject[] KillCharacterPositions;
    public GameObject[] KillCharacterNames;
    bool KillBool = false;

    LANGUAGE language;
    public DAY_STATE state;

    [HideInInspector]
    public string txtRoute;

    PLAYER_ACTIONS actionMade = 0;

    [Header("Things that need to be set up Manually")]
    public GameObject dismissPNJ;
    public GameObject dismissNight;
    public TutorialManager cardManager;
    public CardsManager skullManager;
    public Animator shinySkull;
    public Animator shinyDoor;
    public Animator shinyPapyrus;
    [Tooltip("List of characters")]
    public Character[] CharacterGOs;
    [HideInInspector]
    public SortedDictionary<TYPES, Character> characters;

    public MakeTextAppear questionDisplay;
    public MakeTextAppear textDisplay;
    public Button[] actions;

    public EventManager eventManager;
    public FadeManager fade;

    public GameObject talkingPos;
    public GameObject restingPos;

    [Header("Debug Info")]
    [Range(0,4)]
    public int talkingWithN;
    public TYPES talkingWith;
    public string talkingWith_name;
    [Range(0, 3)]
    public int power = 2;
    public TODAYS_QUESTION question;
    [Range(0,5)]
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

        actionMade = 0;
        talkingWith = TYPES.none;
        SecurityCheck();

        GenerateCharacters();
        LoadDialogues();

        blockInteraction = true;
        BeginDay();
        day = 1;
        KillBool = false;

        KillPanel.SetActive(false);

        foreach (Character pnj in CharacterGOs)
        {
                pnj.SetActive(true);
                pnj.transform.position = talkingPos.transform.position;
                pnj.transform.localScale = pnj.scale;
            pnj.GetComponent<FadeManager>().SetAlpha(0.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(endGame)
        {
            KillPanel.SetActive(false);
            KillText.SetActive(false);

            foreach (GameObject name in KillCharacterNames)
            {
                name.SetActive(false);
            }
            foreach (Character pnj in CharacterGOs)
            {
                pnj.SetActive(false);
            }

            if (WinScreenText.GetComponent<MakeTextAppear>().working == false)
            {
                countEnd += Time.deltaTime;
                if (countEnd >= 0.75f && Input.GetMouseButtonDown(0))
                {
                    countEnd += Time.deltaTime;
                    WinScreen.GetComponent<FadeManager>().Out();
                    loadMenu = true;
                }
            }

            if (loadMenu && WinScreen.GetComponent<FadeManager>().Working() == false)
            {
                SceneManager.LoadScene(0, LoadSceneMode.Single);
            }
        }

        if(state == DAY_STATE.NIGHT)
        {
            if(fade.Working() == false && blockInteraction == false)
            {
                EndedEvent();
                fade.Out();
                music.ChangeMusicVolume(1.0f);
                music.RestartMusic();
                BeginDay();
            }
        }

        if(state == DAY_STATE.KILL)
        {
            KillUpdate();
        }

        if(shinySkull != null)
        {
            if (state == DAY_STATE.ONE_SKULL && blockInteraction == false && fade.Working() == false)
            {
                shinySkull.gameObject.SetActive(true);
            }
            else
            {
                shinySkull.gameObject.SetActive(false);
            }
        }

        if (shinyPapyrus != null)
        {
            if (state == DAY_STATE.THREE_NOTE && blockInteraction == false && fade.Working() == false)
            {
                shinyPapyrus.gameObject.SetActive(true);
            }
            else
            {
                shinyPapyrus.gameObject.SetActive(false);
            }
        }

        if (shinyDoor != null)
        {
            if (state == DAY_STATE.FIVE_DOOR && blockInteraction == false && fade.Working() == false)
            {
                shinyDoor.gameObject.SetActive(true);
            }
            else
            {
                shinyDoor.gameObject.SetActive(false);
            }
        }

        if (blockInteraction == false)
        {
            foreach (Character pnj in CharacterGOs)
            {
                if (talkingWith != pnj.type)
                {
                    pnj.SetActive(false);
                }
                else
                {
                    pnj.SetActive(true);
                    pnj.transform.position = talkingPos.transform.position;
                    pnj.transform.localScale = pnj.scale;
                }
            }
        }

        if(state == DAY_STATE.SIX_TALKING && textDisplay.working == false)
        {
            state = DAY_STATE.SEVEN_CHOOSE_ACTION;
        }

        if(state == DAY_STATE.EIGHT_ANSWER && textDisplay.working == false && blockInteraction == false)
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
            questionDisplay.Skip();
        }

        HideActions();
    }

    public void StartNightEvent()
    {
        if (state == DAY_STATE.EIGHT_ANSWER && textDisplay.working == false)
        {
            talkingWith = TYPES.none;
            talkingWithN++;
            state = DAY_STATE.NINE_NIGHT_EVENT;
            textDisplay.Clean();
            questionDisplay.Clean();
            fade.In();
            music.ChangeMusicVolume(0.0f);
            EndDay();
        }

        //TODO
    }

    public void ClickedOnSkull()
    {
        if(state >= DAY_STATE.ONE_SKULL && state != DAY_STATE.NINE_NIGHT_EVENT && blockInteraction == false)
        {
            music.PlaySound(SOUNDS.paper);
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
            music.PlaySound(SOUNDS.paper);
            blockInteraction = false;
            skullManager.OnExitButton();
            if (state == DAY_STATE.TWO_SKULL_NOTE)
            {
                if (day == 1)
                {
                    state = DAY_STATE.THREE_NOTE;
                }
                else
                {
                    questionDisplay.Begin(questions[question]);
                    state = DAY_STATE.FIVE_DOOR;
                }
            }
        }
    }

    public void ClickedOnParchement()
    {
        if(state >= DAY_STATE.THREE_NOTE && state != DAY_STATE.NINE_NIGHT_EVENT && blockInteraction == false)
        {
            music.PlaySound(SOUNDS.paper);
            blockInteraction = true;
            cardManager.OpenScroll();
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
            music.PlaySound(SOUNDS.paper);
            blockInteraction = false;
            cardManager.CloseScroll();
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
            music.PlaySound(SOUNDS.door);
            talkingWith = CharacterGOs[talkingWithN].type;
            state = DAY_STATE.SIX_TALKING;
            if (characters[talkingWith].angryCount < 2)
            {
                CreateText(characters[talkingWith].name, characters[talkingWith].bubbles[question].text);
            }
            else
            {
                CreateText(characters[talkingWith].name, "...");
            }
        }
    }

    public void MakeAction(int act)
    {
        if(state == DAY_STATE.SEVEN_CHOOSE_ACTION && blockInteraction == false)
        {
            if(act != (int)PLAYER_ACTIONS.PEACEFUL && power <= 0)
            {
                return;
            }
            state = DAY_STATE.EIGHT_ANSWER;
            actionMade = (PLAYER_ACTIONS)act;
            MakeAnswer();
        }
    }

    void ChooseTodayQuestion()
    {
        /*
        if (day < 6)
        {
            int n = UnityEngine.Random.Range(0, 6);
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
        }*/
        question = (TODAYS_QUESTION)(day - 1);
    }

    void HideActions()
    {
        if (state != DAY_STATE.SEVEN_CHOOSE_ACTION || blockInteraction == true)
        {
            foreach (Button c in actions)
            {
                c.gameObject.SetActive(false);
                c.GetComponent<Image>().color = Color.white;
            }
        }
        else
        {
            if (power > 0)
            {
                foreach (Button c in actions)
                {
                    c.gameObject.SetActive(true);
                    c.GetComponent<Image>().color = Color.white;
                }
            }
            else
            {
                int n = 0;
                foreach (Button c in actions)
                {
                    c.gameObject.SetActive(true);
                    if (n != 0)
                    {
                        c.GetComponent<Image>().color = new Color(0.3f,0.3f,0.3f);
                    }
                    n++;
                }
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
        bank[(int)TODAYS_QUESTION.MOST_LOYAL] = Resources.LoadAll<TextAsset>(txtRoute + "Dialogue/most_loyal");

        int n = 0;
        foreach (TextAsset[] dayBank in bank)
        {
            foreach(TextAsset file in dayBank)
            {
                List<string> phrases = CleanPhrases(file);

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
                    Debug.LogError("One of the txts has more phrases than it should! Written in " + lang);
                }
            }
            n++;
        }

        {
            angrySeaWolf = new string[3];

            TextAsset general = Resources.Load<TextAsset>(txtRoute + "Dialogue/GeneralDialogue");
            List<string> phrases = CleanPhrases(general);

            questions = new Dictionary<TODAYS_QUESTION, string>();

            questions.Add(TODAYS_QUESTION.WHOS_TRAITOR, phrases[1]);
            questions.Add(TODAYS_QUESTION.WHO_LIES, phrases[2]);
            questions.Add(TODAYS_QUESTION.WHO_TO_TRUST, phrases[3]);
            questions.Add(TODAYS_QUESTION.WOULD_YOU_LIE_TO_ME, phrases[4]);
            questions.Add(TODAYS_QUESTION.MOST_LOYAL, phrases[5]);
            questions.Add(TODAYS_QUESTION.LAST_DAY, phrases[6]);

            angrySeaWolf[0] = phrases[8];
            angrySeaWolf[1] = phrases[9];
            angrySeaWolf[2] = phrases[10];

            //cardManager.tutorial1.text = phrases[11];
           // cardManager.tutorial2.text = phrases[12];

            winText = phrases[13];
            lostText = phrases[14];


            if (phrases.Count > 15)
            {
                string lang = LanguageToString();
                Debug.LogError("General dialogues have more phrases than they should.\nText written in " + lang);
            }
        }
    }

    List<string> CleanPhrases(TextAsset bank)
    {
        string[] dirtyTexts = bank.text.Split('\n');
        List<string> phrases = new List<string>();

        string newPhrase = "";
        bool toAdd = false;

        foreach (string str in dirtyTexts)
        {
            if (str[0] != '#' && ((str.Length >= 1 && str[0] != '\n' && str[0] != '\r') || toAdd) )
            {
                newPhrase += str;
                toAdd = true;
            }
            else
            {
                int n = 1;
                for(; n < newPhrase.Length - 1; n++)
                {
                    if((newPhrase[newPhrase.Length - n] == '\n' || newPhrase[newPhrase.Length - n] == '\r') && (newPhrase[newPhrase.Length - n - 1] != '\n' && newPhrase[newPhrase.Length - n - 1] != '\r'))
                    {
                        break;
                    }
                }

                if (toAdd)
                {
                    phrases.Add(newPhrase.Substring(0, newPhrase.Length - n));
                }
                newPhrase = "";
                toAdd = false;
            }
           
        }


        return phrases;
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
        textDisplay.Clean();
        fade.In();
        music.ChangeMusicVolume(0.0f);
        blockInteraction = true;
        state = DAY_STATE.NIGHT;
        if (day < 5)
        {
            eventManager.LaunchEvent();
        }
        else
        {
            BeginKill();
        }

    }

    void BeginKill()
    {
        state = DAY_STATE.KILL;
    }

    void KillUpdate()
    {
        int n = 0;
        if (fade.Working() == false && KillBool == false)
        {
            KillBool = true;
            KillPanel.GetComponent<FadeManager>().SetAlpha(0.0f);
            KillPanel.GetComponent<FadeManager>().In();

            KillText.GetComponent<MakeTextAppear>().Begin(questions[TODAYS_QUESTION.LAST_DAY]);
            
            foreach(GameObject name in KillCharacterNames)
            {
                name.GetComponent<Text>().text = CharacterGOs[n].name;
                n++;
            }

            foreach(Character pnj in CharacterGOs)
            {
                pnj.GetComponent<FadeManager>().SetAlpha(0.0f);
                pnj.GetComponent<FadeManager>().In();
            }
        }
        n = 0;
        if(KillBool)
        {
            foreach (Character pnj in CharacterGOs)
            {
                pnj.transform.position = KillCharacterPositions[n].transform.position;
                pnj.transform.localScale = KillCharacterPositions[n].transform.localScale;
                n++;
            }
        }
        else
        {
            foreach (Character pnj in CharacterGOs)
            {
                pnj.transform.position = pnj.originalPos;
            }
        }

    }

    public void BeginDay()
    {
        day++;
        fade.SetAlpha(1.0f);
        fade.Out();
        music.ChangeMusicVolume(1.0f);
        music.RestartMusic();
        talkingWithN = 0;
        state = DAY_STATE.ONE_SKULL;
        music.PlaySound(SOUNDS.bell);
        foreach(Character pnj in CharacterGOs)
        {
            pnj.GetComponent<FadeManager>().SetAlpha(0.0f);
        }

        ChooseTodayQuestion();

        EndedEvent();
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

    //A character has recieved a click
    public void ClickedOnMe(GameObject go, int button)
    {
        if(state == DAY_STATE.KILL)
        {
            Character pnj = go.GetComponent<Character>();
            if (pnj != null)
            {
                foreach (Character _pnj in CharacterGOs)
                {
                    _pnj.GetComponent<FadeManager>().SetAlpha(0.0f);
                }

                if (pnj.type == TYPES.rioter)
                {
                    Win();
                }
                else
                {
                    Loose();
                }
            }
        }
    }

    void Win()
    {
        music.ChangeMusicVolume(1.0f);
        music.RestartMusic();
        WinScreen.SetActive(true);
        //WinScreenText.gameObject.SetActive(true);
        //WinScreenText.GetComponent<MakeTextAppear>().Begin(winText);
        endGame = true;
    }

    void Loose()
    {
        music.ChangeMusicVolume(1.0f);
        music.RestartMusic();
        LoseScreen.SetActive(true);
        //WinScreenText.gameObject.SetActive(true);
        //WinScreenText.GetComponent<MakeTextAppear>().Begin(lostText);
        endGame = true;
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
                music.PlaySound(SOUNDS.coin);
            }

            if(pnj.type == TYPES.sea_wolf && pnj.angryCount >= 2)
            {
                CreateText(pnj.name, angrySeaWolf[2]);
                return;
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
            if (question == TODAYS_QUESTION.MOST_LOYAL && talkingWithN == 4)
            {
                characters[talkingWith].gameObject.SetActive(false);
                characters[talkingWith].gameObject.GetComponent<FadeManager>().SetAlpha(0.0f);
                blockInteraction = true;
            }
            talkingWith = TYPES.none;
            talkingWithN++;
            state = DAY_STATE.FIVE_DOOR;
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

        tmp = tmp.Replace("<rioter>", "<color=#51272f>" + characters[TYPES.rioter].name + "</color>");
        tmp = tmp.Replace("<brute>", "<color=#51272f>" + characters[TYPES.brute].name + "</color>");
        tmp = tmp.Replace("<sea_wolf>", "<color=#51272f>" + characters[TYPES.sea_wolf].name + "</color>");
        tmp = tmp.Replace("<sea wolf>", "<color=#51272f>" + characters[TYPES.sea_wolf].name + "</color>");
        tmp = tmp.Replace("<stingy>", "<color=#51272f>" + characters[TYPES.stingy].name + "</color>");
        tmp = tmp.Replace("<comrad>", "<color=#51272f>" + characters[TYPES.comrad].name + "</color>");
        tmp = tmp.Replace("<comrade>", "<color=#51272f>" + characters[TYPES.comrad].name + "</color>");

        textDisplay.Begin(tmp);
    }
		
}
 