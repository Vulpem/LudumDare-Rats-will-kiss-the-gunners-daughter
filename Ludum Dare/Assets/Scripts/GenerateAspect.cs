using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateAspect : MonoBehaviour {

    public SpriteRenderer head;
    public SpriteRenderer body;
    public SpriteRenderer face;

    public AspectDatabase database;

    // Use this for initialization
    void Start ()
    {
        head.sprite = database.GetHead();
        body.sprite = database.GetBody();
        face.sprite = database.GetFace();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
