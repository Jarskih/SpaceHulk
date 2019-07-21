using UnityEngine;
using UnityEngine.Tilemaps;

public class Movement : MonoBehaviour, IMove
{
    private Tilemap _tileMap;
    private Stats _stats;

    void Start()
    {
        _tileMap = GameObject.FindGameObjectWithTag("Grid").GetComponentInChildren<Tilemap>();
        _stats = GetComponent<Stats>();
    }

    public void Act()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (Move(transform.up))
            {
                _stats.UpdateMovementPoints(-1);
            }
        }
        
        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if(Move(-transform.up))
            {
                _stats.UpdateMovementPoints(-2);
            }
        }
        
        if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
            Turn(90);
        }
        
        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
            Turn(-90);
        }
    }

    bool Move(UnityEngine.Vector3 direction)
    {
        Vector3 pos = transform.position + direction;
        Vector3Int intPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), (int)pos.z);
        var colliderType = _tileMap.GetColliderType(intPos);
        if (colliderType == Tile.ColliderType.None)
        {
            if (!_stats.CheckIfTileIsFree(intPos))
            {
                Debug.Log("Someone on the way");
                return false;
            }
            // TODO Jump over friendly
            
            _stats.UpdateCurrentTile(intPos);
            transform.position += direction;
            return true;
        }
        return false;
    }

    void Turn(int angle)
    {
        _stats.UpdateMovementPoints(-1);
        transform.Rotate(0, 0, angle);
    }
}
