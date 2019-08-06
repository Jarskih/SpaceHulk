using Interfaces;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Movement : MonoBehaviour, IMove
{
    private Tilemap _tileMap;
    private Stats _stats;

    void Start()
    {
        _tileMap = FindObjectOfType<Tilemap>();
        _stats = GetComponent<Stats>();
    }

    public void Act()
    {
        ListenToInput();
    }

    void ListenToInput()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (CanMove(transform.up) == (true,false))
            {
                ICommand c = new MoveForwardCommand(_stats);
                CommandInvoker.AddCommand(c);
            }
            else if (CanMove(transform.up) == (false, true)) {
                ICommand c = new DoubleMoveCommand(_stats);
                CommandInvoker.AddCommand(c);
            } else {
                EventManager.TriggerEvent("Negative");
            }
        } else if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (CanMove(-transform.up) == (true, false))
            {
                ICommand c = new MoveBackwardCommand(_stats);
                CommandInvoker.AddCommand(c);
            }
            else
            {
                EventManager.TriggerEvent("Negative");
            }
            
        } else if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            Turn(90);
            
        } else if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            Turn(-90);
            
        }
        
        _stats.UpdateCurrentTile(new Vector3Int(Mathf.FloorToInt(_stats._targetPos.x), Mathf.FloorToInt(_stats._targetPos.y), 0));
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

    void Turn(int angle)
    {
        ICommand c = new TurnCommand(_stats, angle);
        CommandInvoker.AddCommand(c);
    }
}
