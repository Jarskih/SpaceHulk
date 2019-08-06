using Interfaces;
using UnityEngine;

public class MoveForwardCommand : ICommand
{
    private readonly Transform _transform;
    private readonly Stats _unit;
    private readonly Vector3 _direction;
    private readonly int _moveCost;

    public MoveForwardCommand(Stats unit)
    {
        _unit = unit;
        _transform = _unit.transform;
        _direction = _transform.up;
        _moveCost = _unit.APrules.moving;
    }
    
    public void Execute()
    {
        _unit._targetPos = _transform.position + _direction;
        _unit.UpdateMovementPoints(-_moveCost);
        EventManager.TriggerEvent("PlayerWalk");
        _unit.UpdateCurrentTile(_unit._targetPos);
    }

    public void Undo()
    {
        _unit._targetPos = _transform.position - _direction;
        _unit.UpdateMovementPoints(_moveCost);
        EventManager.TriggerEvent("PlayerWalk");
        _unit.UpdateCurrentTile(_unit._targetPos);
    }
}