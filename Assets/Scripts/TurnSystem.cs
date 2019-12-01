using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class TurnSystem : MonoBehaviour
{
    [SerializeField] private int minEnemiesSpawnedPerTurn;
    [SerializeField] private int maxEnemiesSpawnedPerTurn;
    [SerializeField] private List<Unit> _enemies;
    [SerializeField] private List<Unit> _players;

    [SerializeField] private List<UnitStats> _enemyTypes;
    
    [SerializeField] private Phases currentPhase;
    private GameObject _objective;
    private SpawnEnemies _spawnEnemies;
    private CameraFollow _camFollow;
    private bool spawnedPlayers;
    [SerializeField]
    private int maxEnemies = 12;

    private PlayerInteractions _pi;

    public TurnSystem(List<Unit> enemies)
    {
        this._enemies = enemies;
    }

    public enum Phases
    {
        Movement,
        EnemyMovement,
        EnemySpawn,
        Resolution
    }

    public List<Unit> enemies => _enemies;
    public List<Unit> players => _players;

    // Register listeners
    private void OnEnable()
    {
        EventManager.StartListening("EnemyDied", UpdateEnemyList);
    }

    private void OnDisable()
    {
        EventManager.StopListening("EnemyDied", UpdateEnemyList);
    }

    // Start is called before the first frame update
    private void Start()
    {
        _pi = GetComponent<PlayerInteractions>();
        QualitySettings.vSyncCount = 1;
        _objective = GameObject.FindWithTag("Objective");
        if (_objective == null)
        {
            Debug.LogError("Objective missing from level");

        }

        _camFollow = FindObjectOfType<CameraFollow>();
        
        // First phase
        currentPhase = Phases.Movement;

        // Spawn enemies
        _spawnEnemies = GetComponent<SpawnEnemies>();
        _spawnEnemies.Spawn(GetEnemiesSpawned(), _enemyTypes);

        UpdateEnemyList();

        // Reset command history
        CommandInvoker.ResetHistory();
    }

    public Phases GetCurrentPhase()
    {
        return currentPhase;
    }

    public void SetNextPhase()
    {
        if (currentPhase == Phases.Resolution)
        {
            currentPhase = Phases.Movement;
        }
        else
        {
            currentPhase = currentPhase + 1;
        }
    }

    private int GetEnemiesSpawned()
    {
        return Random.Range(minEnemiesSpawnedPerTurn, maxEnemiesSpawnedPerTurn + 1);
    }

    private void UpdateEnemyList()
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

    private void UpdatePlayerList()
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
    
    // Update is called once per frame
    private void Update()
    {
        TurnManager();
        UpdateEnemyList();
        UpdatePlayerList();
    }

    private void TurnManager()
    {
        switch (currentPhase)
        {
            case (Phases.Movement):
                _pi.UpdateMovement();
                break;
            case (Phases.EnemyMovement):
                _camFollow.followEnabled = true;
                _pi.UpdateEnemyMovement();
                break;
            case (Phases.EnemySpawn):
                _camFollow.followEnabled = false;
                if (maxEnemies > _enemies.Count)
                {
                    // Do not spawn more than maxEnemies
                    int enemiesToSpawn = Mathf.Clamp(maxEnemies - _enemies.Count, 0, maxEnemiesSpawnedPerTurn);
                    _spawnEnemies.Spawn(enemiesToSpawn, _enemyTypes);
                    UpdateEnemyList();
                }
                SetNextPhase();
                break;
            case (Phases.Resolution):

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
                        else if (SceneManager.GetActiveScene().name == "Level2")
                        {
                            SceneManager.LoadScene("Level3");
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
                if (playersAlive == 0)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    break;
                }
                
                SetNextPhase();
                _pi.UpdateMovementPoints();
                break;
            default:
                break;
        }
    }
}
