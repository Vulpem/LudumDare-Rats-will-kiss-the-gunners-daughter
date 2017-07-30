using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MakeTextAppear : MonoBehaviour {

    public string text;
    int currentPosition = 0;
    public float delayWithinLetters = 0.02f;
    public bool working = false;

    public Text UIText;

	// Use this for initialization
	void Start () {
        UIText = GetComponent<Text>();
        working = false;
        UIText.text = "";
	}

    public void Begin(string _text)
    {
        if (working == false)
        {
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
                UIText.text += text[currentPosition];
                currentPosition++;
            }

            yield return new WaitForSeconds(delayWithinLetters);
        }
    }

    public void Skip()
    {
        UIText.text = text;
        working = false;
    }

    public void Clean()
    {
        Skip();
        UIText.text = "";
    }
	
	// Update is called once per frame
	void Update () {

	}
}
