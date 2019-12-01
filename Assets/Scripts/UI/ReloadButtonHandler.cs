using UnityEngine;
using UnityEngine.UI;

public class ReloadButtonHandler : MonoBehaviour
{
    public Button reloadButton;

    private PlayerInteractions _pi;

    // Start is called before the first frame update
    private void Start()
    {
        _pi = FindObjectOfType<PlayerInteractions>();
    }

    // Update is called once per frame
    private void Update()
    {
        var activeUnit = _pi.activeUnit;
        reloadButton.interactable = activeUnit.CanReload();
    }
}
