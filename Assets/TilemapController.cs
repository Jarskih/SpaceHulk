using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapController : MonoBehaviour
{
    private Tilemap _tilemap;
    private TurnSystem _turnSystem;

    void Start()
    {
        _tilemap = GetComponent<Tilemap>();
        _turnSystem = FindObjectOfType<TurnSystem>();
    }
    
    public bool IsFloor(Vector3Int intPos)
    {
        var colliderType = _tilemap.GetColliderType(intPos);
        return (int)colliderType == (int)Tile.ColliderType.None;
    }
    
    public Stats GetUnitFromTile(Vector3Int pos)
    {
        var tile = _tilemap.GetInstantiatedObject(pos);

        if (tile == null)
        {
            Debug.Log("Tile outside of game area");
            return null;
        }
        
        var node = tile.GetComponent<Node>();

        if (node == null)
        {
            Debug.LogWarning("Node not found");
            return null;
        }
        
        Stats unit = null;
        var enemies = _turnSystem.enemies;
        foreach (var enemy in enemies)
        {
            if (enemy.currentNode == node)
            {
                if (enemy.health > 0)
                {
                    unit = enemy;
                }
            }
        }

        if (unit == null)
        {
            var players = _turnSystem.players;
            foreach (var player in players)
            {
                if (player.currentNode == node)
                {
                    unit = player;
                }
            }
        }
        
        return unit;
    }
    
    public bool CheckIfTileOccupied(Vector3Int newPos, Stats currentUnit)
    {
        var tile = _tilemap.GetInstantiatedObject(newPos);

        if (tile == null)
        {
            if (!IsFloor(newPos))
            {
                return true;
            }
        }

        Stats unit = null;
        var node = tile.GetComponent<Node>();

        var enemies = _turnSystem.enemies;
        foreach (var enemy in enemies)
        {
            // Dont check current player
            if (enemy == currentUnit) continue;
            if (enemy.currentNode == node)
            {
                if (enemy.health > 0)
                {
                    unit = enemy;
                }
            }
        }

        if (unit == null)
        {
            var players = _turnSystem.players;
            foreach (var player in players)
            {
                // Dont check current player
                if (player == currentUnit) continue;
                if (player.currentNode == node)
                {
                    if (player.health > 0)
                    {
                        unit = player;
                    }
                }
            }
        }

        return unit != null && unit.health > 0;
    }

}
