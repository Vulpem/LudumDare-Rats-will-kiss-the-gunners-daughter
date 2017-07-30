using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHover : MonoBehaviour {

    bool moving = false;
    bool movingOut = false;
    Vector2 originalSize;

    public Vector2 hoverSize;
    public float movementTime = 0.5f;
    float time = 0.0f;

	// Use this for initialization
	void Start ()
    {
        originalSize = gameObject.GetComponent<RectTransform>().sizeDelta;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (moving == true)
        {
            time += Time.deltaTime;
            Vector2 newSize;
            if (movingOut == true)
            {
                newSize = Vector2.Lerp(originalSize, hoverSize, time / movementTime);
            }
            else
            {
                newSize = Vector3.Lerp(hoverSize, originalSize, time / movementTime);
            }
            if (time >= movementTime)
            {
                moving = false;
                newSize = movingOut ? hoverSize : originalSize;
            }
            gameObject.GetComponent<RectTransform>().sizeDelta = newSize;
        }
	}

    public void OnHover()
    {
        moving = true;
        movingOut = true;
        time = 0.0f;
    }

    public void OnExit()
    {
        moving = true;
        movingOut = false;
        time = 0.0f;
    }
}
