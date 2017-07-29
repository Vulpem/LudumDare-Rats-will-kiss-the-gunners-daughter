using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClick : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ClickDown(int buttonN)
    {
        gameObject.GetComponent<Renderer>().material.color = new Color(0.3f * buttonN, 0, 0);
    }

    public void ClickUp(int buttonN)
    {
        gameObject.GetComponent<Renderer>().material.color = new Color(0, 0.3f * buttonN, 0);
    }
}
