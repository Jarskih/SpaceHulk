using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IsUnitAiming : MonoBehaviour
{
    public Button aimButton;
    public GameObject aimPanel;

    private TurnSystem _turnSystem;
    // Start is called before the first frame update
    void Start()
    {
        _turnSystem = FindObjectOfType<TurnSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        var unitState = _turnSystem.activeUnit.currentState;
        if (unitState == Unit.UnitState.Shooting)
        {
            aimButton.gameObject.SetActive(false);
            aimPanel.SetActive(true);
        }
        else
        {
            aimButton.gameObject.SetActive(true);
            aimPanel.SetActive(false);
        }
    }
}
