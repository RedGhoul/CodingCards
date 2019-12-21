using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public GameObject MainPanel;
    public GameObject QuestionCRUDPanel;
    public GameObject PlayPanel;

    private Dictionary<GameObject, bool> state;

    void Awake()
    {
        state = new Dictionary<GameObject, bool>
        {
            { MainPanel, true },
            { QuestionCRUDPanel, false },
            { PlayPanel, false }
        };
    }
    void Start()
    {
        foreach (KeyValuePair<GameObject, bool> entry in state)
        {
            entry.Key.SetActive(entry.Value);
        }
    }

    public void OnPlayButtonPress()
    {
        TurnOnView("PlayPanel");
    }

    public void OnQuestionButtonPress()
    {
        TurnOnView("QuestionPanel");
    }


    private void TurnOnView(string panelName)
    {
        foreach (KeyValuePair<GameObject, bool> entry in state)
        {
            entry.Key.SetActive(entry.Key.name.Equals(panelName));
        }
    }

}
