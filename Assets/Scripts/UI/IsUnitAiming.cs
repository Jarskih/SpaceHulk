using UnityEngine;
using UnityEngine.UI;

public class IsUnitAiming : MonoBehaviour
{
    public Button aimButton;
    public Button shootButton;
    public GameObject aimPanel;

    private PlayerInteractions _pi;

    // Start is called before the first frame update
    private void Start()
    {
        _pi = FindObjectOfType<PlayerInteractions>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_pi.activeUnit == null) return;
        
        var unitState = _pi.activeUnit.currentState;
        if (unitState == Unit.UnitState.Shooting)
        {
            shootButton.interactable = _pi.activeUnit.CanShoot();
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
