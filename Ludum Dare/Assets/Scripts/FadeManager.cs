using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeManager : MonoBehaviour
{

    FadeIn fadeIn;
    FadeOut fadeOut;

    void Awake()
    {
        fadeIn = GetComponent<FadeIn>();
        if(fadeIn == null)
        {
            fadeIn = gameObject.AddComponent<FadeIn>();
        }

        fadeOut = GetComponent<FadeOut>();
        if(fadeOut == null)
        {
            fadeOut = gameObject.AddComponent<FadeOut>();
        }
    }

    public bool Working()
    {
        return (fadeIn.fade || fadeOut.fade);
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

    public void SetAlpha(float alpha)
    {
        if (fadeIn != null)
        {
            fadeIn.fade = false;
        }
        if (fadeOut != null)
        {
            fadeOut.fade = false;
        }
        if (alpha != 0.0f)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
        CanvasGroup image = gameObject.GetComponent<CanvasGroup>();
        if (image != null)
        {
            image.alpha = alpha;
            return;
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

            Renderer rend = go.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = new Color(rend.material.color.r, rend.material.color.g, rend.material.color.b, alpha);
            }
        }        
    }
}
