using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDetection : MonoBehaviour {

    Vector3 start;
    Vector3 end;
    bool hitSomething;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        hitSomething = false;
		if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            start = ray.origin;
            end = ray.origin + ray.direction.normalized * 100;

            if(Physics.Raycast(ray, out hit, 1000.0f))
            {
                hit.collider.gameObject.SendMessage("Click");
                hitSomething = true;
            }
        }
	}

    void OnDrawGizmos()
    {
        if (hitSomething)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.white;
        }
        Gizmos.DrawLine(start, end);
    }
}
