using System.Collections;
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
    public List<Stats> enemies;
    public Stats activeEnemy;
    private int _activeEnemyIndex;
    private List<Stats> _players = new List<Stats>();
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
    
    private enum Phases
    {
        FirstMovement,
        SecondMovement,
        EnemyMovement,
        EnemySpawn,
        Resolution
    }

    // Register listeners
    void OnEnable()
    {
        EventManager.StartListening("EnemyDied", UpdateEnemyList);
    }

    void OnDisable() {
        EventManager.StopListening("EnemyDied", UpdateEnemyList);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _cameraFollow = GetComponent<CameraFollow>();
        // First phase
        currentPhase = Phases.FirstMovement;

        var players = FindObjectsOfType<Stats>();
        foreach (var player in players)
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
        activeEnemy = enemies[0];
        activeEnemy.UpdateMovementPoints(GetEnemyAP());
    }
    
    void SetFirstPlayer()
    {
        activePlayerIndex = 0;
        activePlayer = _players[0];
    }

    void SetNextPlayer()
    {
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
            foreach (var enemy in enemies)
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
        enemies.Clear();
        var enemyGameObjects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemyGameObjects)
        {
            enemies.Add(enemy.GetComponent<Stats>());
        }

        if (activeEnemy == null)
        {
            activeEnemy = enemies[0];
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
                if (activePlayer.GetComponent<Stats>().Health > 0)
                {
                    activePlayer.Actions(enemies);
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
                if (activeEnemy.GetComponent<Stats>().Health > 0)
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
                if (maxEnemies - enemies.Count > 0)
                {
                    // Do not spawn more than maxEnemies
                    int enemiesToSpawn = Mathf.Clamp(maxEnemies - enemies.Count, 0, 6);
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