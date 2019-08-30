using UnityEngine;
using UnityEngine.UI;

public class IsUnitAiming : MonoBehaviour
{
    public Button aimButton;
    public Button shootButton;
    public GameObject aimPanel;

    private TurnSystem _turnSystem;

    // Start is called before the first frame update
    private void Start()
    {
        _turnSystem = FindObjectOfType<TurnSystem>();
    }

    // Update is called once per frame
    private void Update()
    {
        var unitState = _turnSystem.activeUnit.currentState;
        if (unitState == Unit.UnitState.Shooting)
        {
            shootButton.interactable = _turnSystem.activeUnit.CanShoot();
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
