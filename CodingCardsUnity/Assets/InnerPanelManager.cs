using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerPanelManager : MonoBehaviour
{

    public Dictionary<GameObject,bool> InnerState;

    void Awake()
    {
        InnerState = new Dictionary<GameObject, bool>();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.Equals("CardPanel"))
            {
                InnerState.Add(transform.GetChild(i).gameObject, true);
            }
            else if(transform.GetChild(i).name.Equals("AnswerPanel"))
            {
                InnerState.Add(transform.GetChild(i).gameObject, false);
            }
        }
    }


    void Start()
    {
        foreach (KeyValuePair<GameObject, bool> entry in InnerState)
        {
            entry.Key.SetActive(entry.Value);
        }
    }


    public void OnButtonPress_CardPanel(string type)
    {
        if (type.Equals("Answer"))
        {
            SetInnerPlayPanel(true);
        }
        else if (type.Equals("Next"))
        {
            SetInnerPlayPanel(false);
        }
        else if (type.Equals("AlreadyKnow"))
        {
            Debug.Log("AlreadyKnow");
        }
        else if (type.Equals("DontKnow"))
        {
            Debug.Log("DontKnow");
        }
    }

    private void SetInnerPlayPanel(bool inverse)
    {
        foreach (KeyValuePair<GameObject, bool> entry in InnerState)
        {
            if (entry.Key.name.Equals("CardPanel"))
            {
                entry.Key.SetActive(!inverse);
            }

            if (entry.Key.name.Equals("AnswerPanel"))
            {
                entry.Key.SetActive(inverse);
            }
        }
    }
}
