using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    public float fadeOutTime = 2.0f;
    public float fadeDelay = 3.0f;

    float currentDelay = 0.0f;
    float currentTime = 0.0f;
    public bool fade = false;

	// Update is called once per frame
	void Update ()
    {
        if (fade == true)
        {
            if (currentDelay < fadeDelay)
                currentDelay += Time.deltaTime;

            if (currentDelay >= fadeDelay)
            {
                currentTime += Time.deltaTime;
                Image image = gameObject.GetComponent<Image>();
                Color imageColor = image.color;
                imageColor.a = (fadeOutTime - currentTime) / fadeOutTime;
                image.color = imageColor;

                if (imageColor.a <= 0.0f)
                {
                    gameObject.SetActive(false);
                }
            }
        }
	}
}
