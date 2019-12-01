using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    public IntVariable actionPoints;
    public IntVariable currentWeaponDamage;
    public IntVariable currentActionCost;
    public IntVariable currentMaxAmmo;

    private SetPlayerActive _setPlayerActive;
    [SerializeField] private Unit _activeUnit;
    private TurnSystem _turnSystem;
    private CameraFollow _cameraFollow;
    
    [SerializeField] private int activeUnitIndex = 0;
    [SerializeField] private int _activeEnemyIndex = 0;
    
    public Unit activeUnit => _activeUnit;
    
    // Timer for limiting AI enemy turn time
    private float _enemyTimer = 0;
    [SerializeField] private float _enemyMaxTime;
    
    // Start is called before the first frame update
    void Start()
    {
        _cameraFollow = GetComponent<CameraFollow>();
        _setPlayerActive = GetComponent<SetPlayerActive>();
        _turnSystem = GetComponent<TurnSystem>();
        _activeUnit = _turnSystem.players[0];
        
        UpdateMovementPoints();
        FollowPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        //FollowPlayer();
        UpdateUI();
    }
    
    public void FollowPlayer()
    {
        if (_activeUnit != null)
        {
            _cameraFollow.SetTarget(_activeUnit.transform.position);
        }

        StartCoroutine(DisableFollow());
    }

    IEnumerator DisableFollow()
    {
        _cameraFollow.followEnabled = true;
        yield return new WaitForSeconds(0.2f);
        _cameraFollow.followEnabled = false;
    }
    
    private void UpdateUI()
    {
        _cameraFollow.SetTarget(_activeUnit.TargetPos);

        if (_turnSystem.GetCurrentPhase() == TurnSystem.Phases.Movement)
        {
            if (_activeUnit)
            {
                _setPlayerActive.SetUnitActive(_turnSystem.players, _activeUnit);
                actionPoints.Value = _activeUnit.actionPoints;
            }
        }
        else
        {
            _setPlayerActive.DisableActiveUnits(_turnSystem.players);
            if (_activeUnit)
            {
                actionPoints.Value = _activeUnit.actionPoints;
            }
        }
    }
    
    private bool AllPlayersActed()
    {
        foreach (var player in _turnSystem.players)
        {
            if (player.actionPoints > 0)
            {
                return false;
            }
        }
        return true;
    }

    public void UpdateMovement()
    {
        if (_activeUnit.health <= 0)
        {
            SetNextUnit();
        }
        
        if (_activeUnit == null || _activeUnit.unitType == Unit.UnitType.Alien)
        {
            if (_turnSystem.players.Count > 0)
            {
                activeUnitIndex = -1;
                SetNextUnit();
            }
            else
            {
                return;
            }
        }

        _activeUnit.Actions(_turnSystem.enemies);

        _enemyTimer = 0;
        
        // Update UI variables
        currentWeaponDamage.Value = _activeUnit.GetWeaponStats().damage;
        currentActionCost.Value = _activeUnit.GetWeaponStats().actionCost;
        currentMaxAmmo.Value = _activeUnit.GetWeaponStats().maxAmmo;
    }

    public void UpdateEnemyMovement()
    {
        _enemyTimer += Time.deltaTime;

        if (_enemyTimer > _enemyMaxTime)
        {
            NextEnemy();
            _enemyTimer = 0;
        }

        if (_activeUnit == null || _activeUnit.unitType == Unit.UnitType.Marine)
        {
            if (_turnSystem.enemies.Count > 0)
            {
                _activeUnit = _turnSystem.enemies[0];
                UpdateMovementPoints();
            }
            else
            {
                NextPhase();
                return;
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
    }
    
    public void NextPlayer()
    {
        if (_turnSystem.GetCurrentPhase() == TurnSystem.Phases.Movement)
        {
            SetNextUnit();
        }
    }
    
    public void UpdateMovementPoints()
    {
        if (_turnSystem.GetCurrentPhase() == TurnSystem.Phases.Movement)
        {
            foreach (var player in _turnSystem.players)
            {
                if (player.health > 0)
                {
                    player.SetActionPoints(player.unitStats.maxAP);
                }
            }
        }

        if (_turnSystem.GetCurrentPhase() == TurnSystem.Phases.EnemyMovement)
        {
            if (_activeUnit == null || _activeUnit.unitType == Unit.UnitType.Marine)
            {
                if (_turnSystem.enemies.Count > 0)
                {
                    _activeUnit = _turnSystem.enemies[0];
                }
                else
                {
                    NextPhase();
                }
            }

            foreach (var enemy in _turnSystem.enemies)
            {
                enemy.SetActionPoints(enemy.unitStats.maxAP);
            }
        }
    }
    
    public void SetNextUnit()
    {
        _activeUnit.ReturnToIdle();
        CommandInvoker.ResetHistory();
        activeUnitIndex += 1;

        if (activeUnitIndex >= _turnSystem.players.Count)
        {
            activeUnitIndex = 0;
        }

        _activeUnit = _turnSystem.players[activeUnitIndex];

        if (_activeUnit.health <= 0)
        {
            SetNextUnit();
        }
        
        FollowPlayer();
    }

    private void NextEnemy()
    {
        _activeEnemyIndex = _activeEnemyIndex + 1;
        if (_activeEnemyIndex >= _turnSystem.enemies.Count)
        {
            NextPhase();
            _activeEnemyIndex = 0;
            //Debug.Log("All enemies moved, setting new phase: " + currentPhase);
        }
        else
        {
            //Debug.Log("Next alien turn");
            _activeUnit = _turnSystem.enemies[_activeEnemyIndex];
        }
    }
    
    public void SetActiveUnit(Unit unit)
    {
        _activeUnit.ReturnToIdle();
        _activeUnit = unit;
        activeUnitIndex = 0;
    }

    private void NextPhase()
    {
        activeUnit.ReturnToIdle();
        _turnSystem.SetNextPhase();
        UpdateMovementPoints();
    }
}
