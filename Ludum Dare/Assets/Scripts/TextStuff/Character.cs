using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public TYPES type;
    //The name of the Character is the name of the GameObject
    public bool active = true;
    public bool activeLastFrame = false;
    public bool doneForToday = false;

    public int angryCount = 0;

    public static string gettingAngry;
    public static string gotAngry;
    public static string Angry;

    Transporter transporter;

    [Header(" --- Speech bubbles --- ")]
    public Dictionary<TODAYS_QUESTION, SpeechBubble> bubbles;

    [HideInInspector]
    public TextManager manager;
    [HideInInspector]
    public int characterN = -1;

    Vector3 scale;

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

    void Update()
    {
        if (active != activeLastFrame)
        {
            if (active == false)
            {
                transporter.Transport(1.0f, transform.position, scale * 0.6f, new Color(0.6f, 0.6f, 0.6f));
            }
            else
            {
                transporter.Transport(1.0f, transform.position, scale, new Color(1.0f, 1.0f, 1.0f));
            }
            activeLastFrame = active;
        }
    }

    public void ClickUp(int buttonN)
    {
        manager.ClickedOnMe(gameObject, buttonN);
    }
}
