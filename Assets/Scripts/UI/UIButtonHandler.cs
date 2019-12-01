using UnityEngine;

public class UIButtonHandler : MonoBehaviour
{
    private PlayerInteractions _pi;
    private TurnSystem _turnSystem;

    // Start is called before the first frame update
    private void Start()
    {
        _pi = FindObjectOfType<PlayerInteractions>();
        _turnSystem = FindObjectOfType<TurnSystem>();
    }

    // UI
    public void Reload()
    {
        _pi.activeUnit.Reload();
    }

    public void Aim()
    {
        _pi.activeUnit.Aim();
    }

    public void EndTurn()
    {
        _turnSystem.SetNextPhase();
    }

    public void Shoot()
    {
        _pi.activeUnit.Shoot();
    }

    public void Undo()
    {
        CommandInvoker.Undo();
    }

    public void Redo()
    {
        CommandInvoker.Redo();
    }
}
