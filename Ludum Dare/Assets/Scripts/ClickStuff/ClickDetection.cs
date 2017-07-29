using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDetection : MonoBehaviour {

    Vector3 start;
    Vector3 end;
    bool hitSomething;

    public enum MouseButtons
    {
        MOUSE_BUTTON_LEFT = 0,
        MOUSE_BUTTON_RIGHT = 1,
        MOUSE_BUTTON_WHEEL = 2
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        hitSomething = false;
		for(int n = 0; n < 3; n++)
        {
            if (Input.GetMouseButtonDown(n))
            {
                SendRay("ClickDown", n);
            }
            if (Input.GetMouseButtonUp(n))
            {
                SendRay("ClickUp", n);
            }
        }
    }

    void SendRay (string function, int button)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        start = ray.origin;

        if (Physics.Raycast(ray, out hit, 1000.0f))
        {
            hit.collider.gameObject.SendMessage(function, button);
            hitSomething = true;
            end = hit.point;
        }
        else
        {
            end = ray.origin + ray.direction.normalized * 100;
        }
    }
}
