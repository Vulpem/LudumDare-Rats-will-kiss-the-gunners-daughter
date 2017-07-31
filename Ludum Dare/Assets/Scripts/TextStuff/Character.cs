using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PLAYER_ACTIONS
{
    PEACEFUL = 0,
    AGRESSION,
    BRIVE
}

public enum TYPES
{
    rioter,
    brute,
    sea_wolf,
    stingy,
    comrad,
    none
}


[System.Serializable]
public class SpeechBubble
{
    [Header(" --- Start of a Speech Bubble --- ")]
    [TextArea(2, 10)]
    [Tooltip("Text that will appear when this character is chosen")]
    public string text;
    [Tooltip("Text that will appear if tried to ask a second question in a day")]
    public string busy;

    public Dictionary<PLAYER_ACTIONS, string> answers;

    public SpeechBubble ()
    {
        answers = new Dictionary<PLAYER_ACTIONS, string>();
    }
}

[RequireComponent(typeof(Transporter))]
public class Character : MonoBehaviour {

    public Text characterNameUI;
    public TYPES type;

    public int angryCount = 0;

    [HideInInspector]
    public static string gettingAngry;
    [HideInInspector]
    public static string gotAngry;
    [HideInInspector]
    public static string Angry;

    [Header(" --- Speech bubbles --- ")]
    public Dictionary<TODAYS_QUESTION, SpeechBubble> bubbles;

    [HideInInspector]
    public TextManager manager;

    Vector3 scale;
    Transporter transporter;

    public Character()
    {
        bubbles = new Dictionary<TODAYS_QUESTION, SpeechBubble>();
    }

    void Start()
    {
        scale = gameObject.transform.localScale;
        transporter = GetComponent<Transporter>();
        if(transporter == null)
        {
            transporter = gameObject.AddComponent<Transporter>();
        }
    }

    public void SetName(string _name)
    {
        characterNameUI.text = name = _name;
    }

    public void ClickUp(int buttonN)
    {
        manager.ClickedOnMe(gameObject, buttonN);
    }
}
