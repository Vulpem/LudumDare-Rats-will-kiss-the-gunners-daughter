using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Name
{
    public string name;
    [HideInInspector]
    public bool taken = false;
}

public class NameBank : MonoBehaviour {

    public Name[] names;

    string GetName()
    {
        int n = Random.Range(0, names.Length);
        while(names[n].taken == true)
        {
            n++;
            if(n >= names.Length) { n = 0; }
        }

        names[n].taken = true;
        return names[n].name;
    }
}
