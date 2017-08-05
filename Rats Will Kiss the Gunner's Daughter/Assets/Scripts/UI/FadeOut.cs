using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    public float fadeOutTime = 1.0f;

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
                Text text = go.GetComponent<Text>();
                for (int n = 0; n < go.transform.childCount; n++)
                {
                    childs.Push(go.transform.GetChild(n).gameObject);
                }
                CanvasGroup group = go.GetComponent<CanvasGroup>();
                if (group != null)
                {
                    group.alpha = (fadeOutTime - currentTime) / fadeOutTime;
                }
                Image image = go.GetComponent<Image>();
                if (image != null)
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, (fadeOutTime - currentTime) / fadeOutTime);
                    maxAlpha = Mathf.Max(maxAlpha, image.color.a);
                }
                else if (text != null)
                {
                    text.color = new Color(text.color.r, text.color.g, text.color.b, (fadeOutTime - currentTime) / fadeOutTime);
                    maxAlpha = Mathf.Max(maxAlpha, text.color.a);
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
                currentTime = 0.0f;
                gameObject.SetActive(false);
            }

        }
    }
}
