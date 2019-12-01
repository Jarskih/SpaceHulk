using Interfaces;
using UnityEngine;

public class MoveBackwardCommand : ICommand
{
    private readonly Transform _transform;
    private readonly Unit _unit;
    private readonly Vector3 _direction;
    private readonly int _moveCost;
    private DoorNode _door;

    public MoveBackwardCommand(Unit unit)
    {
        _unit = unit;
        _transform = _unit.transform;
        _direction = -_transform.up;
        _moveCost = _unit.APrules.backwardsMove;
    }
    
    public void Execute()
    {
        _unit.UpdateCurrentTile(_transform.position + _direction);
        _unit.UpdateMovementPoints(-_moveCost);
        EventManager.TriggerEvent("PlayerWalk");
        
        if (_unit.currentNode.GetComponent<IOpenable>() == null) return;
        _door = _unit.currentNode.GetComponent<IOpenable>() as DoorNode;
        if (_door != null) _door.Open();
    }

    public void Undo()
    {
        _unit.UpdateCurrentTile(_transform.position - _direction);
        _unit.UpdateMovementPoints(_moveCost);
        EventManager.TriggerEvent("PlayerWalk");
        
        if (_door != null)
        {
            _door.Close();
        }
    }
}