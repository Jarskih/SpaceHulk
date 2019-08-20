using Interfaces;
using UnityEngine;

public class MoveBackwardCommand : ICommand
{
    private readonly Transform _transform;
    private readonly Unit _unit;
    private readonly Vector3 _direction;
    private readonly int _moveCost;

    public MoveBackwardCommand(Unit unit)
    {
        _unit = unit;
        _transform = _unit.transform;
        _direction = -_transform.up;
        _moveCost = _unit.APrules.moving;
    }
    
    public void Execute()
    {
        _unit.targetPos = _transform.position + _direction;
        _unit.UpdateMovementPoints(-_moveCost);
        EventManager.TriggerEvent("PlayerWalk");
        _unit.UpdateCurrentTile(_unit.targetPos);
    }

    public void Undo()
    {
        _unit.targetPos = _transform.position - _direction;
        _unit.UpdateMovementPoints(_moveCost);
        EventManager.TriggerEvent("PlayerWalk");
        _unit.UpdateCurrentTile(_unit.targetPos);
    }
}