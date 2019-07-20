using UnityEngine;
using UnityEngine.Tilemaps;

public class Movement : MonoBehaviour, IMove
{
    private Tilemap tileMap;
    private Stats stats;

    void Start()
    {
        tileMap = GameObject.FindGameObjectWithTag("Grid").GetComponentInChildren<Tilemap>();
        stats = GetComponent<Stats>();
    }

    public void Act()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            //Debug.Log("current pos: " + transform.position);
            Vector3 pos = transform.position + transform.up;
            //Debug.Log(pos);
            Vector3Int intPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), (int)pos.z);
            //Debug.Log("Next tile: " + intPos);
            var colliderType = tileMap.GetColliderType(intPos);
            //Debug.Log(colliderType);
            if (colliderType == Tile.ColliderType.None)
            {
                stats.UpdateMovementPoints(-1);
                transform.position +=  transform.up;
            }
        }
        
        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            Vector3 pos = transform.position - transform.up;
            Vector3Int intPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), (int)pos.z);
            var colliderType = tileMap.GetColliderType(intPos);
            if (colliderType == Tile.ColliderType.None)
            {
                stats.UpdateMovementPoints(-2);
                transform.position -= transform.up;
            }
        }
        
        if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
            stats.UpdateMovementPoints(-1);
            transform.Rotate(0, 0, 90);
        }
        
        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
            stats.UpdateMovementPoints(-1);
            transform.Rotate(0, 0, -90);
        }
    }
}
