using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HideUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public GameObject[] uiToDisable;
    private TurnSystem _turnSystem;
    public Color marineColor;
    public Color alienColor;
    
    // Start is called before the first frame update
    void Start()
    {
        _turnSystem = FindObjectOfType<TurnSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_turnSystem.GetCurrentPhase() == TurnSystem.Phases.FirstMovement)
        {
            EnableUI();
            UpdateTurnText("Marine Turn", marineColor);
        }
        else
        {
            DisableUI();
            UpdateTurnText("Alien Turn", alienColor);
        }
    }

    void DisableUI()
    {
        foreach (var gameObject in uiToDisable)
        {
            gameObject.SetActive(false);
        }
    }

    void EnableUI()
    {
        foreach (var gameObject in uiToDisable)
        {
            gameObject.SetActive(true);
        }
    }

    void UpdateTurnText(string value, Color color)
    {
        text.text = value;
        text.color = color;
    }
}
