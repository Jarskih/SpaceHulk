﻿using UnityEngine;
using UnityEngine.UI;

public class GetWeaponImage : MonoBehaviour
{
    private TurnSystem _turnSystem;
    private Image _image;
    private Unit _unit;

    // Start is called before the first frame update
    private void Start()
    {
        _turnSystem = FindObjectOfType<TurnSystem>();
        _image = GetComponent<Image>();
    }

    // Update is called once per frame
    private void Update()
    {
        var unit = _turnSystem.activeUnit;
        if (_turnSystem.activeUnit.unitType == Unit.UnitType.Marine)
        {
            _unit = unit;
            _image.sprite = _unit.GetWeaponStats().weaponImage;
        }
    }
}