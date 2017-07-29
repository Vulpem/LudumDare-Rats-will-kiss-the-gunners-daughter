using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpeechBubble
{
    string name;
    public string Text;
}

public class Character : MonoBehaviour {

    public string descrpition;
    public SpeechBubble[] bubbles;

    [HideInInspector]
    public TextManager manager;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ClickUp(int buttonN)
    {
        manager.ClickedOnMe(gameObject, buttonN);
    }

    void SetNames()
    {
      
    }
}
