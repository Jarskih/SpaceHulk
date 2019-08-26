using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.Tilemaps;

public class Unit : MonoBehaviour
{
    public IntVariable maxAP;
    public UnitType unitType;
    public SetActive setActive;
    public APrules APrules;
    public StatsListVariable enemyTargets;
    public int actionPoints;
    public GameObject currentNode;
    private Vector3 _targetPos;
    private ChangeSprite _changeSprite;
    private IMove _movement;
    private ComplexActions _complexActions;
    private int _health = 1;
    private Tilemap _tileMap;
    private TurnSystem _turnSystem;
   

    public enum UnitType
    {
        Alien,
        Marine
    }

    [SerializeField] private UnitState _currentState;
    public Vector3 startingPos;

    public enum UnitState
    {
        Idle,
        Shooting
    }

    public int health => _health;
    public UnitState currentState => _currentState;

    public Vector3 TargetPos => currentNode ? currentNode.transform.position : transform.position;


    void Start()
    {
        setActive = GetComponentInChildren<SetActive>();
        _changeSprite = GetComponentInChildren<ChangeSprite>();
        _turnSystem = FindObjectOfType<TurnSystem>();
        _movement = GetComponent<IMove>();
        _complexActions = GetComponent<ComplexActions>();
        _tileMap = FindObjectOfType<Tilemap>();
        StartCoroutine(SaveCurrentTile());
        startingPos = transform.position;

    }

    void Update()
    {
        transform.position = TargetPos;
        //transform.position = Vector3.MoveTowards(transform.position, TargetPos, 0.3f);
        //UpdateCurrentTile(targetPos);
    }

    public void ReturnToIdle()
    {
        _currentState = UnitState.Idle;
        enemyTargets.list.Clear();
        _complexActions?.ClearTargetingTiles();
    }

    private IEnumerator SaveCurrentTile()
    {
        yield return new WaitForSeconds(0.1f);
        UpdateCurrentTile(GetCurrentTilePos());
    }
    
    private void UpdateCurrentTile(Vector3Int newPos)
    {
        var tileObject = _tileMap.GetInstantiatedObject(newPos);
        if (tileObject)
        {
            currentNode = tileObject;
        }
        else
        {
            //Debug.LogWarning("no tileobject at: " + newPos);
        }
    }

    public void UpdateCurrentTile(Vector3 newPos)
    {
        UpdateCurrentTile(new Vector3Int(Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.y), 0));
    }
    
    public Vector3Int GetCurrentTilePos()
    {
        return new Vector3Int(Mathf.RoundToInt(TargetPos.x), Mathf.RoundToInt(TargetPos.y), 0);
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
        _currentState = state;
    }

    public void Actions(IEnumerable<Unit> enemies)
    {
        // Wait for unit to move before allowing new orders
        if (Vector3.Distance(transform.position, TargetPos) > 0.05f) return;
        
        if (currentState == UnitState.Idle)
        {
            _movement.Act();
            _complexActions?.UpdateAmmo();
            enemyTargets.list.Clear();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Aim();
                return;
            }
        }

        if (currentState == UnitState.Shooting)
        {
            _complexActions.Act(enemies);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Shoot();
                return;
            }
        }
    }
    
    public void UpdateMovementPoints(int change)
    {
        actionPoints = Mathf.Clamp(actionPoints + change, 0, maxAP.Value);
    }

    public void SetActionPoints(int AP)
    {
        actionPoints = Mathf.Clamp(AP, 0, maxAP.Value);
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        Mathf.Clamp(_health, 0, 5);
        if (unitType == UnitType.Marine)
        {
            if (_health > 0) return;
            
            EventManager.TriggerEvent("Die");
            foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
            {
                spriteRenderer.enabled = false;
            }
        }
        else
        {
            EventManager.TriggerEvent("EnemyDie");
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

    public void Reload()
    {
        if (actionPoints >= APrules.reloading)
        {
            _complexActions.Reload();
        }
    }
    
    public void Aim()
    {
        if (actionPoints >= APrules.playerAttacking)
        {
            _complexActions.FindTargets();
            _currentState = UnitState.Shooting;
        }
        else
        {
            EventManager.TriggerEvent("Negative");
            _currentState = UnitState.Idle;
        }
    }

    public WeaponStats GetWeaponStats()
    {
        return _complexActions.weaponStats;
    }

    public void Shoot()
    {
        if (currentState == UnitState.Shooting)
        {
            _complexActions.Shoot();
        }
    }
}
