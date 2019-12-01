using UnityEngine;

public class CurrentUnitUI : MonoBehaviour
{
    public IntVariable currentAP;
    public IntVariable currentHealth;
    public StringVariable currentUnitName;
    private PlayerInteractions _pi;
    private Unit _activeUnit;

    public Unit activeUnit => _activeUnit;

    private void Start()
    {
        _pi = FindObjectOfType<PlayerInteractions>();
    }

    private void Update()
    {
        _activeUnit = _pi.activeUnit;
        currentAP.Value = _activeUnit.actionPoints;
        currentHealth.Value = _activeUnit.health;
        currentUnitName.Value = _activeUnit.unitStats.unitName;
    }
}
