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

        events = Resources.LoadAll<TextAsset>(eventsPath + "/EventExpl");
        answers[0] = Resources.LoadAll<TextAsset>(eventsPath + "/PositiveAsw");
        answers[1] = Resources.LoadAll<TextAsset>(eventsPath + "/NegativeAsw");
        answers[2] = Resources.LoadAll<TextAsset>(eventsPath + "/NeutralAsw");

        responses[0] = Resources.LoadAll<TextAsset>(eventsPath + "/PositiveResp");
        responses[1] = Resources.LoadAll<TextAsset>(eventsPath + "/NegativeResp");
        responses[2] = Resources.LoadAll<TextAsset>(eventsPath + "/NeutralResp");

      //  LaunchEvent();

    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    public void LaunchEvent()
    {
        //Generating random content
        eventIndex = ChooseNewEvent();
        GenerateAnswerOrders();

        //Activating UI texts
        panel.SetActive(true);
        for (int i = 0; i < answersText.Length; i++)
        {
            answersText[i].transform.parent.gameObject.SetActive(true);
        }
        eventText.gameObject.SetActive(true);
         
        //Filling UI texts
        eventText.text = events[eventIndex].text;       
        for (int i = 0; i < 3; i++)
        {
            answersText[i].text = answers[answersOrder[i]][eventIndex].text;
        }
        
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
            answersText[i].transform.parent.gameObject.SetActive(false);
        }
        eventText.gameObject.SetActive(false);

        eventResponse.text = responses[answersOrder[answer]][eventIndex].text;
        eventResponse.gameObject.SetActive(true);

        if (answersOrder[answer] == 0)
            textManager.power++;
        else if (answersOrder[answer] == 1)
            textManager.power--;

        finishEventButton.SetActive(true);
    }

    public void OnFinishEvent()
    {
        finishEventButton.SetActive(false);
        eventResponse.gameObject.SetActive(false);
        panel.SetActive(false);
        textManager.blockInteraction = false;
    }
}
