using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IsUnitAiming : MonoBehaviour
{
    public Button aimButton;
    public Button shootButton;

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
            shootButton.gameObject.SetActive(true);
            aimButton.gameObject.SetActive(false);
        }
        else
        {
            aimButton.gameObject.SetActive(true);
            shootButton.gameObject.SetActive(false);
        }
    }
}
