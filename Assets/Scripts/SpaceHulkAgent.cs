﻿using System;
using MLAgents;
using UnityEngine;

public class SpaceHulkAgent : Agent
{
    /*
     * The ML-Agents Academy class orchestrates the agent simulation loop as follows:
     *   Calls your Academy subclass's AcademyReset() function.
     *   Calls the AgentReset() function for each Agent in the scene.
     *   Calls the CollectObservations() function for each Agent in the scene.
     *   Uses each Agent's Brain to decide on the Agent's next action.
     *   Calls your subclass's AcademyStep() function.
     *   Calls the AgentAction() function for each Agent in the scene, passing in the action chosen by the Agent's Brain. (This function is not called if the Agent is done.)
     *   Calls the Agent's AgentOnDone() function if the Agent has reached its Max Step count or has otherwise marked itself as done. Optionally, you can set an Agent to restart if it finishes before the end of an episode. In this case, the Academy calls the AgentReset() function.
      *  When the Academy reaches its own Max Step count, it starts the next episode again by calling your Academy subclass's AcademyReset() function.
     */

    private SpaceHulkAcademy _academy;
    [SerializeField] private Unit _player;
    private Unit _oldPlayer;
    private EnemyMovement _movement;
    private Unit _unit;
    private TilemapController _tilemapController;
    private SpawnPlayers _spawnPlayers;
    public bool killedPlayer;
    public bool resetPos;
    public bool randomPlayerPos;

    private const int Up = 0;
    private const int Down = 1;
    private const int Left = 2;
    private const int Right = 3;
    
    [Tooltip("Selecting will turn on action masking. Note that a model trained with action " +
             "masking turned on may not behave optimally when action masking is turned off.")]
    public bool maskActions = true;

    public override void InitializeAgent()
    {
        _academy = FindObjectOfType(typeof(SpaceHulkAcademy)) as SpaceHulkAcademy;
        _tilemapController = FindObjectOfType<TilemapController>();
        _movement = GetComponent<EnemyMovement>();
        _unit = GetComponent<Unit>();
        _spawnPlayers = FindObjectOfType<SpawnPlayers>();
    }

    public override void CollectObservations()
    {
        // AddVectorObs(_unit.targetPos);
        //var surroundingTiles = _tilemapController.GetSurroundingTiles(_unit.targetPos, _unit);
        //foreach (var tile in surroundingTiles)
        //{
        //    AddVectorObs(tile);
        //}

        Vector3Int intPos = _unit.GetCurrentTilePos();
        AddVectorObs(_tilemapController.IsFloor(intPos + Vector3Int.up));
        AddVectorObs(_tilemapController.IsFloor(intPos - Vector3Int.up));
        AddVectorObs(_tilemapController.IsFloor(intPos + Vector3Int.right));
        AddVectorObs(_tilemapController.IsFloor(intPos - Vector3Int.right));
        AddVectorObs(_unit.TargetPos);
        
        
        // Normalized Action points
        // To normalize a value to [0, 1], you can use the following formula:
        // normalizedValue = (currentValue - minValue)/(maxValue - minValue)
        //var ap = _unit.actionPoints / 6;
        //AddVectorObs(ap);

            var players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in players)
            {
                if (_player == null)
                {
                    _player = player.GetComponent<Unit>();
                }
                else
                {
                    var currentDistance = Vector3.Distance(transform.position, _player.transform.position);
                    var newDistance = Vector3.Distance(transform.position, player.transform.position);
                    if (newDistance < currentDistance)
                    {
                        _player = player.GetComponent<Unit>();
                    }
                }
            }
      
        if (_player != null)
        {
            AddVectorObs(Vector3.Distance(_player.TargetPos, _unit.TargetPos));
            AddVectorObs(_player.TargetPos);
            AddVectorObs((_player.TargetPos - _unit.TargetPos).normalized);
        }
        else
        {
            AddVectorObs(0);
            AddVectorObs(Vector3.zero);
            AddVectorObs(Vector3.zero);
            Debug.LogError("no player");
        }
        
        
        
        // Mask the necessary actions if selected by the user.
        if (maskActions)
        {
            SetMask();
        }
    }
    
    /// <summary>
    /// Applies the mask for the agents action to disallow unnecessary actions.
    /// </summary>
    private void SetMask()
    {
        {
            // Check left
            Vector3 pos = _unit.TargetPos - transform.right;
            Vector3Int left = new Vector3Int(Mathf.RoundToInt(pos.x), (int)pos.y, 0);
            if (!_tilemapController.IsFloor(left))
            {
                SetActionMask(Left);
            }
            else
            {
                if (_unit.actionPoints < 2)
                {
                    if(_tilemapController.TileOccupiedByAlien(left, _unit))
                    {
                        SetActionMask(Left);
                    }
                }
            }
        }
        
        {
            // Check right
            Vector3 pos = _unit.TargetPos + transform.right;
            Vector3Int right = new Vector3Int(Mathf.RoundToInt(pos.x), (int)pos.y, 0);
            if (!_tilemapController.IsFloor(right))
            {
                SetActionMask(Right);
            }
            else
            {
                if (_unit.actionPoints < 2)
                {
                    if(_tilemapController.TileOccupiedByAlien(right, _unit))
                    {
                        SetActionMask(Right);
                    }
                }
            }
        }
        
        {
            // Check up
            Vector3 pos = _unit.TargetPos + transform.up;
            Vector3Int up = new Vector3Int(Mathf.RoundToInt(pos.x), (int)pos.y, 0);
            if (!_tilemapController.IsFloor(up))
            {
                SetActionMask(Up);
            }
            else
            {
                if (_unit.actionPoints < 2)
                {
                    if(_tilemapController.TileOccupiedByAlien(up, _unit))
                    {
                        SetActionMask(Up);
                    }
                }
            }
        }
        
        {
            // Check down
            Vector3 pos = _unit.TargetPos - transform.up;
            Vector3Int down = new Vector3Int(Mathf.RoundToInt(pos.x), (int)pos.y, 0);
            if (!_tilemapController.IsFloor(down))
            {
                SetActionMask(Down);
            }
            else
            {
                if (_unit.actionPoints < 2)
                {
                    if(_tilemapController.TileOccupiedByAlien(down, _unit))
                    {
                        SetActionMask(Down);
                    }
                }
            }
        }
    }

    // to be implemented by the developer
    public override void AgentAction(float[] vectorAction, string textAction)
    {   
        AddReward(-0.01f);
        int action = Mathf.FloorToInt(vectorAction[0]);
        Vector3 pos;
        
        switch (action)
        {
            case Right:
               //Debug.Log("Right");
               pos = _unit.TargetPos + transform.right;
               Vector3Int right = new Vector3Int(Mathf.RoundToInt(pos.x), (int)pos.y, 0);
               if (_tilemapController.IsFloor(right))
               {
                   _movement.CanMove(transform.right);
               }
               else
               {
                   AddReward(-0.5f);
                   Done();
               }
               break;
            case Left:
               //Debug.Log("Left");
               pos = _unit.TargetPos - transform.right;
               Vector3Int left = new Vector3Int(Mathf.RoundToInt(pos.x), (int)pos.y, 0);
               if (_tilemapController.IsFloor(left))
               {
                   _movement.CanMove(-transform.right);
               }
               else
               {
                   AddReward(-0.5f);
                   Done();
               }

               break;
            case Up:
               //Debug.Log("Up");
               pos = _unit.TargetPos + transform.up;
               Vector3Int up = new Vector3Int(Mathf.RoundToInt(pos.x), (int)pos.y, 0);
               if (_tilemapController.IsFloor(up))
               {
                   _movement.CanMove(transform.up);
               }
               else
               {
                   AddReward(-0.5f);
                   Done();
               }
                break;
            case Down:
               //Debug.Log("Down");
               pos = _unit.TargetPos - transform.up;
               Vector3Int down = new Vector3Int(Mathf.RoundToInt(pos.x), (int)pos.y, 0);
               if (_tilemapController.IsFloor(down))
               {
                   _movement.CanMove(-transform.up);
               }
               else
               {
                   AddReward(-0.5f);
                   Done();
               }
               break;
            default:
                throw new ArgumentException("Invalid action value");
        }

        if (killedPlayer)
        {
            _academy.playersKilled++;
            SetReward(1f);
            Done();
        } else if (_unit.isDead)
        {
            Done();
        }
    }

    // to be implemented by the developer
    public override void AgentReset()
    {
        if (resetPos)
        {
            if (_player != null && randomPlayerPos)
            {
                var tilePos = _spawnPlayers.GetRandomTilePos();
                _player.transform.position = tilePos;
                _player.UpdateCurrentTile(tilePos);
            }
            _unit.UpdateCurrentTile(_unit.startingPos);
            _unit.transform.position = _unit.startingPos;
           _academy.AcademyReset();
        }
        killedPlayer = false;
    }
    
   // Note that in order for AgentOnDone() to be called, the Agent's ResetOnDone property must be false. You can set ResetOnDone on the Agent's Inspector or in code.
    public override void AgentOnDone()
    {
        Destroy(gameObject);
    }
}
