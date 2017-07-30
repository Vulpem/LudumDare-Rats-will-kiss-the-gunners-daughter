using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transporter : MonoBehaviour {

    Vector3 speed;
    Color changeColor;
    Vector3 changeScale;

    Vector3 position;
    Color color;
    Vector3 scale;

    float duration;
    float counter;

    bool working = false;
    Renderer rend;
	
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

	void Update ()
    {
		if(working)
        {
            transform.position += speed * Time.deltaTime;
            rend.material.color += changeColor * Time.deltaTime;
            transform.localScale += changeScale * Time.deltaTime;
            counter += Time.deltaTime;
            if(counter >= duration)
            {
                working = false;
                transform.localScale = scale;
                transform.position = position;
                rend.material.color = color;
            }
        }

	}

    public void Transport(float time, Vector3 pos, Vector3 scal, Color col)
    {
        position = pos;
        color = col;
        scale = scal;
        duration = time;
        counter = 0.0f;
        working = true;

        speed = (pos - transform.position) / time;
        changeColor = (col - GetComponent<Renderer>().material.color) / time;
        changeScale = (scal - transform.localScale) / time;

    }
}
