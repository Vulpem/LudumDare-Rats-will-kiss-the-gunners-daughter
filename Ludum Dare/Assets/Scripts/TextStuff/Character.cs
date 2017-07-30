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
    bool active = true;
    bool updatePos = false;
    public bool doneForToday = false;

    public int angryCount = 0;

    public static string gettingAngry;
    public static string gotAngry;
    public static string Angry;

    [Header(" --- Speech bubbles --- ")]
    public Dictionary<TODAYS_QUESTION, SpeechBubble> bubbles;

    [HideInInspector]
    public TextManager manager;
    [HideInInspector]
    public int characterN = -1;

    Vector3 scale;
    Vector3 originalPos;
    Transporter transporter;

    float timeToMove = 0.0f;
    float counter = 0.0f;

    public Character()
    {
        bubbles = new Dictionary<TODAYS_QUESTION, SpeechBubble>();
    }

    void Start()
    {
        originalPos = transform.position;
        scale = gameObject.transform.localScale;
        transporter = GetComponent<Transporter>();
        if(transporter == null)
        {
            transporter = gameObject.AddComponent<Transporter>();
        }
    }

    void Update()
    {
        RandomMovement();

        if (updatePos)
        {
            UpdatePos();
        }
    }

    void RandomMovement()
    {
        counter += Time.deltaTime;
        if(counter >= timeToMove)
        {
            counter = 0.0f;
            timeToMove = Random.Range(3.0f, 8.0f);

            Vector3 newPos = originalPos;
            newPos.x += Random.Range(-0.35f, 0.35f);
            newPos.y += Random.Range(-0.25f, 0.25f);

            transporter.Transport(Random.Range(0.5f, 1.5f), newPos);
        }
    }

    void UpdatePos()
    {
        if (active == false)
        {
            if (manager.talkingWith != TYPES.none)
            {
                Vector3 dif = originalPos - manager.characters[manager.talkingWith].gameObject.transform.position;
                Vector3 newPos = originalPos + dif / 7.0f;
                newPos.y -= 0.2f;
                transporter.Transport(1.0f, newPos, scale * 0.6f, new Color(0.6f, 0.6f, 0.6f));
            }
            else
            {
                transporter.Transport(1.0f, originalPos, scale * 0.6f, new Color(0.6f, 0.6f, 0.6f));
            }
        }
        else
        {
            transporter.Transport(1.0f, originalPos, scale, new Color(1.0f, 1.0f, 1.0f));
        }
        updatePos = false;
    }

    public void SetActive(bool _active)
    {
        active = _active;
        updatePos = true;
    }

    public bool IsActive()
    {
        return active;
    }

    public void ClickUp(int buttonN)
    {
        manager.ClickedOnMe(gameObject, buttonN);
    }
}
