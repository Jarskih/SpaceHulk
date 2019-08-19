using Interfaces;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Movement : MonoBehaviour, IMove
{
    private Tilemap _tileMap;
    private Stats _stats;
    private TilemapController _tilemapController;

    void Start()
    {
        _tileMap = FindObjectOfType<Tilemap>();
        _tilemapController = FindObjectOfType<TilemapController>();
        _stats = GetComponent<Stats>();
    }

    public void Act()
    {
        ListenToInput();
    }

    void ListenToInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            CanMove(transform.up);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.S))
        {     
            CanMove(-transform.up);
        } else if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            Turn(90);
            
        } else if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            Turn(-90);
        }
    }

    void CanMove(Vector3 direction)
    {
        Vector3 pos = _stats.targetPos + direction;
        Vector3Int intPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), 0);
        
        // Check if tile is not a floor
        if (!_tilemapController.IsFloor(intPos))
        {
            return;
        }
        
       // If tile is occupied by a unit move two tiles
       if (_tilemapController.CheckIfTileOccupied(intPos, _stats))
       {
           // if unit type is not friendly unit don't allow move through
           if (_tilemapController.GetUnitFromTile(intPos).unitType != _stats.unitType)
           {
               return;
           }
           
           if (direction == transform.up)
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
            EventManager.TriggerEvent("Negative");
            return;
        }

        if (direction == transform.up)
        {
            ICommand c = new MoveForwardCommand(_stats);
            CommandInvoker.AddCommand(c);
        }
        else if(direction == -transform.up)
        {
            ICommand c = new MoveBackwardCommand(_stats);
            CommandInvoker.AddCommand(c);
        }
        else
        {
            EventManager.TriggerEvent("Negative");
        }
    }

    void MoveTwoTiles(Vector3 direction)
    {
      if (_stats.actionPoints <= 2)
      {
          Debug.Log("Not enough AP to jump over");
          EventManager.TriggerEvent("Negative");
          return;
      }
         
      Vector3 pos = _stats.targetPos + direction*2;
      Vector3Int intPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), (int)pos.z);
         
      // Check if a tile 2 steps away is walkable (jump over)
      var colliderType = _tileMap.GetColliderType(intPos);
      if (colliderType == Tile.ColliderType.None)
      {
          // if tile is not occupied by unit allow movement
          if (!_tilemapController.CheckIfTileOccupied(intPos, _stats))
          {
              ICommand c = new DoubleMoveCommand(_stats);
              CommandInvoker.AddCommand(c);
          }
      }

        Debug.Log("Someone on the way");
        EventManager.TriggerEvent("Negative");
    }

    void Turn(int angle)
    {
        ICommand c = new TurnCommand(_stats, angle);
        CommandInvoker.AddCommand(c);
    }
}
