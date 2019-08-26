using Interfaces;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Movement : MonoBehaviour, IMove
{
    [SerializeField] private float timeBetweenDecisions;
    [SerializeField] private float timeSinceDecision = 0;
    [SerializeField] private bool isAi;
    private Tilemap _tileMap;
    private Unit _unit;
    private TilemapController _tilemapController;
    private SpaceHulkPlayerAgent _agent;

    void Start()
    {
        _tileMap = FindObjectOfType<Tilemap>();
        _tilemapController = FindObjectOfType<TilemapController>();
        _unit = GetComponent<Unit>();
        _agent = GetComponent<SpaceHulkPlayerAgent>();
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

    public void CanMove(Vector3 direction)
    {
        Vector3 pos = _unit.TargetPos + direction;
        Vector3Int intPos = new Vector3Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), 0);
        
        // Check if tile is not a floor
        if (!_tilemapController.IsWalkable(intPos, _unit.GetTileMap()))
        {
            return;
        }
        
       // If tile is occupied by a unit move two tiles
       if (_tilemapController.CheckIfTileOccupied(intPos, _unit))
       {
           // if unit type is not friendly unit don't allow move through
           if (_tilemapController.GetUnitFromTile(intPos).unitType != _unit.unitType)
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
        ICommand c = new MeleeAttackCommand(_unit, enemy);
        CommandInvoker.AddCommand(c);
    }

    void MoveOneTile(Vector3 direction, Vector3Int intPos)
    {
       
        // Check if tile is walkable
        var colliderType = _tileMap.GetColliderType(intPos);
        var door = _tileMap.GetInstantiatedObject(intPos).GetComponent<IOpenable>() as DoorNode;
        if (colliderType == Tile.ColliderType.Grid && door == null)
        {
            Debug.Log("Tile not walkable");
            EventManager.TriggerEvent("Negative");
            return;
        }

        if (direction == transform.up)
        {
            ICommand c = new MoveForwardCommand(_unit);
            CommandInvoker.AddCommand(c);
        }
        else if(direction == -transform.up)
        {
            if (_unit.actionPoints >= 2)
            {
                ICommand c = new MoveBackwardCommand(_unit);
                CommandInvoker.AddCommand(c);
            }
            else
            {
                EventManager.TriggerEvent("Negative");
            }
        }
    }

    void MoveTwoTiles(Vector3 direction)
    {
      if (_unit.actionPoints < 2)
      {
          Debug.Log("Not enough AP to jump over");
          EventManager.TriggerEvent("Negative");
          return;
      }
         
      Vector3 pos = _unit.TargetPos + direction*2;
      Vector3Int intPos = new Vector3Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), (int)pos.z);
         
      // Check if a tile 2 steps away is walkable (jump over)
      var colliderType = _tileMap.GetColliderType(intPos);
      if (colliderType == Tile.ColliderType.None || _tileMap.GetInstantiatedObject(intPos).GetComponent<IOpenable>() != null)
      {
          // if tile is not occupied by unit allow movement
          if (!_tilemapController.CheckIfTileOccupied(intPos, _unit))
          {
              ICommand c = new DoubleMoveCommand(_unit);
              CommandInvoker.AddCommand(c);
              return;
          }
      }

        Debug.Log("Someone on the way");
        EventManager.TriggerEvent("Negative");
    }

    public void Turn(int angle)
    {
        ICommand c = new TurnCommand(_unit, angle);
        CommandInvoker.AddCommand(c);
    }
}
