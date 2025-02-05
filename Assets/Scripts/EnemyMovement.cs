﻿using Interfaces;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyMovement : MonoBehaviour, IMove
{
    [SerializeField] private float timeBetweenDecisions;
    [SerializeField] private float timeSinceDecision = 0;
    [SerializeField] private bool isAi;
    private TilemapController _tilemapController;
    private Tilemap _tileMap;
    private Unit _unit;
    private SpaceHulkAgent _agent;
    [SerializeField] private bool isTraining;

    private void Start()
    {
        _unit = GetComponent<Unit>();
        _tileMap = FindObjectOfType<Tilemap>();
        _tilemapController = FindObjectOfType<TilemapController>();
        _agent = GetComponent<SpaceHulkAgent>();
    }

    private void Update()
    {

        if (isTraining)
        {
            Act();
        }
    }

    public void Act()
    {
        if (isAi)
        {
            timeSinceDecision += Time.deltaTime;
            if (timeSinceDecision > timeBetweenDecisions)
            {
                _agent.RequestDecision();
                timeSinceDecision = 0;
            }
        }
        else
        {
            ListenToInput();
        }
    }

    private void ListenToInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            CanMove(transform.up);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            CanMove(-transform.up);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            CanMove(-transform.right);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            CanMove(transform.right);
        }
    }

    public void CanMove(Vector3 direction)
    {
        Vector3 pos = _unit.TargetPos + direction;
        Vector3Int intPos = new Vector3Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), 0);

        // Check if tile is not a floor
        if (!_tilemapController.IsWalkable(intPos, _unit.GetTileMap()))
        {
            return;
        }

        // If tile is occupied by a unit
        if (_tilemapController.CheckIfTileOccupied(intPos, _unit))
        {
            // if unit type is not friendly unit attack it
            if (_tilemapController.TileOccupiedByMarine(intPos, _unit))
            {
                if (isTraining)
                {
                    GetComponent<SpaceHulkAgent>().killedPlayer = true;
                    return;
                }
                else
                {
                    GetComponent<SpaceHulkAgent>().killedPlayer = true;
                    AttackUnit(intPos);
                    return;
                }
            }

            if (_tilemapController.TileOccupiedByAlien(intPos, _unit))
            {
                MoveTwoTiles(direction);
                return;
            }
        }

        MoveOneTile(direction, intPos);
    }

    private void AttackUnit(Vector3Int intPos)
    {
        var enemy = _tilemapController.GetUnitFromTile(intPos);
        ICommand c = new MeleeAttackCommand(_unit, enemy);
        CommandInvoker.AddCommand(c);
    }

    private void MoveOneTile(Vector3 direction, Vector3Int intPos)
    {

        // Check if tile is walkable
        var colliderType = _tileMap.GetColliderType(intPos);
        var door = _tileMap.GetInstantiatedObject(intPos).GetComponent<IOpenable>() as DoorNode;
        if (colliderType == Tile.ColliderType.Grid && door == null)
        {
            Debug.Log("Tile not walkable");
            return;
        }

        ICommand c = new EnemyMoveCommand(direction, _unit);
        CommandInvoker.AddCommand(c);
    }

    private void MoveTwoTiles(Vector3 direction)
    {
        if (_unit.actionPoints < 2)
        {
            Debug.Log("Not enough AP to jump over");
            return;
        }

        Vector3 pos = _unit.TargetPos + direction * 2;
        Vector3Int intPos = new Vector3Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), (int)pos.z);

        // Check if a tile 2 steps away is walkable (jump over)
        if (_tilemapController.IsWalkable(intPos, _tileMap) || _tileMap.GetInstantiatedObject(intPos).GetComponent<IOpenable>() != null)
        {
            // If tile is not occupied move to new tile
            if (!_tilemapController.CheckIfTileOccupied(intPos, _unit))
            {
                ICommand c = new EnemyDoubleMoveCommand(direction, _unit);
                CommandInvoker.AddCommand(c);
            }
            else
            {
                Debug.Log("EnemyMove: Someone on the way");
            }
        }
        else
        {
            Debug.Log("EnemyMove: Not walkable");
        }
    }

    /*
    bool Move(Vector3 direction)
    {
        Vector3 pos = _stats.targetPos + direction;
        Vector3Int intPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), 0);
        var colliderType = _tileMap.GetColliderType(intPos);
        if (colliderType == Tile.ColliderType.None)
        {
            if (!_stats.CheckIfTileOccupied(intPos))
            {
                var enemyStats = _stats.GetUnitFromTile(intPos);
                if (enemyStats != null && enemyStats.unitType != _stats.unitType)
                {
                    enemyStats.TakeDamage(1);
                    _stats.UpdateMovementPoints(-_stats.APrules.enemyAttacking);
                    return true;
                }
                else
                {
                    pos = _stats.targetPos + direction * 2;
                    intPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), (int) pos.z);
                    colliderType = _tileMap.GetColliderType(intPos);
                    if (colliderType == Tile.ColliderType.None)
                    {
                        if (_stats.CheckIfTileOccupied(intPos) && _stats.actionPoints >= 2)
                        {
                            _stats.UpdateCurrentTile(intPos);
                            _stats.UpdateMovementPoints(-_stats.APrules.moving * 2);
                            return true;
                        }
                        else
                        {
                            Debug.Log("Someone on the way");
                            return false;
                        }
                    }
                }
            }
            else
            {
                _stats.UpdateCurrentTile(intPos);
                _stats.UpdateMovementPoints(-_stats.APrules.moving);
                return true;
            }
        }
        return false;
    }
    */
}
