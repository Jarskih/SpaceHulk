﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public IntVariable actionPoints;
    public IntVariable PlayerAPPerTurn;
    [SerializeField] private int minEnemyAPperTurn;
    [SerializeField] private int maxEnemyAPperTurn;
    [SerializeField] private int minEnemiesSpawnedPerTurn;
    [SerializeField] private int maxEnemiesSpawnedPerTurn;
    private List<Stats> _enemies;
    private List<Stats> _players;
    private Stats _activeEnemy;
    private int _activeEnemyIndex;
    [SerializeField]
    private Stats activePlayer;
    [SerializeField]
    private int activePlayerIndex;
    [SerializeField]
    private Phases currentPhase;
    public Color enemyColor;
    private SpawnEnemies _spawnEnemies;
    private CameraFollow _cameraFollow;

    [SerializeField]
    private int maxEnemies = 12;

    public TurnSystem(List<Stats> enemies)
    {
        this._enemies = enemies;
    }

    private enum Phases
    {
        FirstMovement,
        SecondMovement,
        EnemyMovement,
        EnemySpawn,
        Resolution
    }

    public List<Stats> enemies => _enemies;
    public List<Stats> players => _players;
    public Stats activeEnemy => _activeEnemy;

    // Register listeners
    void OnEnable()
    {
        EventManager.StartListening("EnemyDied", UpdateEnemyList);
    }

    void OnDisable() {
        EventManager.StopListening("EnemyDied", UpdateEnemyList);
    }

    private void Awake()
    {
        _enemies = new List<Stats>();
        _players = new List<Stats>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _cameraFollow = GetComponent<CameraFollow>();
        // First phase
        currentPhase = Phases.FirstMovement;

        var foundPlayers = FindObjectsOfType<Stats>();
        foreach (var player in foundPlayers)
        {
            _players.Add(player);
        }
        // Choose first player
        SetFirstPlayer();
        UpdateMovementPoints();
        
        // Spawn enemies
        _spawnEnemies = GetComponent<SpawnEnemies>();
        _spawnEnemies.Spawn(GetEnemiesSpawned());

        UpdateEnemyList();
        
        // Choose first enemy
        CommandInvoker.ResetHistory();
        SetFirstEnemy();
    }

    public Color GetCurrentColor()
    {
        if (currentPhase == Phases.FirstMovement || currentPhase == Phases.SecondMovement)
        {
            return activePlayer.GetComponent<AvatarColor>().healthy;
        }
        else
        {
            return enemyColor;
        }
    }
    
    void SetFirstEnemy()
    {
        _activeEnemyIndex = 0;
        _activeEnemy = _enemies[0];
        activeEnemy.UpdateMovementPoints(GetEnemyAP());
    }
    
    void SetFirstPlayer()
    {
        activePlayerIndex = 0;
        activePlayer = _players[0];
    }

    void SetNextPlayer()
    {
        CommandInvoker.ResetHistory();
        activePlayerIndex = activePlayerIndex + 1;
        if (activePlayerIndex >= _players.Count)
        {
            SetNextPhase();
            SetFirstPlayer();
            Debug.Log("All players moved, setting new phase: " + currentPhase);
        }
        else
        {
            Debug.Log("Next player turn");
            activePlayer = _players[activePlayerIndex];
        }
    }

    void NextEnemy()
    {
        _activeEnemyIndex = _activeEnemyIndex + 1;
        if (_activeEnemyIndex >= _enemies.Count)
        {
            SetNextPhase();
            SetFirstEnemy();
            Debug.Log("All enemies moved, setting new phase: " + currentPhase);
        }
        else
        {
            Debug.Log("Next alien turn");
            _activeEnemy = _enemies[_activeEnemyIndex];
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

        if (currentPhase == Phases.EnemyMovement)
        {
            EventManager.TriggerEvent("EnemyTurn");
        }

        UpdateMovementPoints();
    }

    void UpdateMovementPoints()
    {
        if (currentPhase == Phases.FirstMovement || currentPhase == Phases.SecondMovement)
        {
            foreach (var player in _players)
            {
                player.UpdateMovementPoints(PlayerAPPerTurn.Value);
            }
        }

        if (currentPhase == Phases.EnemyMovement)
        {
            foreach (var enemy in _enemies)
            {
                var ap = GetEnemyAP();
                enemy.UpdateMovementPoints(ap);
            }
        }
    }

    int GetEnemyAP()
    {
        return Random.Range(minEnemyAPperTurn, maxEnemyAPperTurn);
    }

    int GetEnemiesSpawned()
    {
        return Random.Range(minEnemiesSpawnedPerTurn, maxEnemiesSpawnedPerTurn);
    }

    public void UpdateEnemyList()
    {
        // Find enemies
        _enemies.Clear();
        var enemyGameObjects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemyGameObjects)
        {
            _enemies.Add(enemy.GetComponent<Stats>());
        }

        if (activeEnemy == null)
        {
            _activeEnemy = _enemies[0];
        }
    }

    void UpdateUI()
    {
        if (currentPhase == Phases.FirstMovement || currentPhase == Phases.SecondMovement)
        {
            actionPoints.Value = activePlayer.actionPoints;
        }
        else
        {
            actionPoints.Value = activeEnemy.actionPoints;
        }
    }

    void FollowPlayer()
    {
        if (currentPhase == Phases.FirstMovement || currentPhase == Phases.SecondMovement)
        {
            _cameraFollow.SetTarget(activePlayer.transform.position);
        }
        else
        {
            _cameraFollow.SetTarget(activeEnemy.transform.position);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        UpdateUI();
        FollowPlayer();
        UpdateEnemyList();
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            activePlayer.actionPoints = 0;
            SetNextPlayer();
        }
        
        switch (currentPhase)
        {
            case(Phases.FirstMovement):
            case(Phases.SecondMovement):
                if (activePlayer.GetComponent<Stats>().health > 0)
                {
                    activePlayer.Actions(_enemies);
                }
                else
                {
                    activePlayer.actionPoints = 0;
                }
                
                
                if (activePlayer.actionPoints <= 0)
                {
                    SetNextPlayer();
                }
                break;
            case(Phases.EnemyMovement):     
                if (activeEnemy.GetComponent<Stats>().health > 0)
                {
                    activeEnemy.Movement();
                }
                else
                {
                    activeEnemy.actionPoints = 0;
                }
                
                if (activeEnemy.actionPoints <= 0)
                {
                    NextEnemy();
                }
                break;
            case(Phases.EnemySpawn):
                if (maxEnemies - _enemies.Count > 0)
                {
                    // Do not spawn more than maxEnemies
                    int enemiesToSpawn = Mathf.Clamp(maxEnemies - _enemies.Count, 0, 6);
                    _spawnEnemies.Spawn(enemiesToSpawn);
                    UpdateEnemyList();
                }
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