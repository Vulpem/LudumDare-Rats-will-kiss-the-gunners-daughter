using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {

    public TextManager manager;
    public GameObject globalPanel;

    public Text tutorial1;
    public Text tutorial2;
    public GameObject[] characterPositions;
    public GameObject[] characterNames;

    public GameObject button1;
    public GameObject button2;

    bool page1 = true;

	// Use this for initialization
	void Start () {
        page1 = false;
        globalPanel.SetActive(true);
        TogglePage();
        globalPanel.SetActive(false);

        tutorial1.text = manager.tutorial1;
        tutorial2.text = manager.tutorial2;
    }

    void Update()
    {
        if (globalPanel.activeInHierarchy)
        {
            for (int n = 0; n < manager.CharacterGOs.Length; n++)
            {
                manager.CharacterGOs[n].gameObject.SetActive(!page1);
                manager.CharacterGOs[n].transform.position = characterPositions[n].transform.position;
                //manager.CharacterGOs[n].transform.localScale = characterPositions[n].transform.localScale;
            }
        }
    }
	
    public void TogglePage()
    {
        if (globalPanel.activeInHierarchy)
        {
            page1 = !page1;

            tutorial1.gameObject.SetActive(page1);
            tutorial2.gameObject.SetActive(!page1);

            button1.SetActive(page1);
            button2.SetActive(!page1);

            foreach (GameObject go in characterNames)
            {
                go.SetActive(!page1);
            }
        }
    }

    public void OpenScroll()
    {
        manager.blockInteraction = true;
        globalPanel.SetActive(true);
    }

    public void CloseScroll()
    {
        for (int n = 0; n < manager.CharacterGOs.Length; n++)
        {
            if (manager.CharacterGOs[n].type != manager.talkingWith)
            {
                manager.CharacterGOs[n].gameObject.SetActive(false);
            }
            else
            {
                manager.CharacterGOs[n].gameObject.SetActive(true);
            }
        }

        manager.blockInteraction = false;
        globalPanel.SetActive(false);
    }
}
