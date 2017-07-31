using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour {

    [Header(" -- Database variables --")]
    public string eventsPath;
    public TextAsset[] events;
    public TextAsset[][] answers;
    public TextAsset[][] responses;

    [Header(" -- UI variables --")]
    public Text eventText;
    public Text[] answersText;
    public Text eventResponse;

    List<int> launchedEvents;
    public int[] answersOrder;
    public int eventIndex;

    public GameObject finishEventButton;
    public GameObject panel;

    public TextManager textManager;

    // Use this for initialization
    void Start ()
    {
        //Initializing arrays and lists
        answers = new TextAsset[3][];
        responses = new TextAsset[3][];
        launchedEvents = new List<int>();
        answersOrder = new int[3];

        events = Resources.LoadAll<TextAsset>(textManager.txtRoute + eventsPath + "/EventExpl");
        answers[0] = Resources.LoadAll<TextAsset>(textManager.txtRoute + eventsPath + "/PositiveAsw");
        answers[1] = Resources.LoadAll<TextAsset>(textManager.txtRoute + eventsPath + "/NegativeAsw");
        answers[2] = Resources.LoadAll<TextAsset>(textManager.txtRoute + eventsPath + "/NeutralAsw");

        responses[0] = Resources.LoadAll<TextAsset>(textManager.txtRoute + eventsPath + "/PositiveResp");
        responses[1] = Resources.LoadAll<TextAsset>(textManager.txtRoute + eventsPath + "/NegativeResp");
        responses[2] = Resources.LoadAll<TextAsset>(textManager.txtRoute + eventsPath + "/NeutralResp");

      //  LaunchEvent();

    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    public void LaunchEvent()
    {
        /*
        //Generating random content
        eventIndex = ChooseNewEvent();
        GenerateAnswerOrders();

        //Activating UI texts
        panel.GetComponent<FadeManager>().In();
        for (int i = 0; i < answersText.Length; i++)
        {
            answersText[i].transform.parent.gameObject.GetComponent<FadeManager>().In();
        }
        eventText.gameObject.GetComponent<FadeManager>().In();
         
        //Filling UI texts
        eventText.text = events[eventIndex].text;       
        for (int i = 0; i < 3; i++)
        {
            answersText[i].text = answers[answersOrder[i]][eventIndex].text;
        }*/


        //////  TMP
        OnFinishEvent();
        textManager.power = 2;
        
    }

    int ChooseNewEvent()
    {
        bool eventRepeated = true;
        while (eventRepeated)
        {
            eventRepeated = false;
            int newEvent = Random.Range(0, events.Length);
            for (int i = 0; i < launchedEvents.Count; i++)
            {
                if (launchedEvents[i] == newEvent)
                {
                    eventRepeated = true;
                    break;
                }
            }
            if (eventRepeated == false)
                return newEvent;
        }
        print("Warning: repeating event");
        return 0;
    }

    //0 - positive // 1 - negative // 2 - neutral
    void GenerateAnswerOrders()
    {
        List<int> list = new List<int>(new int[] { 0, 1, 2 });
        for (uint i = 0; i < 3; i++)
        {
            int index = Random.Range(0, list.Count);
            answersOrder[i] = list[index];
            list.RemoveAt(index);       
        }
    }

    public void OnSelectAnswer(int answer)
    {
        for (int i = 0; i < answersText.Length; i++)
        {
            answersText[i].transform.parent.gameObject.GetComponent<FadeManager>().Out();
        }
        eventText.gameObject.GetComponent<FadeManager>().Out();

        eventResponse.text = responses[answersOrder[answer]][eventIndex].text;
        eventResponse.gameObject.GetComponent<FadeManager>().In();

        if (answersOrder[answer] == 0)
            textManager.power = 3;
        else if (answersOrder[answer] == 1)
            textManager.power = 1;
        else
            textManager.power = 2;

        finishEventButton.GetComponent<FadeManager>().In();
    }

    public void OnFinishEvent()
    {
        finishEventButton.GetComponent<FadeManager>().SetAlpha(0.0f);
        eventResponse.GetComponent<FadeManager>().SetAlpha(0.0f);
        panel.GetComponent<FadeManager>().SetAlpha(0.0f);
        textManager.EndedEvent();
    }
}
