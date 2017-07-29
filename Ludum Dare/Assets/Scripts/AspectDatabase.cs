﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AspectDatabase : MonoBehaviour {

    public string path;

    public Sprite[] heads;
    public Sprite[] bodies;
    public Sprite[] faces;

    public int[] bodySizes;
    public int[] headPositions;

	// Use this for initialization
	void Awake ()
    {
        heads = Resources.LoadAll<Sprite>(path + "/Head");
        bodies = Resources.LoadAll<Sprite>(path + "/Body");
        faces = Resources.LoadAll<Sprite>(path + "/Face");
    }

    public Sprite GetHead()
    {
        int index = Random.Range(0, heads.Length);
        return heads[index];
    }

    public  Sprite GetBody()
    {
        int index = Random.Range(0, bodies.Length);
        return bodies[index];
    }

    public Sprite GetFace()
    {
        int index = Random.Range(0, faces.Length);
        return faces[index];
    }

    public float GetHeadPositionY(Sprite body)
    {
        float size = body.bounds.size.y * 100;
        for (int i = 0; i < bodySizes.Length; i++)
        {
            if (size == bodySizes[i])
            {
                return (float)headPositions[i] / 100;
            }
        }
        return (float)headPositions[0]/100; //Just in case
    }
}