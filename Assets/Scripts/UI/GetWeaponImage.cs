using UnityEngine;
using UnityEngine.UI;

public class GetWeaponImage : MonoBehaviour
{
    private PlayerInteractions _pi;
    private Image _image;
    private Unit _unit;

    // Start is called before the first frame update
    private void Start()
    {
        _pi = FindObjectOfType<PlayerInteractions>();
        _image = GetComponent<Image>();
    }

    // Update is called once per frame
    private void Update()
    {
        var unit = _pi.activeUnit;
        if (_pi.activeUnit.unitType == Unit.UnitType.Marine)
        {
            _unit = unit;
            _image.sprite = _unit.GetWeaponStats().weaponImage;
        }
    }
}
