using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actions : MonoBehaviour {

    [HideInInspector]
    public TextManager manager;
    [Range(0, 4)]
    public int action_n;

    void ClickUp(int button)
    {
        if(button == 0)
        {
            manager.SendMessage("MakeAction", action_n);
        }
    }
}
