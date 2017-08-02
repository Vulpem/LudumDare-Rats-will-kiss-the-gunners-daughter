using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerDisplay : MonoBehaviour {

    int lastPower;
    public TextManager textManager;

	// Use this for initialization
	void Start ()
    {
        lastPower = textManager.power;
        UpdateDisplay(true);
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (textManager.power != lastPower)
        {
            lastPower = textManager.power;
            UpdateDisplay();
        }
	}

    void UpdateDisplay(bool instantDisappear = false)
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (i < textManager.power)
            {
                gameObject.transform.GetChild(i).gameObject.GetComponent<PowerCoin>().Activate();
            }
            else
            {
                if (instantDisappear == false)
                {
                    gameObject.transform.GetChild(i).gameObject.GetComponent<PowerCoin>().Consume();
                }
                else
                {
                    gameObject.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }
}
