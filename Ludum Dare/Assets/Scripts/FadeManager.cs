using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeManager : MonoBehaviour
{

    public FadeIn fadeIn;
    public FadeOut fadeOut;

    void Awake()
    {
        fadeIn = GetComponent<FadeIn>();
        fadeOut = GetComponent<FadeOut>();
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
        if (alpha != 0.0f)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
        CanvasGroup image = gameObject.GetComponent<CanvasGroup>();
        image.alpha = alpha;
    }
}
