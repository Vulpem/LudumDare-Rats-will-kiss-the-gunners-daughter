using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Name
{
    public Name (string _name) { name = _name; }
    public string name;
    [HideInInspector]
    public bool taken = false;
}

public class NameBank : MonoBehaviour {

    [HideInInspector]
    public List<Name> names;

    void Awake()
    {
        TextAsset[] textBank = Resources.LoadAll<TextAsset>("NameBank");

        string bank = textBank[0].text;
        string[] tmp = bank.Split('\n');
        names = new List<Name>();

        foreach (string str in tmp)
        {
            names.Add(new Name(str));
        }
    }

    public string GetName()
    {
        int n = Random.Range(0, names.Count);
        while(names[n].taken == true)
        {
            n++;
            if(n >= names.Count) { n = 0; }
        }

        names[n].taken = true;
        return names[n].name;
    }
}
