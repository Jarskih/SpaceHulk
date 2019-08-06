using Interfaces;
using UnityEngine;

public class EnemyDoubleMoveCommand : ICommand
{
    private readonly Transform _transform;
    private readonly Stats _unit;
    private readonly Vector3 _direction;
    private readonly int _moveCost;

    public EnemyDoubleMoveCommand(Vector3 direction, Stats unit)
    {
        _unit = unit;
        _transform = _unit.transform;
        _direction = direction*2;
        _moveCost = _unit.APrules.moving*2;
    }
    
    public void Execute()
    {
        _unit._targetPos = _transform.position + _direction;
        _unit.UpdateMovementPoints(-_moveCost);
        EventManager.TriggerEvent("EnemyMove");
    }

    public void Undo()
    {
        _unit._targetPos = _transform.position - _direction;
        _unit.UpdateMovementPoints(_moveCost);
        EventManager.TriggerEvent("EnemyMove");
    }
}