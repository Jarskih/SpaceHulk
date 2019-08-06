using Interfaces;
using UnityEngine;

public class DoubleMoveCommand : ICommand
{
    private readonly Transform _transform;
    private readonly Stats _unit;
    private readonly Vector3 _direction;
    private readonly int _moveCost;

    public DoubleMoveCommand(Stats unit)
    {
        _unit = unit;
        _transform = _unit.transform;
        _direction = _transform.up;
        _moveCost = _unit.APrules.moving*2;
    }
    
    public void Execute()
    {
        _unit._targetPos =_transform.position + _direction * 2;
        _unit.UpdateMovementPoints(-_moveCost);
        EventManager.TriggerEvent("PlayerWalk");
    }

    public void Undo()
    {
        _unit._targetPos =_transform.position - _direction * 2;
        _unit.UpdateMovementPoints(_moveCost);
        EventManager.TriggerEvent("PlayerWalk");
    }
}