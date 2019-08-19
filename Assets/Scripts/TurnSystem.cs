using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TurnSystem : MonoBehaviour
{
    public IntVariable actionPoints;
    public IntVariable PlayerAPPerTurn;
    [SerializeField] private int minEnemyAPperTurn;
    [SerializeField] private int maxEnemyAPperTurn;
    [SerializeField] private int minEnemiesSpawnedPerTurn;
    [SerializeField] private int maxEnemiesSpawnedPerTurn;
    [SerializeField] private List<Stats> _enemies;
    [SerializeField] private List<Stats> _players;
    [SerializeField] private Stats _activeEnemy;
    [SerializeField] private Stats activePlayer;
    [SerializeField] private int activePlayerIndex;
    [SerializeField]  private Phases currentPhase;
    private int _activeEnemyIndex;
    public Color enemyColor;
    private SpawnEnemies _spawnEnemies;
    private CameraFollow _cameraFollow;
    private bool spawnedPlayers;
    [SerializeField]
    private int maxEnemies = 12;

    private Tilemap _tilemap;
    private SpawnPlayers _spawnPlayers;

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
        _spawnPlayers = GetComponent<SpawnPlayers>();
        _cameraFollow = GetComponent<CameraFollow>();
        _tilemap = FindObjectOfType<Tilemap>();
        // First phase
        currentPhase = Phases.FirstMovement;
        
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
        if ((currentPhase == Phases.FirstMovement || currentPhase == Phases.SecondMovement) && activePlayer)
        {
           return activePlayer.GetComponent<AvatarColor>().healthy;
        }

        return enemyColor;
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
            _enemies.Add(enemy.GetComponent<Stats>());
        }

        if (activeEnemy == null)
        {
            _activeEnemy = _enemies[0];   
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
            _players.Add(player.GetComponent<Stats>());
        }

        if (activePlayer == null)
        {
            activePlayer = _players[0];   
        }
    }

    void UpdateUI()
    {   
        if (currentPhase == Phases.FirstMovement || currentPhase == Phases.SecondMovement)
        {
            if (activePlayer)
            {
                actionPoints.Value = activePlayer.actionPoints;
            }
        }
        else
        {
            if (activeEnemy)
            {
                actionPoints.Value = activeEnemy.actionPoints;
            }
        }
    }

    void FollowPlayer()
    {
        if (currentPhase == Phases.FirstMovement || currentPhase == Phases.SecondMovement)
        {
            if (activePlayer != null)
            {
                _cameraFollow.SetTarget(activePlayer.transform.position);
            }
        }
        else
        {
            if (activeEnemy != null)
            {
                _cameraFollow.SetTarget(activeEnemy.transform.position);
            }
        }
    }

    void SpawnPlayers()
    {
        // Spawn players
        _spawnPlayers.SpawnPlayer();
            
        var foundPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in foundPlayers)
        {
            _players.Add(player.GetComponent<Stats>());
        }
        // Choose first player
        SetFirstPlayer();
        UpdateMovementPoints();
    }
    
    // Update is called once per frame
    void Update()
    {
        UpdateUI();
        FollowPlayer();
        UpdateEnemyList();
        UpdatePlayerList();

        if (!spawnedPlayers)
        {
            if (_tilemap.transform.childCount > 0)
            {
                SpawnPlayers();
                spawnedPlayers = true;
            }
        }
        else
        {
            TurnManager();
        }
    }


    void TurnManager()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            activePlayer.actionPoints = 0;
            SetNextPlayer();
        }
        
        switch (currentPhase)
        {
            case(Phases.FirstMovement):
            case(Phases.SecondMovement):
                if (activePlayer == null)
                {
                    return;
                }
                
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
                if (enemies.Count == 0)
                {
                    SetNextPhase();
                    break;
                }
                
                // TODO enable later
                // EventManager.TriggerEvent("EnemyTurn");
                
                if (activeEnemy && activeEnemy.GetComponent<Stats>().health > 0)
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
                if (maxEnemies >_enemies.Count)
                {
                    // Do not spawn more than maxEnemies
                    int enemiesToSpawn = Mathf.Clamp(maxEnemies - _enemies.Count, 0, 6);
                    _spawnEnemies.Spawn(enemiesToSpawn);
                    UpdateEnemyList();
                }
                SetNextPhase();
                break;
            case(Phases.Resolution):
                if (players.Count == 0)
                {
                    SpawnPlayers();
                }
                SetNextPhase();
                break;
            default:
                break;    
        }
    }
}