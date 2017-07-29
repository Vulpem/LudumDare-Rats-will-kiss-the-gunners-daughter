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
        UpdateDisplay();
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

    void UpdateDisplay()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (i < lastPower)
            {
                gameObject.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
