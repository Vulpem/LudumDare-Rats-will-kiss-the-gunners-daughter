using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsManager : MonoBehaviour
{
    public TextManager textManager;
    public GameObject cardPanel;
    public Text cardText;
    public string cardsPath;

    public TextAsset[] cards;

	// Use this for initialization
	void Start ()
    {
        cards = Resources.LoadAll<TextAsset>(cardsPath);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void OnClickCard()
    {
        cardText.text = cards[textManager.day - 1].text;
        cardPanel.SetActive(true);
    }

    public void OnExitButton()
    {
        cardPanel.SetActive(false);
    }
}
