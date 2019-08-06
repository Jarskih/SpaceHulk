using Interfaces;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyMovement : MonoBehaviour, IMove
{
    private Tilemap _tileMap;
    private Stats _stats;
    
    void Start()
    {
        _stats = GetComponent<Stats>();
        _tileMap = GameObject.FindObjectOfType<Tilemap>();
    }
    
    public void Act()
    {
        ListenToInput();
    }
    void ListenToInput()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (CanMove(transform.up) == (true, false))
            {
                ICommand c = new EnemyMoveCommand(transform.up, _stats);
                CommandInvoker.AddCommand(c);
            } else if(CanMove(transform.up) == (false, true))
            {
                ICommand c = new EnemyDoubleMoveCommand(transform.up, _stats);
                CommandInvoker.AddCommand(c);
            }
        }
        
        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (CanMove(-transform.up) == (true, false))
            {
                ICommand c = new EnemyMoveCommand(-transform.up, _stats);
                CommandInvoker.AddCommand(c);
            } else if(CanMove(-transform.up) == (false, true))
            {
                ICommand doubleMoveCommand = new EnemyDoubleMoveCommand(transform.up, _stats);
                doubleMoveCommand.Execute();
            }
        }
        
        if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) 
        {
            if (CanMove(-transform.right) == (true, false))
            {
                ICommand c = new EnemyMoveCommand(-transform.right, _stats);
                CommandInvoker.AddCommand(c);
            } else if(CanMove(-transform.up) == (false, true))
            {
                ICommand c = new EnemyDoubleMoveCommand(-transform.right, _stats);
                CommandInvoker.AddCommand(c);
            }
        }
        
        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) 
        {
            if (CanMove(transform.right) == (true, false))
            {
                ICommand c = new EnemyMoveCommand(transform.right, _stats);
                CommandInvoker.AddCommand(c);
            } else if(CanMove(transform.up) == (false, true))
            {
                ICommand c = new EnemyDoubleMoveCommand(transform.up, _stats);
                CommandInvoker.AddCommand(c);
            }
        }
    }

    (bool,bool) CanMove(Vector3 direction)
    {
        Vector3 pos = _stats._targetPos + direction;
        Vector3Int intPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), 0);
        
        // If tile is occupied by a unit
        if (!_stats.CheckIfTileIsFree(intPos))
        {
            if (direction == Vector3.up)
            {
                return MoveTwoTiles(direction);
            }
        }

        return MoveOneTile(direction, intPos);
    }
    
    (bool,bool) MoveOneTile(Vector3 direction, Vector3Int intPos)
    {
       
        // Check if tile is walkable
        var colliderType = _tileMap.GetColliderType(intPos);
        if (colliderType != Tile.ColliderType.None)
        {
            Debug.Log("Tile not walkable");
            return (false, false);
        }

        return (true, false);
    }

    (bool,bool) MoveTwoTiles(Vector3 direction)
    {
        if (_stats.actionPoints <= 2)
        {
            Debug.Log("Not enough AP to jump over");
            return (false, false);
        }
         
        Vector3 pos = _stats._targetPos + direction*2;
        Vector3Int intPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), (int)pos.z);
         
        // Check if a tile 2 steps away is walkable (jump over)
        var colliderType = _tileMap.GetColliderType(intPos);
        if (colliderType == Tile.ColliderType.None)
        {
            if (_stats.CheckIfTileIsFree(intPos))
            {
                return (false, true);
            }
        }

        Debug.Log("Someone on the way");
        return (false, false);
    }

    bool Move(Vector3 direction)
    {
        Vector3 pos = _stats._targetPos + direction;
        Vector3Int intPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), 0);
        var colliderType = _tileMap.GetColliderType(intPos);
        if (colliderType == Tile.ColliderType.None)
        {
            if (!_stats.CheckIfTileIsFree(intPos))
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
                    pos = _stats._targetPos + direction * 2;
                    intPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), (int) pos.z);
                    colliderType = _tileMap.GetColliderType(intPos);
                    if (colliderType == Tile.ColliderType.None)
                    {
                        if (_stats.CheckIfTileIsFree(intPos) && _stats.actionPoints >= 2)
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
}
