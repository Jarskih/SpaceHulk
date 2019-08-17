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
       // ListenToInput();
    }
    void ListenToInput()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            //CanMove(transform.up);
        }
        
        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            //CanMove(-transform.up);
        }
        
        if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            //CanMove(-transform.right);
        }
        
        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            //CanMove(transform.right);
        }
    }

    public void CanMove(Vector3 direction)
    {
        Vector3 pos = _stats.targetPos + direction;
        Vector3Int intPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), 0);
        
        // If tile is occupied by a unit
        if (_stats.CheckIfTileOccupied(intPos))
        {
            // if unit type is not friendly unit attack it
            if (_stats.GetUnitFromTile(intPos).unitType != _stats.unitType)
            {
                AttackUnit(intPos);
                return;
            }
            
            if (direction == Vector3.up)
            {
                MoveTwoTiles(direction);
                return;
            }
        }

        MoveOneTile(direction, intPos);
    }
    
    private void AttackUnit(Vector3Int intPos)
    {
        var enemy = _stats.GetUnitFromTile(intPos);
        ICommand c = new MeleeAttackCommand(_stats, enemy);
        CommandInvoker.AddCommand(c);
    }
    
    void MoveOneTile(Vector3 direction, Vector3Int intPos)
    {
       
        // Check if tile is walkable
        var colliderType = _tileMap.GetColliderType(intPos);
        if (colliderType != Tile.ColliderType.None)
        {
            Debug.Log("Tile not walkable");
            return;
        }
        
        ICommand c = new EnemyMoveCommand(direction, _stats);
        CommandInvoker.AddCommand(c);
    }

    void MoveTwoTiles(Vector3 direction)
    {
        if (_stats.actionPoints <= 2)
        {
            Debug.Log("Not enough AP to jump over");
            return;
        }
         
        Vector3 pos = _stats.targetPos + direction*2;
        Vector3Int intPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), (int)pos.z);
         
        // Check if a tile 2 steps away is walkable (jump over)
        var colliderType = _tileMap.GetColliderType(intPos);
        if (colliderType == Tile.ColliderType.None)
        {
            // If tile is not occupied move to new tile
            if (!_stats.CheckIfTileOccupied(intPos))
            {
                ICommand c = new EnemyDoubleMoveCommand(direction, _stats);
                CommandInvoker.AddCommand(c);
            }
        }

        Debug.Log("EnemyMove: Someone on the way");
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
