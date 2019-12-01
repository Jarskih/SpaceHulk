using Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Unit : MonoBehaviour
{
    public GameObject blood;
    public UnitStats unitStats;
    public UnitType unitType;
    public SetActive setActive;
    public APrules APrules;
    public StatsListVariable enemyTargets;
    public int actionPoints;
    public GameObject currentNode;
    private Vector3 _targetPos;
    private ChangeSprite _changeSprite;
    private IMove _movement;
    private ShootAction _shootAction;
    private int _health;
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

    private void Start()
    {
        setActive = GetComponentInChildren<SetActive>();
        _changeSprite = GetComponentInChildren<ChangeSprite>();
        _turnSystem = FindObjectOfType<TurnSystem>();
        _movement = GetComponent<IMove>();
        _shootAction = GetComponent<ShootAction>();
        _tileMap = FindObjectOfType<Tilemap>();
        StartCoroutine(SaveCurrentTile());
        startingPos = transform.position;
        if (unitType == UnitType.Marine)
        {
            _health = unitStats.maxHealth;
            actionPoints = unitStats.maxAP;
        }
        else
        {
            _health = 1;
        }
    }

    private void Update()
    {
        //transform.position = TargetPos;
        transform.position = Vector3.MoveTowards(transform.position, TargetPos, 0.3f);
        //UpdateCurrentTile(targetPos);
    }

    public void ReturnToIdle()
    {
        _currentState = UnitState.Idle;
        _shootAction?.ClearTargetingTiles();
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
        if (Vector3.Distance(transform.position, TargetPos) > 0.05f)
        {
            return;
        }

        if (currentState == UnitState.Idle)
        {
            _movement.Act();
            _shootAction?.UpdateAmmo();
            _shootAction.ClearTargetingTiles();
            enemyTargets.list.Clear();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Aim();
                return;
            }
        }

        if (currentState == UnitState.Shooting)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                _shootAction.NextTarget();
            }
            
            _shootAction.Act(enemies);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Shoot();
                return;
            }
        }
    }

    public void UpdateMovementPoints(int change)
    {
        actionPoints = Mathf.Clamp(actionPoints + change, 0, unitStats.maxAP);
    }

    public void SetActionPoints(int AP)
    {
        actionPoints = Mathf.Clamp(AP, 0, unitStats.maxAP);
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        Mathf.Max(_health, 0);
        BloodCreator.CreateBlood(currentNode.transform.position, blood);
        if (_health > 0)
        {
            return;
        }
        
        if (unitType == UnitType.Marine)
        {
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

    public void GainHealth(int value)
    {
        _health += value;
        Mathf.Clamp(_health, 0, 5);
    }

    public void Reload()
    {
        if (actionPoints >= GetWeaponStats().reloading)
        {
            _shootAction.Reload();
        }
        else
        {
            EventManager.TriggerEvent("Negative");
        }
    }

    public void Aim()
    {
        if (actionPoints >= GetWeaponStats().actionCost)
        {
            // Check all targets and target first one in list
            _shootAction.FindTargets();
            if (_shootAction.TargetEnemy())
            {
                _currentState = UnitState.Shooting;
            }
            else
            {
                EventManager.TriggerEvent("Negative");
                _currentState = UnitState.Idle;
            }
        }
        else
        {
            EventManager.TriggerEvent("Negative");
            _currentState = UnitState.Idle;
        }
    }

    public WeaponStats GetWeaponStats()
    {
        return _shootAction.weaponStats;
    }

    public void Shoot()
    {
        if (currentState == UnitState.Shooting)
        {
            if (actionPoints >= GetWeaponStats().actionCost)
            {
                _shootAction.Shoot();
            }
            else
            {
                EventManager.TriggerEvent("Negative");
            }
        }
        else
        {
            EventManager.TriggerEvent("Negative");
        }
    }

    public bool CanShoot()
    {
        return actionPoints >= GetWeaponStats().actionCost && enemyTargets.list.Count > 0;
    }

    public bool CanReload()
    {
        return actionPoints >= GetWeaponStats().reloading && _shootAction.ammoUI.Value != GetWeaponStats().maxAmmo;
    }
}
