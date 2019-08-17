using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using MLAgents;
using UnityEngine;

public class SpaceHulkAgent : Agent
{
    private SpaceHulkAcademy _academy;
    private Stats _player;
    private EnemyMovement _movement;
    private Stats _stats;
    
    public float timeBetweenDecisionsAtInference;
    private float timeSinceDecision;
    private bool killedPlayer;

    private const int DoNothing = 0;
    private const int Up = 1;
    private const int Down = 2;
    private const int Left = 3;
    private const int Right = 4;

    public override void InitializeAgent()
    {
        _academy = FindObjectOfType(typeof(SpaceHulkAcademy)) as SpaceHulkAcademy;
        _movement = GetComponent<EnemyMovement>();
        _stats = GetComponent<Stats>();
    }

    public override void CollectObservations()
    {
        AddVectorObs(gameObject.transform.position);
        // Normalized Action points
        // To normalize a value to [0, 1], you can use the following formula:
        // normalizedValue = (currentValue - minValue)/(maxValue - minValue)
        var ap = _stats.actionPoints / 4;
        AddVectorObs(ap);

        if (_player == null)
        {
            _player = GameObject.FindWithTag("Player")?.GetComponent<Stats>();
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
        }
    }

    // to be implemented by the developer
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if (timeSinceDecision >= timeBetweenDecisionsAtInference)
        {
            timeSinceDecision = 0f;
        }
        else
        {
            return;
        }
        
        Debug.Log("Action");
       
        AddReward(-0.01f);
        int action = Mathf.FloorToInt(vectorAction[0]);

        Vector3 targetPos = transform.position;
        switch (action)
        {
            case DoNothing:
                break;
            case Right:
                _movement.CanMove(transform.right);
                break;
            case Left:
                _movement.CanMove(-transform.right);
                break;
            case Up:
                _movement.CanMove(transform.up);
                break;
            case Down:
                _movement.CanMove(-transform.up);
                break;
            default:
                throw new ArgumentException("Invalid action value");
        }

        if (killedPlayer)
        {
            Done();
            SetReward(1f);
        }
        if (_stats.isDead)
        {
            Done();
            SetReward(-1f);
        }
    }

    // to be implemented by the developer
    public override void AgentReset()
    {
        _academy.AcademyReset();
    }

    public void FixedUpdate()
    {
        WaitTimeInference();
    }

    private void WaitTimeInference()
    {
         timeSinceDecision += Time.fixedDeltaTime;
    }
}
