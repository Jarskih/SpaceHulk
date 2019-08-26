﻿using System.Collections;
using System.Collections.Generic;
using Barracuda;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class TurnSystem : MonoBehaviour
{
    public IntVariable actionPoints;
    public IntVariable PlayerAPPerTurn;
    [SerializeField] private int minEnemyAPperTurn;
    [SerializeField] private int maxEnemyAPperTurn;
    [SerializeField] private int minEnemiesSpawnedPerTurn;
    [SerializeField] private int maxEnemiesSpawnedPerTurn;
    [SerializeField] private List<Unit> _enemies;
    [SerializeField] private List<Unit> _players;
    [SerializeField] private int activePlayerIndex = 0;
    [SerializeField] private int _activeEnemyIndex = 0;
    [SerializeField] private Phases currentPhase;
    private SetPlayerActive _setPlayerActive;
    [SerializeField] private Unit _activeUnit;
    private GameObject _objective;
    public Color enemyColor;
    private SpawnEnemies _spawnEnemies;
    private CameraFollow _cameraFollow;
    private bool spawnedPlayers;
    [SerializeField]
    private int maxEnemies = 12;
    
    // Timer for limiting AI enemy turn time
    private float _enemyTimer = 0;
    [SerializeField] private float _enemyMaxTime;

    private Tilemap _tilemap;
    private SpawnPlayers _spawnPlayers;

    public TurnSystem(List<Unit> enemies)
    {
        this._enemies = enemies;
    }

    public enum Phases
    {
        FirstMovement,
        EnemyMovement,
        EnemySpawn,
        Resolution
    }

    public List<Unit> enemies => _enemies;
    public List<Unit> players => _players;
    public Unit activeUnit => _activeUnit;

    // Register listeners
    void OnEnable()
    {
        EventManager.StartListening("EnemyDied", UpdateEnemyList);
    }

    void OnDisable()
    {
        EventManager.StopListening("EnemyDied", UpdateEnemyList);
    }

    // Start is called before the first frame update
    void Start()
    {
        _setPlayerActive = GetComponent<SetPlayerActive>();
        _objective = GameObject.FindWithTag("Objective");
        if (_objective == null)
        {
            Debug.LogError("Objective missing from level");

        }
        _spawnPlayers = GetComponent<SpawnPlayers>();
        _cameraFollow = GetComponent<CameraFollow>();
        _tilemap = FindObjectOfType<Tilemap>();
        // First phase
        currentPhase = Phases.FirstMovement;
        _activeUnit = _players[0]; 
               
        // Spawn enemies
        _spawnEnemies = GetComponent<SpawnEnemies>();
        _spawnEnemies.Spawn(GetEnemiesSpawned());

        UpdateEnemyList();

        UpdateMovementPoints();
        
        // Choose first enemy
        CommandInvoker.ResetHistory();
    }

    public Phases GetCurrentPhase()
    {
        return currentPhase;
    }

    public Color GetCurrentColor()
    {
        if ((currentPhase == Phases.FirstMovement) && _activeUnit.unitType == Unit.UnitType.Marine)
        {
           return _activeUnit.GetComponent<AvatarColor>().healthy;
        }

        return enemyColor;
    }

    bool AllPlayersActed()
    {
        foreach (var player in players)
        {
            if (player.actionPoints > 0) return false;
        }
        return true;
    }

    void SetNextPlayer()
    {
        _activeUnit.ReturnToIdle();
        CommandInvoker.ResetHistory();
        activePlayerIndex = activePlayerIndex + 1;
        
        if (activePlayerIndex >= _players.Count)
        {
            if (AllPlayersActed())
            {
                SetNextPhase();
                activePlayerIndex = 0;
                EventManager.TriggerEvent("EnemyTurn");
            }
            else
            {
                activePlayerIndex = -1;
                SetNextPlayer();
            }
            //Debug.Log("All players moved, setting new phase: " + currentPhase);
        }
        else
        {
            //Debug.Log("Next player turn");
            _activeUnit = _players[activePlayerIndex];
            if (_activeUnit.health <= 0)
            {
                SetNextPlayer();
            }
            
            if (AllPlayersActed())
            {
                SetNextPhase();
                activePlayerIndex = 0;
                EventManager.TriggerEvent("EnemyTurn");
            }
            else
            {
                if (_activeUnit.actionPoints <= 0)
                {
                    SetNextPlayer();
                }
            }
        }
    }

    void NextEnemy()
    {
        _activeEnemyIndex = _activeEnemyIndex + 1;
        if (_activeEnemyIndex >= _enemies.Count)
        {
            SetNextPhase();
            _activeEnemyIndex = 0;
            //Debug.Log("All enemies moved, setting new phase: " + currentPhase);
        }
        else
        {
            //Debug.Log("Next alien turn");
            _activeUnit = _enemies[_activeEnemyIndex];
        }
    }

    void SetNextPhase()
    {
        activeUnit.ReturnToIdle();
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
        if (currentPhase == Phases.FirstMovement)
        {
            foreach (var player in _players)
            {
                if (player.health > 0)
                {
                    player.SetActionPoints(PlayerAPPerTurn.Value);
                }
            }
        }

        if (currentPhase == Phases.EnemyMovement)
        {
            foreach (var enemy in _enemies)
            {
                var ap = GetEnemyAP();
                enemy.SetActionPoints(ap);
            }
        }
    }

    int GetEnemyAP()
    {
        return Random.Range(minEnemyAPperTurn, maxEnemyAPperTurn+1);
    }

    int GetEnemiesSpawned()
    {
        return Random.Range(minEnemiesSpawnedPerTurn, maxEnemiesSpawnedPerTurn+1);
    }

    public void UpdateEnemyList()
    {
        // Find enemies
        _enemies.Clear();
        var enemyGameObjects = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemyGameObjects.Length == 0)
        {
            return;
        }
        
        foreach (var enemy in enemyGameObjects)
        {
            _enemies.Add(enemy.GetComponent<Unit>());
        }
    }

    public void UpdatePlayerList()
    {
        _players.Clear();
        var playerObjects = GameObject.FindGameObjectsWithTag("Player");
        
        if (playerObjects.Length == 0)
        {
            return;
        }
        
        foreach (var player in playerObjects)
        {
            _players.Add(player.GetComponent<Unit>());
        }
    }

    void UpdateUI()
    {
        _cameraFollow.SetTarget(_activeUnit.TargetPos);
        
        if (currentPhase == Phases.FirstMovement)
        {
            if (_activeUnit)
            {
                _setPlayerActive.SetUnitActive(_players, _activeUnit);
                actionPoints.Value = _activeUnit.actionPoints;
            }
        }
        else
        {
            _setPlayerActive.DisableActiveUnits(_players);
            if (_activeUnit)
            {
                actionPoints.Value = _activeUnit.actionPoints;
            }
        }
    }

    void FollowPlayer()
    {
        if (_activeUnit != null)
        {
            _cameraFollow.SetTarget(_activeUnit.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        TurnManager();
        UpdateUI();
        FollowPlayer();
        UpdateEnemyList();
        UpdatePlayerList();
    }

    public void NextPlayer()
    {
        if (currentPhase == Phases.FirstMovement)
        {
            SetNextPlayer();
        }
    }

    
    // UI
    public void Reload()
    {
        _activeUnit.Reload();
    }

    public void Aim()
    {
        _activeUnit.Aim();
    }

    public void EndTurn()
    {
        SetNextPhase();
    }

    public void Shoot()
    {
        _activeUnit.Shoot();
    }

    void TurnManager()
    {       
        switch (currentPhase)
        {
            case(Phases.FirstMovement):
                if(Input.GetKeyDown(KeyCode.Tab))
                {
                    SetNextPlayer();
                }
                
                if (_activeUnit == null || _activeUnit.unitType == Unit.UnitType.Alien)
                {
                    if (_players.Count > 0)
                    {
                        activePlayerIndex = -1;
                        SetNextPlayer();
                    }
                    else
                    {
                        break;
                    }
                }
                
                if (_activeUnit.GetComponent<Unit>().health > 0 && _activeUnit.actionPoints > 0)
                {
                    _activeUnit.Actions(_enemies);
                }
                else
                {
                    _activeUnit.actionPoints = 0;
                }
                
                /*
                if (_activeUnit.actionPoints <= 0)
                {
                    SetNextPlayer();
                }
                */
                break;
            case(Phases.EnemyMovement):
                
                _enemyTimer += Time.deltaTime;

                if (_enemyTimer > _enemyMaxTime)
                {
                    NextEnemy();
                    _enemyTimer = 0;
                }
                
                if (_activeUnit == null || _activeUnit.unitType == Unit.UnitType.Marine)
                {
                    if (_enemies.Count > 0)
                    {
                        _activeUnit = _enemies[0];
                    }
                    else
                    {
                        SetNextPhase();
                        break;
                    }
                }
                
                if (_activeUnit.GetComponent<Unit>().health > 0)
                {
                    _activeUnit.Movement();
                }
                else
                {
                    _activeUnit.actionPoints = 0;
                }
                
                if (_activeUnit.actionPoints <= 0)
                {
                    NextEnemy();
                }
                break;
            case(Phases.EnemySpawn):
                if (maxEnemies >_enemies.Count)
                {
                    // Do not spawn more than maxEnemies
                    int enemiesToSpawn = Mathf.Clamp(maxEnemies - _enemies.Count, 0, maxEnemiesSpawnedPerTurn);
                    _spawnEnemies.Spawn(enemiesToSpawn);
                    UpdateEnemyList();
                }
                SetNextPhase();
                break;
            case(Phases.Resolution):
                
                int playersAlive = 0;

                foreach (var player in _players)
                {
                    if (Mathf.RoundToInt(player.TargetPos.x) == Mathf.RoundToInt(_objective.transform.position.x) &&
                        Mathf.RoundToInt(player.TargetPos.y) == Mathf.RoundToInt(_objective.transform.position.y))
                    {
                        if (SceneManager.GetActiveScene().name == "Level1")
                        {
                            SceneManager.LoadScene("Level2");
                        }
                        else
                        {
                            SceneManager.LoadScene("WinScreen");
                        }
                        break;
                    }

                    if (player.health > 0)
                    {
                        playersAlive++;
                    }
                }
                if(playersAlive == 0)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    break;
                }

                _enemyTimer = 0;
                SetNextPhase();
                break;
            default:
                break;    
        }
    }
}