using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public List<Stats> enemies;
    public Stats activeEnemy;
    private int activeEnemyIndex;
    public Stats[] players;
    [SerializeField]
    private Stats activePlayer;
    [SerializeField]
    private int activePlayerIndex;
    [SerializeField]
    private Phases currentPhase;

    private enum Phases
    {
        FirstMovement,
        Actions,
        SecondMovement,
        EnemyMovement,
        EnemySpawn,
        Resolution
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        SetFirstPlayer();
        currentPhase = Phases.FirstMovement;
        UpdateMovementPoints(activePlayer);

        var enemyGameObjects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemyGameObjects)
        {
            enemies.Add(enemy.GetComponent<Stats>());
        }

        activeEnemy = enemies[0];
        activeEnemy.UpdateMovementPoints(4);
    }

    void SetFirstEnemy()
    {
        
    }
    
    void SetFirstPlayer()
    {
        activePlayerIndex = 0;
        activePlayer = players[0];
    }

    void SetNextPlayer()
    {
        activePlayerIndex = activePlayerIndex + 1;
        if (activePlayerIndex >= players.Length)
        {
            Debug.Log("All players moved, setting new phase: " + currentPhase);
            SetNextPhase();
            SetFirstPlayer();
            UpdateMovementPoints(activePlayer);
        }
        else
        {
            Debug.Log("Next player turn");
            activePlayer = players[activePlayerIndex];
            UpdateMovementPoints(activePlayer);
        }
    }

    void NextEnemy()
    {
        activeEnemyIndex = activeEnemyIndex + 1;
        if (activeEnemyIndex >= enemies.Count)
        {
            Debug.Log("All enemies moved, setting new phase: " + currentPhase);
            SetNextPhase();
        }
        else
        {
            Debug.Log("Next alien turn");
            activeEnemy = enemies[activeEnemyIndex];
            UpdateMovementPoints(activeEnemy);
        }
    }

    void SetNextPhase()
    {
        if (currentPhase == Phases.Resolution)
        {
            currentPhase = Phases.FirstMovement;
        }
        else
        {
            currentPhase = currentPhase + 1;
        }
    }

    void UpdateMovementPoints(Stats unit)
    {
        if (currentPhase == Phases.FirstMovement || currentPhase == Phases.SecondMovement)
        {
            unit.UpdateMovementPoints(2);
        }

        if (currentPhase == Phases.Actions)
        {
            unit.UpdateMovementPoints(1);
        }

        if (currentPhase == Phases.EnemyMovement)
        {
            unit.UpdateMovementPoints(4);
        }
    }
    
    // Update is called once per frame
    void Update()
    {        
        switch (currentPhase)
        {
            case(Phases.FirstMovement):
            case(Phases.SecondMovement):
                activePlayer.Movement();
                if (activePlayer.actionPoints <= 0)
                {
                    SetNextPlayer();
                }
                break;
            case(Phases.Actions):
                activePlayer.Actions();
                if (activePlayer.actionPoints <= 0)
                {
                    SetNextPlayer();
                }
                break;
            case(Phases.EnemyMovement):
                activeEnemy.Movement();
                if (activeEnemy.actionPoints <= 0)
                {
                    NextEnemy();
                }
                break;
            default:
                break;    
        }
    }
}