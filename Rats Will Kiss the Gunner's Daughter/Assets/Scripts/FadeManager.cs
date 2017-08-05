using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{

    FadeIn fadeIn;
    FadeOut fadeOut;

    public GameObject linkAlpha;

    void Update()
    {
        if (linkAlpha != null)
        {
            float Alpha = 1.0f;

            {
                Image image = linkAlpha.GetComponent<Image>();
                Text text = linkAlpha.GetComponent<Text>();
                SpriteRenderer rend = linkAlpha.GetComponent<SpriteRenderer>();
                if (image != null)
                {
                    Alpha = image.color.a;
                }
                else if (text != null)
                {
                    Alpha = text.color.a;
                }
                else if (rend != null)
                {
                    Alpha = rend.material.color.a;
                }
            }

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

                Image image = go.GetComponent<Image>();
                Text text = go.GetComponent<Text>();
                if (group != null)
                {
                    group.alpha = Alpha;
                }
                if (image != null)
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, Alpha);
                }
                else if (text != null)
                {
                    text.color = new Color(text.color.r, text.color.g, text.color.b, Alpha);
                }
                else
                {
                    SpriteRenderer rend = go.GetComponent<SpriteRenderer>();
                    if (rend != null)
                    {
                        rend.material.color = new Color(rend.material.color.r, rend.material.color.g, rend.material.color.b, Alpha);
                    }
                }
            }
        }
    }

    void Awake()
    {
        fadeIn = GetComponent<FadeIn>();
        if (fadeIn == null)
        {
            fadeIn = gameObject.AddComponent<FadeIn>();
        }

        fadeOut = GetComponent<FadeOut>();
        if (fadeOut == null)
        {
            fadeOut = gameObject.AddComponent<FadeOut>();
        }
    }

    public bool Working()
    {
        if (fadeIn == null || fadeOut == null)
        {
            fadeIn = GetComponent<FadeIn>();
            fadeOut = GetComponent<FadeOut>();
        }

        return (fadeIn.fade == true || fadeOut.fade == true);
    }

    public void In()
    {
            gameObject.SetActive(true);
            fadeIn.fade = true;
            fadeOut.fade = false;
    }

    public void Out()
    {
            fadeOut.fade = true;
            fadeIn.fade = false;
    }

    public void SetAlpha(float Alpha)
    {
        if (fadeIn != null)
        {
            fadeIn.fade = false;
        }
        if (fadeOut != null)
        {
            fadeOut.fade = false;
        }
        if (Alpha != 0.0f)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }

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
            Image image = go.GetComponent<Image>();
            Text text = go.GetComponent<Text>();
            if (group != null)
            {
                group.alpha = Alpha;
            }
            if (image != null)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, Alpha);
            }
            else if (text != null)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, Alpha);
            }
            else
            {
                SpriteRenderer rend = go.GetComponent<SpriteRenderer>();
                if (rend != null)
                {
                    rend.material.color = new Color(rend.material.color.r, rend.material.color.g, rend.material.color.b, Alpha);
                }
            }
        }
    }

    public float GetAlpha()
    {
        GameObject go = gameObject;

        CanvasGroup group = go.GetComponent<CanvasGroup>();
        Image image = go.GetComponent<Image>();
        Text text = go.GetComponent<Text>();
        if (group != null)
        {
            return group.alpha;
        }
        if (image != null)
        {
            return image.color.a;
        }
        else if (text != null)
        {
            return text.color.a;
        }
        else
        {
            SpriteRenderer rend = go.GetComponent<SpriteRenderer>();
            if (rend != null)
            {
                return rend.material.color.a;
            }
        }
        return 0.0f;
    }
}
