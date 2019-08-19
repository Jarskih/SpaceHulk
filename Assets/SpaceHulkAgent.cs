using System;
using MLAgents;
using UnityEngine;

public class SpaceHulkAgent : Agent
{
    private SpaceHulkAcademy _academy;
    private Stats _player;
    private EnemyMovement _movement;
    private Stats _stats;
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
        _stats = GetComponent<Stats>();
    }

    public override void CollectObservations()
    {
        AddVectorObs(_stats.targetPos);
        // Normalized Action points
        // To normalize a value to [0, 1], you can use the following formula:
        // normalizedValue = (currentValue - minValue)/(maxValue - minValue)
        var ap = _stats.actionPoints / 6;
        AddVectorObs(ap);

        if (_player == null)
        {
            _player = GameObject.FindWithTag("Player")?.GetComponent<Stats>();
            oldDist = Vector3.Distance(_player.targetPos, _stats.targetPos);
        }
        
        if (_player != null)
        {
            AddVectorObs(_player.transform.position);
            AddVectorObs(_player.transform.rotation);
        }
        else
        {
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
            Vector3 pos = _stats.targetPos - transform.right;
            Vector3Int left = new Vector3Int((int)pos.x, (int)pos.y, 0);
            if (!_tilemapController.IsFloor(left))
            {
                SetActionMask(Left);
            }
        }
        
        {
            // Check right
            Vector3 pos = _stats.targetPos + transform.right;
            Vector3Int right = new Vector3Int((int)pos.x, (int)pos.y, 0);
            if (!_tilemapController.IsFloor(right))
            {
                SetActionMask(Right);
            }
        }
        
        {
            // Check up
            Vector3 pos = _stats.targetPos + transform.up;
            Vector3Int up = new Vector3Int((int)pos.x, (int)pos.y, 0);
            if (!_tilemapController.IsFloor(up))
            {
                SetActionMask(Up);
            }
        }
        
        {
            // Check down
            Vector3 pos = _stats.targetPos - transform.up;
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
        AddReward(-0.01f);
        int action = Mathf.FloorToInt(vectorAction[0]);
        
        switch (action)
        {
            case Right:
               // Debug.Log("Right");
                _movement.CanMove(transform.right);
                break;
            case Left:
               // Debug.Log("Left");
                _movement.CanMove(-transform.right);
                break;
            case Up:
               // Debug.Log("Up");
                _movement.CanMove(transform.up);
                break;
            case Down:
               // Debug.Log("Down");
                _movement.CanMove(-transform.up);
                break;
            default:
                throw new ArgumentException("Invalid action value");
        }

        /*
        newDist = Vector3.Distance(_player.transform.position, _stats.targetPos);
        
        if(newDist < oldDist)
        {
            AddReward(0.1f);
            oldDist = newDist;
        }
        */

        if (killedPlayer)
        {
            Done();
            SetReward(5f);
        } else if (_stats.isDead)
        {
            Done();
            SetReward(-1f);
        }
    }

    // to be implemented by the developer
    public override void AgentReset()
    {
        killedPlayer = false;
        _academy.AcademyReset();
    }
    
    public override void AgentOnDone()
    {
        Destroy(gameObject);
    }
}
