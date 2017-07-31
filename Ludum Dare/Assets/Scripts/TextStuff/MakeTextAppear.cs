using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MakeTextAppear : MonoBehaviour {

    public GameObject Bubble;
    public string text;
    int currentPosition = 0;
    public float delayWithinLetters = 0.02f;
    public bool working = false;

    public Text UIText;

	// Use this for initialization
	void Awake () {
        UIText = GetComponent<Text>();
        working = false;
        UIText.text = "";
        if (Bubble != null)
        {
            Bubble.SetActive(false);
        }

    }

    public void Begin(string _text)
    {
        if (working == false)
        {
            if (Bubble != null)
            {
                Bubble.SetActive(true);
            }
            text = _text;
            working = true;
            currentPosition = 0;
            UIText.text = "";

            StartCoroutine(UpdateText());
        }
    }

    IEnumerator UpdateText()
    {
        while (working)
        {
            if (currentPosition >= text.Length)
            {
                working = false;
                break;
            }
            else
            {
                if (text[currentPosition] != '<')
                {
                    UIText.text += text[currentPosition];
                    currentPosition++;
                }
                else
                {
                    string toAdd = ""; 
                    while(text[currentPosition] != '>' && currentPosition < text.Length)
                    {
                        toAdd += text[currentPosition];
                        currentPosition++;
                    }
                    toAdd += text[currentPosition];
                    currentPosition++;
                    while (text[currentPosition] != '>' && currentPosition < text.Length)
                    {
                        toAdd += text[currentPosition];
                        currentPosition++;
                    }
                    toAdd += text[currentPosition];
                    currentPosition++;

                    UIText.text += toAdd;
                }
            }

            yield return new WaitForSeconds(delayWithinLetters);
        }
    }

    public void Skip()
    {
        if (working)
        {
            UIText.text = text;
            working = false;
        }
    }

    public void Clean()
    {
        Skip();
        UIText.text = "";
        if (Bubble != null)
        {
            Bubble.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {

	}
}
