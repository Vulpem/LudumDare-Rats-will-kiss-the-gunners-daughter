using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateAspect : MonoBehaviour {

    public SpriteRenderer head;
    public SpriteRenderer body;
    public SpriteRenderer face;

    public GameObject bodyObject;
    public GameObject headObject;

    public GameObject pivot;

    public AspectDatabase database;

    // Use this for initialization
    void Start ()
    {
        head.sprite = database.GetHead();
        body.sprite = database.GetBody();
        face.sprite = database.GetFace();

        float bodySize = body.sprite.bounds.size.y;

        Vector3 bodyPosition = bodyObject.transform.position;
        bodyPosition.y = pivot.transform.position.y + bodySize / 2;
        bodyObject.transform.position = bodyPosition;

        Vector3 headPostition = headObject.transform.localPosition;
 
        headPostition.y = database.GetHeadPositionY(body.sprite);
        headObject.transform.localPosition = headPostition;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
