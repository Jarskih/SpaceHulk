using UnityEngine;

public class UIButtonHandler : MonoBehaviour
{
    private TurnSystem _turnSystem;

    // Start is called before the first frame update
    private void Start()
    {
        _turnSystem = FindObjectOfType<TurnSystem>();
    }

    // UI
    public void Reload()
    {
        _turnSystem.activeUnit.Reload();
    }

    public void Aim()
    {
        _turnSystem.activeUnit.Aim();
    }

    public void EndTurn()
    {
        _turnSystem.SetNextPhase();
    }

    public void Shoot()
    {
        _turnSystem.activeUnit.Shoot();
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
