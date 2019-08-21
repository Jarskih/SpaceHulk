using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.Tilemaps;

public class Unit : MonoBehaviour
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

    [SerializeField] private UnitState currentState;
   public Vector3 startingPos;

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
        startingPos = targetPos;
    }

    void Update()
    {
        //transform.position = targetPos;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.3f);
        SaveCurrentTile();
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

    public void Actions(IEnumerable<Unit> enemies)
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

    public void TakeDamage(int damage)
    {
        _health -= damage;
        Mathf.Clamp(_health, 0, 5);
        if (unitType == UnitType.Marine)
        {
            if (_health <= 0)
            {
                EventManager.TriggerEvent("Wounded");
                //isDead = true;
                //Destroy(gameObject);
            }
        }
        else
        {
            EventManager.TriggerEvent("EnemyDied");
            Destroy(gameObject);
        }
    }

    private IEnumerator RemovePlayer()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    public void GainHealth(int value)
    {
        _health += value;
        Mathf.Clamp(_health, 0, 5);
    }
}
