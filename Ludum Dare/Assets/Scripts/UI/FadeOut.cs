using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    public float fadeOutTime = 1.0f;
    public float fadeDelay = 0.0f;

    float currentDelay = 0.0f;
    float currentTime = 0.0f;
    public bool fade = false;
    float maxAlpha;

	// Update is called once per frame
	void Update ()
    {
        if (fade == true)
        {
            currentTime += Time.deltaTime;
            maxAlpha = 0.0f;

            Stack<GameObject> childs = new Stack<GameObject>();
            childs.Push(gameObject);
            while (childs.Count > 0)
            {
                GameObject go = childs.Pop();
                for (int n = 0; n < go.transform.childCount; n++)
                {
                    childs.Push(go.transform.GetChild(n).gameObject);
                }

                CanvasGroup image = go.GetComponent<CanvasGroup>();
                if (image != null)
                {
                    image.alpha = (fadeOutTime - currentTime) / fadeOutTime;
                    maxAlpha = Mathf.Max(maxAlpha, image.alpha);
                }
                else
                {
                    SpriteRenderer rend = go.GetComponent<SpriteRenderer>();
                    if (rend != null)
                    {
                        rend.material.color = new Color(rend.material.color.r, rend.material.color.g, rend.material.color.b, (fadeOutTime - currentTime) / fadeOutTime);
                        maxAlpha = Mathf.Max(maxAlpha, rend.material.color.a);
                    }
                }
            }

            if (maxAlpha <= 0.0f)
            {
                fade = false;
                gameObject.SetActive(false);
            }

        }
    }
}
