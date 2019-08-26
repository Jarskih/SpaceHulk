using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetWeaponName : MonoBehaviour
{
    private TurnSystem _turnSystem;
    private TextMeshProUGUI _textMesh;
    private Unit _unit;
    
    // Start is called before the first frame update
    void Start()
    {
        _turnSystem = FindObjectOfType<TurnSystem>();
        _textMesh = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        var unit = _turnSystem.activeUnit;
        if (_turnSystem.activeUnit.unitType == Unit.UnitType.Marine)
        {
            _unit = unit;
            _textMesh.text = _unit.GetWeaponStats().name;
        }
    }
}
