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
    public Vector3 _targetPos;
    
    private IMove _movement;
    private ComplexActions _complexActions;
    private int _health = 1;
    private Tilemap _tileMap;

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

    public int Health => _health;

    void Start()
    {
        _targetPos = transform.position;
        _movement = GetComponent<IMove>();
        _complexActions = GetComponent<ComplexActions>();
        _tileMap = GameObject.FindObjectOfType<Tilemap>();
        StartCoroutine(SaveCurrentTile());
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, 0.3f);
    }

    private IEnumerator SaveCurrentTile()
    {
        yield return new WaitForSeconds(0.1f);
        UpdateCurrentTile(GetCurrentTilePos());
    }

    public Vector3Int GetCurrentTilePos()
    {
        var position = transform.position;
        return new Vector3Int(Mathf.FloorToInt(_targetPos.x), Mathf.FloorToInt(_targetPos.y), 0);
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
        if (Vector3.Distance(transform.position, _targetPos) > 0.05f) return;
        
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
        var currentTile = _tileMap.GetInstantiatedObject(GetCurrentTilePos());
        if (currentTile != null)
        {
            currentTile.GetComponent<Node>().unitOnNode = null;
        }
            
        var tileObject = _tileMap.GetInstantiatedObject(newPos);
        tileObject.GetComponent<Node>().unitOnNode = this;
    }

    public void UpdateCurrentTile(Vector3 newPos)
    {
        UpdateCurrentTile(new Vector3Int((int)newPos.x, (int)newPos.y, 0));
    }

    public bool CheckIfTileIsFree(Vector3Int newPos)
    {
        var unit =_tileMap.GetInstantiatedObject(newPos)?.GetComponent<Node>()?.unitOnNode;
        return unit == null || unit._health <= 0;
    }

    public Stats GetUnitFromTile(Vector3Int pos)
    {
        pos.z = 0;
        var obj = _tileMap.GetInstantiatedObject(pos);
        var unit = obj.GetComponent<Node>()?.unitOnNode;
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
}
