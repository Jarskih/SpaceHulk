using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public List<Stats> enemies;
    public Stats activeEnemy;
    private int _activeEnemyIndex;
    public Stats[] players;
    [SerializeField]
    private Stats activePlayer;
    [SerializeField]
    private int activePlayerIndex;
    [SerializeField]
    private Phases currentPhase;

    private SpawnEnemies _spawnEnemies;
    
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
        // First phase
        currentPhase = Phases.FirstMovement;
        
        // Choose first player
        SetFirstPlayer();
        UpdateMovementPoints();
        
        // Spawn enemies
        _spawnEnemies = GetComponent<SpawnEnemies>();
        _spawnEnemies.Spawn(1);

        UpdateEnemyList();
        
        // Choose first enemy
        SetFirstEnemy();
    }

    void SetFirstEnemy()
    {
        _activeEnemyIndex = 0;
        activeEnemy = enemies[0];
        activeEnemy.UpdateMovementPoints(4);
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
            SetNextPhase();
            SetFirstPlayer();
            Debug.Log("All players moved, setting new phase: " + currentPhase);
        }
        else
        {
            Debug.Log("Next player turn");
            activePlayer = players[activePlayerIndex];
        }
    }

    void NextEnemy()
    {
        _activeEnemyIndex = _activeEnemyIndex + 1;
        if (_activeEnemyIndex >= enemies.Count)
        {
            SetNextPhase();
            SetFirstEnemy();
            Debug.Log("All enemies moved, setting new phase: " + currentPhase);
        }
        else
        {
            Debug.Log("Next alien turn");
            activeEnemy = enemies[_activeEnemyIndex];
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

        UpdateMovementPoints();
    }

    void UpdateMovementPoints()
    {
        if (currentPhase == Phases.FirstMovement || currentPhase == Phases.SecondMovement)
        {
            foreach (var player in players)
            {
                player.UpdateMovementPoints(2);
            }
        }

        if (currentPhase == Phases.Actions)
        {
            foreach (var player in players)
            {
                player.UpdateMovementPoints(1);
            }
        }

        if (currentPhase == Phases.EnemyMovement)
        {
            foreach (var enemy in enemies)
            {
                enemy.UpdateMovementPoints(4);
            }
        }
    }

    void UpdateEnemyList()
    {
        // Find enemies
        enemies.Clear();
        var enemyGameObjects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemyGameObjects)
        {
            enemies.Add(enemy.GetComponent<Stats>());
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            activePlayer.actionPoints = 0;
            SetNextPlayer();
        }
        
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
            case(Phases.EnemySpawn):
                _spawnEnemies.Spawn(6);
                UpdateEnemyList();
                SetNextPhase();
                break;
            case(Phases.Resolution):
                SetNextPhase();
                break;
            default:
                break;    
        }
    }
}