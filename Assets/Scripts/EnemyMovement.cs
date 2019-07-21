using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyMovement : MonoBehaviour, IMove
{
    private Tilemap _tileMap;
    private Stats _stats;

    void Start()
    {
        _stats = GetComponent<Stats>();
        _tileMap = GameObject.FindGameObjectWithTag("Grid").GetComponentInChildren<Tilemap>();
    }
    
    public void Act()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            Move(transform.up);
        }
        
        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            Move(-transform.up);
        }
        
        if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) 
        {
            Move(-transform.right);
        }
        
        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) 
        {
            Move(transform.right);
        } 
    }

    void Move(UnityEngine.Vector3 direction)
    {
        Vector3 pos = transform.position + direction;
        Vector3Int intPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), (int)pos.z);
        var colliderType = _tileMap.GetColliderType(intPos);
        if (colliderType == Tile.ColliderType.None)
        {

            if (!_stats.CheckIfTileIsFree(intPos))
            {
                var enemyStats = _stats.GetUnitFromTile(intPos);
                if (enemyStats != null && enemyStats.unitType != _stats.unitType)
                {
                    enemyStats.TakeDamage(1);
                }
                else
                {

                    pos = transform.position + direction * 2;
                    intPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), (int) pos.z);
                    colliderType = _tileMap.GetColliderType(intPos);
                    if (colliderType == Tile.ColliderType.None)
                    {
                        if (_stats.CheckIfTileIsFree(intPos) && _stats.actionPoints >= 2)
                        {
                            _stats.UpdateCurrentTile(intPos);
                            transform.position += direction * 2;
                            _stats.UpdateMovementPoints(-2);
                            return;
                        }
                        else
                        {
                            Debug.Log("Someone on the way");
                            return;
                        }
                    }
                }
            }
            else
            {
                _stats.UpdateCurrentTile(intPos);
           
                _stats.UpdateMovementPoints(-1);
                transform.position += direction;

            }
        }
    }
}
