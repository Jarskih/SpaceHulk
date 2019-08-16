using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Stats : MonoBehaviour
{
    public APrules APrules;
    public int actionPoints;
    public bool isDead;
    public StatsListVariable enemyTargets;
    public Vector3 targetPos;
    public Node currentNode;
    private ChangeSprite _changeSprite;
    private IMove _movement;
    private ComplexActions _complexActions;
    private int _health = 1;
    private Tilemap _tileMap;
    private TurnSystem _turnSystem;
    public UnitType unitType;
    public enum UnitType
    {
        Alien,
        Marine
    }

   private UnitState currentState;

    public enum UnitState
    {
        Idle,
        Shooting
    }

    public int health => _health;

    void Start()
    {
        _changeSprite = GetComponentInChildren<ChangeSprite>();
        _turnSystem = FindObjectOfType<TurnSystem>();
        targetPos = transform.position;
        _movement = GetComponent<IMove>();
        _complexActions = GetComponent<ComplexActions>();
        _tileMap = GameObject.FindObjectOfType<Tilemap>();
        StartCoroutine(SaveCurrentTile());
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.3f);
    }

    private IEnumerator SaveCurrentTile()
    {
        yield return new WaitForSeconds(0.1f);
        UpdateCurrentTile(GetCurrentTilePos());
    }
    
    public Vector3Int GetCurrentTilePos()
    {
        return new Vector3Int(Mathf.FloorToInt(targetPos.x), Mathf.FloorToInt(targetPos.y), 0);
    }

    public Tilemap GetTileMap()
    {
        return _tileMap;
    }

    public void Movement()
    {
         _movement.Act();
    }

    public void ChangeState(UnitState state)
    {
        currentState = state;
    }

    public void Actions(IEnumerable<Stats> enemies)
    {
        // Wait for unit to move before allowing new orders
        if (Vector3.Distance(transform.position, targetPos) > 0.05f) return;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (actionPoints >= 2)
            {
                currentState = UnitState.Shooting;
            }
            else
            {
             EventManager.TriggerEvent("Negative");   
            }
        }
        
        if (currentState == UnitState.Idle)
        {
            _movement.Act();
        }

        if (currentState == UnitState.Shooting)
        {
            _complexActions.Act(enemies);
        }
    }

    public void UpdateMovementPoints(int change)
    {
        actionPoints = Mathf.Clamp(actionPoints + change, 0, 6);
    }

    public void UpdateCurrentTile(Vector3Int newPos)
    {
        var tileObject = _tileMap.GetInstantiatedObject(newPos);
        if (tileObject)
        {
            currentNode = tileObject.GetComponent<Node>();
        }
        else
        {
            Debug.LogWarning("no tileobject at: " + newPos);
        }
    }

    public void UpdateCurrentTile(Vector3 newPos)
    {
        UpdateCurrentTile(new Vector3Int(Mathf.FloorToInt(newPos.x), Mathf.FloorToInt(newPos.y), 0));
    }

    public bool CheckIfTileOccupied(Vector3Int newPos)
    {
        var tile = _tileMap.GetInstantiatedObject(newPos);

        if (tile == null)
        {
            Debug.Log("Tile outside of game area");
            return true;
        }
        
        Stats unit = null;
        var node = tile.GetComponent<Node>();
        
        var enemies = _turnSystem.enemies;
        foreach (var enemy in enemies)
        {
            // Dont check current player
            if (enemy == this) continue;
            if (enemy.currentNode == node)
            {
                if (enemy.health > 0)
                {
                    unit = enemy;
                }
            }
        }

        if (unit == null)
        {
            var players = _turnSystem.players;
            foreach (var player in players)
            {
                // Dont check current player
                if (player == this) continue;
                if (player.currentNode == node)
                {
                    if (player.health > 0)
                    {
                        unit = player;
                    }
                }
            }
        }
        
        return unit != null && unit.health > 0;
    }

    public Stats GetUnitFromTile(Vector3Int pos)
    {
        var tile = _tileMap.GetInstantiatedObject(pos);

        if (tile == null)
        {
            Debug.Log("Tile outside of game area");
            return null;
        }
        
        var node = tile.GetComponent<Node>();

        if (node == null)
        {
            Debug.LogWarning("Node not found");
            return null;
        }
        
        Stats unit = null;
        var enemies = _turnSystem.enemies;
        foreach (var enemy in enemies)
        {
            if (enemy.currentNode == node)
            {
                if (enemy.health > 0)
                {
                    unit = enemy;
                }
            }
        }

        if (unit == null)
        {
            var players = _turnSystem.players;
            foreach (var player in players)
            {
                if (player.currentNode == node)
                {
                    unit = player;
                }
            }
        }
        
        return unit;
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        Mathf.Clamp(_health, 0, 5);
        if (unitType == UnitType.Marine)
        {
            if (_health <= 0)
            {
                EventManager.TriggerEvent("Wounded");
                isDead = true;
            }
        }
        else
        {
            EventManager.TriggerEvent("EnemyDied");
            Destroy(gameObject);
        }
    }

    public void GainHealth(int value)
    {
        _health += value;
        Mathf.Clamp(_health, 0, 5);
    }
}
