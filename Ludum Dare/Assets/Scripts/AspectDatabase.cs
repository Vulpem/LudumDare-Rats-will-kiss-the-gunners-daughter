using System.Collections;
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

    List<int> givenFaces;
    List<int> givenHeads;
    List<int> givenBodies;

	// Use this for initialization
	void Awake ()
    {
        givenFaces = new List<int>();
        givenHeads = new List<int>();
        givenBodies = new List<int>();

        heads = Resources.LoadAll<Sprite>(path + "/Head");
        bodies = Resources.LoadAll<Sprite>(path + "/Body");
        faces = Resources.LoadAll<Sprite>(path + "/Face");
    }

    public Sprite GetHead()
    {
        while(true)
        {
            int index = Random.Range(0, heads.Length);
            bool repeated = false;
            for (int i = 0; i < givenHeads.Count; i++)
            {
                if (givenHeads[i] == index)
                {
                    repeated = true;
                    break;
                }
            }
            if (repeated == false)
            {
                givenHeads.Add(index);
                return heads[index];
            }
        }
    }

    public  Sprite GetBody()
    {
        while(true)
        {
            int index = Random.Range(0, bodies.Length);
            int repeated = 0;
            for (int i = 0; i < givenBodies.Count; i++)
            {
                if (givenBodies[i] == index)
                    repeated++;
            }
            if (repeated < 2)
            {
                givenBodies.Add(index);
                return bodies[index];
            }
        }
    }

    public Sprite GetFace()
    {
        while(true)
        {
            int index = Random.Range(0, faces.Length);
            bool repeated = false;
            for (int i = 0; i < givenFaces.Count; i++)
            {
                if (givenFaces[i] == index)
                {
                    repeated = true;
                    break;
                }
            }
            if (repeated == false)
            {
                givenFaces.Add(index);
                return faces[index];
            }
        }
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
