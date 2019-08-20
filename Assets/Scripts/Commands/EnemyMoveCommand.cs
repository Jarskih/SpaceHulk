using Interfaces;
using UnityEngine;

public class EnemyMoveCommand : ICommand
{
    private readonly Transform _transform;
    private readonly Unit _unit;
    private readonly Vector3 _direction;
    private readonly int _moveCost;

    public EnemyMoveCommand(Vector3 direction, Unit unit)
    {
        _unit = unit;
        _transform = _unit.transform;
        _direction = direction;
        _moveCost = _unit.APrules.moving;
    }
    
    public void Execute()
    {
        _unit.targetPos = _transform.position + _direction;
        _unit.UpdateMovementPoints(-_moveCost);
        EventManager.TriggerEvent("EnemyMove");
        _unit.UpdateCurrentTile(_unit.targetPos);
    }

    public void Undo()
    {
        _unit.targetPos = _transform.position - _direction;
        _unit.UpdateMovementPoints(_moveCost);
        EventManager.TriggerEvent("EnemyMove");
        _unit.UpdateCurrentTile(_unit.targetPos);
    }
}