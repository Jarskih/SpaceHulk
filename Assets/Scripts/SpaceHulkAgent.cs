using System;
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
    private EnemyMovement _movement;
    private Unit _unit;
    private TilemapController _tilemapController;
    public bool killedPlayer;

    private const int Up = 0;
    private const int Down = 1;
    private const int Left = 2;
    private const int Right = 3;

    private float newDist;
    private float oldDist;
    
    [Tooltip("Selecting will turn on action masking. Note that a model trained with action " +
             "masking turned on may not behave optimally when action masking is turned off.")]
    public bool maskActions = true;

    public override void InitializeAgent()
    {
        _academy = FindObjectOfType(typeof(SpaceHulkAcademy)) as SpaceHulkAcademy;
        _tilemapController = FindObjectOfType<TilemapController>();
        _movement = GetComponent<EnemyMovement>();
        _unit = GetComponent<Unit>();
    }

    public override void CollectObservations()
    {
        // AddVectorObs(_unit.targetPos);
        var surroundingTiles = _tilemapController.GetSurroundingTiles(_unit.targetPos, _unit);
        foreach (var tile in surroundingTiles)
        {
            AddVectorObs(tile);
        }
        
        // Normalized Action points
        // To normalize a value to [0, 1], you can use the following formula:
        // normalizedValue = (currentValue - minValue)/(maxValue - minValue)
        //var ap = _unit.actionPoints / 6;
        //AddVectorObs(ap);

        if (_player == null)
        {
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
        }
        
        if (_player != null)
        {
            AddVectorObs(Vector3.Distance(_player.targetPos, _unit.targetPos));
            AddVectorObs(_player.targetPos);
            AddVectorObs((_player.targetPos - _unit.targetPos).normalized);
        }
        else
        {
            AddVectorObs(0);
            AddVectorObs(Vector3.zero);
            AddVectorObs(Quaternion.identity);
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
            Vector3 pos = _unit.targetPos - transform.right;
            Vector3Int left = new Vector3Int((int)pos.x, (int)pos.y, 0);
            if (!_tilemapController.IsFloor(left))
            {
                SetActionMask(Left);
            }
        }
        
        {
            // Check right
            Vector3 pos = _unit.targetPos + transform.right;
            Vector3Int right = new Vector3Int((int)pos.x, (int)pos.y, 0);
            if (!_tilemapController.IsFloor(right))
            {
                SetActionMask(Right);
            }
        }
        
        {
            // Check up
            Vector3 pos = _unit.targetPos + transform.up;
            Vector3Int up = new Vector3Int((int)pos.x, (int)pos.y, 0);
            if (!_tilemapController.IsFloor(up))
            {
                SetActionMask(Up);
            }
        }
        
        {
            // Check down
            Vector3 pos = _unit.targetPos - transform.up;
            Vector3Int down = new Vector3Int((int)pos.x, (int)pos.y, 0);
            if (!_tilemapController.IsFloor(down))
            {
                SetActionMask(Down);
            }
        }
    }

    // to be implemented by the developer
    public override void AgentAction(float[] vectorAction, string textAction)
    {   
        SetReward(-0.01f);
        int action = Mathf.FloorToInt(vectorAction[0]);
        Vector3 pos;
        
        switch (action)
        {
            case Right:
               //Debug.Log("Right");
               pos = _unit.targetPos + transform.right;
               Vector3Int right = new Vector3Int((int)pos.x, (int)pos.y, 0);
               if (_tilemapController.IsFloor(right))
               {
                   _movement.CanMove(transform.right);
               }
               else
               {
                   Done();
               }
               break;
            case Left:
               //Debug.Log("Left");
               pos = _unit.targetPos - transform.right;
               Vector3Int left = new Vector3Int((int)pos.x, (int)pos.y, 0);
               if (_tilemapController.IsFloor(left))
               {
                   _movement.CanMove(-transform.right);
               }
               else
               {
                   Done();
               }

               break;
            case Up:
               //Debug.Log("Up");
               pos = _unit.targetPos + transform.up;
               Vector3Int up = new Vector3Int((int)pos.x, (int)pos.y, 0);
               if (_tilemapController.IsFloor(up))
               {
                   _movement.CanMove(transform.up);
               }
               else
               {
                   Done();
               }
                break;
            case Down:
               //Debug.Log("Down");
               pos = _unit.targetPos - transform.up;
               Vector3Int down = new Vector3Int((int)pos.x, (int)pos.y, 0);
               if (_tilemapController.IsFloor(down))
               {
                   _movement.CanMove(-transform.up);
               }
               else
               {
                   Done();
               }
               break;
            default:
                throw new ArgumentException("Invalid action value");
        }

        if (killedPlayer)
        {
            Done();
            SetReward(1f);
        } else if (_unit.isDead)
        {
            Done();
        }
    }

    // to be implemented by the developer
    public override void AgentReset()
    {
        if (!_academy.GetIsInference())
        {
            _unit.targetPos = _unit.startingPos;
            _unit.transform.position = _unit.startingPos;
           // _academy.AcademyReset();
        }
        killedPlayer = false;
    }
    
   // Note that in order for AgentOnDone() to be called, the Agent's ResetOnDone property must be false. You can set ResetOnDone on the Agent's Inspector or in code.
    public override void AgentOnDone()
    {
        Destroy(gameObject);
    }
}
