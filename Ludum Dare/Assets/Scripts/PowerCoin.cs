using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerCoin : MonoBehaviour {

    public float acceleration_consumed = 1000.0f;
    float acceleration = 0.0f;
    public float speed = 20.0f;
    public float disappearSpeed = 1080.0f;

    bool beingConsumed = false;

    float angle = 0.0f;

	// Use this for initialization
	void Start () {
        speed = 0.0f;
    }
	
	// Update is called once per frame
	void Update ()
    {

        if (beingConsumed)
        {
            gameObject.SetActive(false);
        }
          /*  if (beingConsumed)
        {
            acceleration = acceleration_consumed;
            speed += acceleration * Time.deltaTime;
            angle += speed * Time.deltaTime;
        }
        else
        {
            acceleration = 0.0f;
            speed = 0.0f;
            angle = 0.0f;
        }
      
		transform.localRotation = Quaternion.Euler(0, 0, angle);

        if(beingConsumed && speed > disappearSpeed)
        {
            angle = 0.0f;
            speed = 20.0f;
            speed += Random.Range(-5.0f, 5.0f);
            if (Random.Range(-1.0f, 1.0f) > 0.0f)
            {
                speed *= -1;
            }

            beingConsumed = false;
            gameObject.SetActive(false);
        }*/
    }

    public void Consume()
    {
        beingConsumed = true;
    }

    public void Activate()
    {
        if (gameObject.activeInHierarchy == false || beingConsumed)
        {
            gameObject.SetActive(true);

            beingConsumed = false;
            angle = 0.0f;
            speed = 20.0f;
            speed += Random.Range(-5.0f, 5.0f);
            if (Random.Range(-1.0f, 1.0f) > 0.0f)
            {
                speed *= -1;
            }
        }
    }
}
