using UnityEngine;
using UnityEngine.UI;

public class ReloadButtonHandler : MonoBehaviour
{
    public Button reloadButton;

    private TurnSystem _turnSystem;

    // Start is called before the first frame update
    private void Start()
    {
        _turnSystem = FindObjectOfType<TurnSystem>();
    }

    // Update is called once per frame
    private void Update()
    {
        var activeUnit = _turnSystem.activeUnit;
        reloadButton.interactable = activeUnit.CanReload();
    }
}
