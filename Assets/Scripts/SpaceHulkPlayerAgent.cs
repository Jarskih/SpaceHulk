using System;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

public class SpaceHulkPlayerAgent : Agent
{
    private SpaceHulkAcademy _academy;
    private TilemapController _tilemapController;
    private Movement _movement;
    private Unit _unit;
    private List<Unit> _enemies;

    private const int Forward = 0;
    private const int Backward = 1;
    private const int Left = 2;
    private const int Right = 3;
    
    [Tooltip("Selecting will turn on action masking. Note that a model trained with action " +
                 "masking turned on may not behave optimally when action masking is turned off.")]
    public bool maskActions = true;

    public override void InitializeAgent()
    {
        _academy = FindObjectOfType(typeof(SpaceHulkAcademy)) as SpaceHulkAcademy;
        _tilemapController = FindObjectOfType<TilemapController>();
        _movement = GetComponent<Movement>();
        _unit = GetComponent<Unit>();
    }

    public override void CollectObservations()
    {
        Vector3Int intPos = _unit.GetCurrentTilePos();
        AddVectorObs(_tilemapController.IsWalkable(intPos + Vector3Int.up, _unit.GetTileMap()));
        AddVectorObs(_tilemapController.IsWalkable(intPos - Vector3Int.up, _unit.GetTileMap()));
        AddVectorObs(_tilemapController.IsWalkable(intPos + Vector3Int.right, _unit.GetTileMap()));
        AddVectorObs(_tilemapController.IsWalkable(intPos - Vector3Int.right, _unit.GetTileMap()));
        AddVectorObs(_unit.TargetPos);
        AddVectorObs(_unit.transform.up);
    }

    
    /// <summary>
    /// Applies the mask for the agents action to disallow unnecessary actions.
    /// </summary>
    private void SetMask()
    {
        // Mask moving back or forward when there is a wall
        {
            // Check up
            Vector3 pos = _unit.TargetPos + transform.up;
            Vector3Int up = new Vector3Int(Mathf.RoundToInt(pos.x), (int)pos.y, 0);
            if (!_tilemapController.IsWalkable(up, _unit.GetTileMap()))
            {
                SetActionMask(Forward);
            }
            else
            {
                if (_unit.actionPoints < 2)
                {
                    if(_tilemapController.CheckIfTileOccupied(up, _unit))
                    {
                        SetActionMask(Forward);
                    }
                }
            }
        }
        
        {
            // Check down
            Vector3 pos = _unit.TargetPos - transform.up;
            Vector3Int down = new Vector3Int(Mathf.RoundToInt(pos.x), (int)pos.y, 0);
            if (!_tilemapController.IsWalkable(down, _unit.GetTileMap()))
            {
                SetActionMask(Backward);
            }
            else
            {
                if(_tilemapController.CheckIfTileOccupied(down, _unit))
                {
                    SetActionMask(Backward);
                }
            }
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(-0.01f);
        int action = Mathf.FloorToInt(vectorAction[0]);
        Vector3 pos;
        
        switch (action)
        {
            case Right:
               //Debug.Log("Right");
               _movement.Turn(-90);
               break;
            case Left:
               //Debug.Log("Left");
               _movement.Turn(90);
               break;
            case Forward:
               //Debug.Log("Up");
               pos = _unit.TargetPos + transform.up;
               Vector3Int up = new Vector3Int(Mathf.RoundToInt(pos.x), (int)pos.y, 0);
               _movement.CanMove(transform.up);
                break;
            case Backward:
               //Debug.Log("Down");
               pos = _unit.TargetPos - transform.up;
               Vector3Int down = new Vector3Int(Mathf.RoundToInt(pos.x), (int)pos.y, 0);
               _movement.CanMove(-transform.up);
               break;
            default:
                throw new ArgumentException("Invalid action value");
        }
        
        // Mask the necessary actions if selected by the user.
        if (maskActions)
        {
            SetMask();
        }
    }

    public override void AgentReset()
    {
        
    }
    
    // Note that in order for AgentOnDone() to be called, the Agent's ResetOnDone property must be false. You can set ResetOnDone on the Agent's Inspector or in code.
    public override void AgentOnDone()
    {
        Destroy(gameObject);
    }
    
}
