using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MainMenu : MonoBehaviourPunCallbacks
{
    public static MainMenu current;

    public GameObject[] UI;


    // Start is called before the first frame update
    void Awake()
    {
        if (current != null && current != this)
        {
            Destroy(current);
        }
        else
        {
            current = this;
        }

        CloseAll();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Open(int UIIndex)
    {
        for (int i = 0; i < UI.Length; i++)
        {
            if (i == UIIndex)
            {
                UI[i].SetActive(true);
            }
            else
            {
                UI[i].SetActive(false);
            }
        }
    }

    public void CloseAll()
    {
        for (int i = 0; i < UI.Length; i++)
        {
            UI[i].SetActive(false);
        }
    }
}
