using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class TurnCommand : ICommand
{
    private readonly Unit _unit;
    private readonly Vector3 _direction = Vector3.up;
    private readonly int _moveCost;
    private readonly int _angle;

    public TurnCommand(Unit unit, int angle)
    {
        _unit = unit;
        _moveCost = _unit.APrules.turning;
        _angle = angle;
    }
    
    public void Execute()
    {
        _unit.transform.Rotate(0, 0, _angle);
        _unit.UpdateMovementPoints(-_moveCost);
    }

    public void Undo()
    {
        _unit.transform.Rotate(0, 0, -_angle);
        _unit.UpdateMovementPoints(_moveCost);
    }
}
