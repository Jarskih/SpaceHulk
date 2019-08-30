using UnityEngine;

public class CurrentUnitUI : MonoBehaviour
{
    public IntVariable currentAP;
    public IntVariable currentHealth;
    public StringVariable currentUnitName;
    private TurnSystem _turnSystem;
    private Unit _activeUnit;

    public Unit activeUnit => _activeUnit;

    private void Start()
    {
        _turnSystem = FindObjectOfType<TurnSystem>();
    }

    private void Update()
    {
        _activeUnit = _turnSystem.activeUnit;
        currentAP.Value = _activeUnit.actionPoints;
        currentHealth.Value = _activeUnit.health;
        currentUnitName.Value = _activeUnit.unitStats.unitName;
    }
}
