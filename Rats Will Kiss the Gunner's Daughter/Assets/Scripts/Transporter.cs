using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transporter : MonoBehaviour
{

    Vector3 speed;
    Color changeColor;
    Vector3 changeScale;

    Vector3 position;
    Color color;
    Vector3 scale;

    float duration;
    float counter;

    Color rendererColor;

    bool working = false;

    void Start()
    {
        rendererColor = Color.white;
    }

    void Update()
    {
        if (working)
        {
            transform.position += speed * Time.deltaTime;
            rendererColor += changeColor * Time.deltaTime;
            /////
            transform.localScale += changeScale * Time.deltaTime;
            counter += Time.deltaTime;
            if (counter >= duration)
            {
                working = false;
                transform.localScale = scale;
                transform.position = position;
                rendererColor = color;
            }
            SetColor(rendererColor);
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
        changeColor = (col - rendererColor) / time;
        changeScale = (scal - transform.localScale) / time;

    }

    public void SetColor(Color col)
    {
        Stack<GameObject> childs = new Stack<GameObject>();
        childs.Push(gameObject);

        while (childs.Count > 0)
        {
            GameObject toTint = childs.Pop();

            for (int n = 0; n < toTint.transform.childCount; n++)
            {
                childs.Push(toTint.transform.GetChild(n).gameObject);
            }

            Renderer rend = toTint.GetComponent<Renderer>();
            if (rend != null)
            {
                Color c = new Color(col.r, col.g, col.b, rend.material.color.a);
                rend.material.color = c;
            }
        }
    }

    public void Transport(float time, Vector3 pos)
    {
        Transport(time, pos, transform.localScale, color);
    }

    public bool isWorking()
    {
        return working;
    }
}
