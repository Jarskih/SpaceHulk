using TMPro;
using UnityEngine;

public class SetWeaponName : MonoBehaviour
{
    private PlayerInteractions _pi;
    private TextMeshProUGUI _textMesh;
    private Unit _unit;

    // Start is called before the first frame update
    private void Start()
    {
        _pi = FindObjectOfType<PlayerInteractions>();
        _textMesh = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    private void Update()
    {
        var unit = _pi.activeUnit;
        if (unit == null) return;
        if (_pi.activeUnit.unitType == Unit.UnitType.Marine)
        {
            _unit = unit;
            _textMesh.text = _unit.GetWeaponStats().name;
        }
    }
}
