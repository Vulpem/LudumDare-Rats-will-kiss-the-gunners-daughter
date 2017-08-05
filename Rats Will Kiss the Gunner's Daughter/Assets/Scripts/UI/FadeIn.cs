using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    public float fadeInTime = 1.0f;
    public float fadeDelay = 0.0f;
    float currentTime = 0.0f;
    public bool fade = false;
    float minAlpha = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if (fade == true)
        {
            currentTime += Time.deltaTime;
            minAlpha = 1.0f;

            Stack<GameObject> childs = new Stack<GameObject>();
            childs.Push(gameObject);
            while (childs.Count > 0)
            {
                GameObject go = childs.Pop();
                for (int n = 0; n < go.transform.childCount; n++)
                {
                    childs.Push(go.transform.GetChild(n).gameObject);
                }
                CanvasGroup group = go.GetComponent<CanvasGroup>();
                if (group != null)
                {
                    group.alpha = currentTime / fadeInTime;
                }
                Image image = go.GetComponent<Image>();
                Text text = go.GetComponent<Text>();
                if (image != null)
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, currentTime / fadeInTime);
                    minAlpha = Mathf.Min(minAlpha, image.color.a);
                }
                else if(text != null)
                {
                    text.color = new Color(text.color.r, text.color.g, text.color.b, currentTime / fadeInTime);
                    minAlpha = Mathf.Min(minAlpha, text.color.a);
                }
                else
                {
                    SpriteRenderer rend = go.GetComponent<SpriteRenderer>();
                    if (rend != null)
                    {
                        rend.material.color = new Color(rend.material.color.r, rend.material.color.g, rend.material.color.b, currentTime / fadeInTime);
                        minAlpha = Mathf.Min(minAlpha, rend.material.color.a);
                    }
                }
            }

            if (minAlpha >= 1.0f)
            {
                fade = false;
                currentTime = 0.0f;
            }

        }
    }
}
