using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImTheDoor : MonoBehaviour {

    public TextManager manager;
	void ClickDown(int button)
    {
        if (button == 0)
        {
            manager.ClickedOnDoor();
        }
    }
}
