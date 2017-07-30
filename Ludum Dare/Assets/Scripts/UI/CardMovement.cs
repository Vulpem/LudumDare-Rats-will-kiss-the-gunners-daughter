using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMovement : MonoBehaviour {

    bool moving = false;
    bool movingOut = false;
    Vector3 originalPosition;

    public Vector3 movedPosition;
    public float movementTime = 0.5f;
    float time = 0.0f;

	// Use this for initialization
	void Start ()
    {
        originalPosition = gameObject.GetComponent<RectTransform>().anchoredPosition;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (moving == true)
        {
            time += Time.deltaTime;
            Vector3 newPosition;
            if (movingOut == true)
            {
                newPosition = Vector3.Lerp(originalPosition, movedPosition, time / movementTime);
            }
            else
            {
                newPosition = Vector3.Lerp(movedPosition, originalPosition, time / movementTime);
            }
            if (time >= movementTime)
            {
                moving = false;
                newPosition = movingOut ? movedPosition : originalPosition;
            }
            gameObject.GetComponent<RectTransform>().anchoredPosition = newPosition;
        }
	}

    public void MoveCardUp()
    {
        moving = true;
        movingOut = true;
        time = 0.0f;
    }

    public void MoveCardDown()
    {
        moving = true;
        movingOut = false;
        time = 0.0f;
    }
}
