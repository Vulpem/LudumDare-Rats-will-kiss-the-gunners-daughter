using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour {

    [Header(" -- Database variables --")]
    public string eventsPath;
    public TextAsset[] events;
    public TextAsset[][] answers;
 //   public TextAsset[] positiveAnswers;
  //  public TextAsset[] negativeAnswers;
  //  public TextAsset[] neutralAnswers;

    [Header(" -- UI variables --")]
    public Text eventText;
    public Text[] answersText;

    List<int> launchedEvents;
    public int[] answerOrders;

    // Use this for initialization
    void Start ()
    {
        //Initializing arrays and lists
        answers = new TextAsset[3][];
        launchedEvents = new List<int>();
        answerOrders = new int[3];

        events = Resources.LoadAll<TextAsset>(eventsPath + "/EventExpl");
        answers[0] = Resources.LoadAll<TextAsset>(eventsPath + "/PositiveAsw");
        answers[1] = Resources.LoadAll<TextAsset>(eventsPath + "/NegativeAsw");
        answers[2] = Resources.LoadAll<TextAsset>(eventsPath + "/NeutralAsw");

        LaunchEvent();

    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    void LaunchEvent()
    {
        //Generating random content
        int eventIndex = ChooseNewEvent();
        GenerateAnswerOrders();

        //Filling UI texts
        eventText.text = events[eventIndex].text;
        
        for (int i = 0; i < 3; i++)
        {
            answersText[i].text = answers[answerOrders[i]][eventIndex].text;
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
            answerOrders[i] = list[index];
            list.RemoveAt(index);       
        }
    }

    public void OnSelectAnswer(int answer)
    {

    }
}
