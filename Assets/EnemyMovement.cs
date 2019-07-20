using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyMovement : MonoBehaviour, IMove
{
    private Tilemap tileMap;
    private Stats stats;

    void Start()
    {
        stats = GetComponent<Stats>();
        tileMap = GameObject.FindGameObjectWithTag("Grid").GetComponentInChildren<Tilemap>();
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
                stats.UpdateMovementPoints(-1);
                transform.position -= transform.up;
            }
        }
        
        if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
            Vector3 pos = transform.position - transform.right;
            Vector3Int intPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), (int)pos.z);
            var colliderType = tileMap.GetColliderType(intPos);
            if (colliderType == Tile.ColliderType.None)
            {
                stats.UpdateMovementPoints(-1);
                transform.position -= transform.right;
            }
        }
        
        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
            Vector3 pos = transform.position + transform.right;
            Vector3Int intPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), (int)pos.z);
            var colliderType = tileMap.GetColliderType(intPos);
            if (colliderType == Tile.ColliderType.None)
            {
                stats.UpdateMovementPoints(-1);
                transform.position += transform.right;
            }
        } 
    }
}
