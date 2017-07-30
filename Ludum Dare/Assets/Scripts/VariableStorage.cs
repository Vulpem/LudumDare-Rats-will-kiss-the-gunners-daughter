using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VariableStorage : MonoBehaviour {

    static public Dictionary<string, int> ints;
    static public Dictionary<string, string> strings;

    static bool generated = false;

    void Awake ()
    {
        if (generated == false)
        {
            ints = new Dictionary<string, int>();
            strings = new Dictionary<string, string>();
            generated = true;
        }

    }
 
}
