using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    public float fadeInTime = 2.0f;
    public float fadeDelay = 0.0f;
    float currentTime = 0.0f;
    public bool fade = false;
    public TextManager textManager;
	// Update is called once per frame
	void Update ()
    {
        if (fade == true)
        {
            currentTime += Time.deltaTime;
            CanvasGroup image = gameObject.GetComponent<CanvasGroup>();
            image.alpha = currentTime / fadeInTime;

            if (image.alpha >= 1.0f)
            {
                fade = false;
            }
        }
	}
}
