using System.Collections.Generic;
using UnityEngine;

public class UnitPortrait : MonoBehaviour
{
    public Sprite _unitPortrait;
    public Sprite _unitDead;
    public GameObject portraitGameObject;
    public TurnSystem _turnSystem;
    private List<Unit> _units = new List<Unit>();
    private bool isInit;

    private void Start()
    {
        _turnSystem = FindObjectOfType<TurnSystem>();
    }

    private void Update()
    {
        if (_units.Count == 0)
        {
            _units = _turnSystem.players;
        }

        if (_units.Count > 0 && !isInit)
        {
            isInit = true;
            foreach (var unit in _units)
            {
                var unitInstance = Instantiate(portraitGameObject, Vector3.zero, Quaternion.identity);
                var upManager = unitInstance.GetComponent<UnitPortraitManager>();
                upManager.unit = unit;
                unitInstance.transform.SetParent(transform);
                unitInstance.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
