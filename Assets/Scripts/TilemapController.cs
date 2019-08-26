using System.Collections.Generic;
using System.Xml.Linq;
using Interfaces;
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

    public bool IsWalkable(Vector3Int intPos, Tilemap tilemap)
    {
        GameObject tile = _tilemap.GetInstantiatedObject(intPos);
        var walkable = tile.GetComponent<IWalkable>() != null;
        return walkable;
    }

    public Unit GetUnitFromTile(Vector3Int pos)
    {
        var tile = _tilemap.GetInstantiatedObject(pos);

        if (tile == null)
        {
            Debug.Log("Tile outside of game area");
            return null;
        }
        
        Unit unit = null;
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        //var enemies = _turnSystem.enemies;
        foreach (var enemy in enemies)
        {
            var u = enemy.GetComponent<Unit>();
            if (u.currentNode == tile)
            {
                if (u.health > 0)
                {
                    unit = u;
                }
            }
        }

        if (unit == null)
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            //var players = _turnSystem.players;
            foreach (var player in players)
            {
                var u = player.GetComponent<Unit>();
                if (u.currentNode == tile)
                {
                    unit = u;
                }
            }
        }
        
        return unit;
    }
    
    public bool CheckIfTileOccupied(Vector3Int newPos, Unit currentUnit)
    {
        var tile = _tilemap.GetInstantiatedObject(newPos);

        if (tile == null)
        {
            return true;
        }

        Unit unit = null;

        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        //var enemies = _turnSystem.enemies;
        foreach (var enemy in enemies)
        {
            var u = enemy.GetComponent<Unit>();
            // Dont check current player
            if (u == currentUnit) continue;
            if (u.currentNode == tile)
            {
                if (u.health > 0)
                {
                    unit = u;
                }
            }
        }

        if (unit == null)
        {
            // var players = _turnSystem.players;
            var players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in players)
            {
                var u = player.GetComponent<Unit>();
                // Dont check current player
                if (u == currentUnit) continue;
                if (u.currentNode == tile)
                {
                    if (u.health > 0)
                    {
                        unit = u;
                    }
                }
            }
        }

        return unit != null && unit.health > 0;
    }

    public bool TileOccupiedByAlien(Vector3Int newPos, Unit currentUnit)
    {
        var tile = _tilemap.GetInstantiatedObject(newPos);

        if (tile == null)
        {
            return true;
        }

        Unit unit = null;

        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        //var enemies = _turnSystem.enemies;
        foreach (var enemy in enemies)
        {
            var u = enemy.GetComponent<Unit>();
            // Dont check current player
            if (u == currentUnit) continue;
            if (u.currentNode == tile)
            {
                if (u.health > 0)
                {
                    unit = u;
                }
            }
        }

        return unit != null && unit.health > 0;
    }
    
    public bool TileOccupiedByMarine(Vector3Int newPos, Unit currentUnit)
    {
        var tile = _tilemap.GetInstantiatedObject(newPos);

        if (tile == null)
        {
            return true;
        }

        Unit unit = null;
            
        // var players = _turnSystem.players;
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            var u = player.GetComponent<Unit>();
            // Dont check current player
            if (u == currentUnit) continue;
            if (u.currentNode == tile)
            {
                if (u.health > 0)
                {
                    unit = u;
                }
            }
        }

        return unit != null && unit.health > 0;
    }

    //Add information about the surroundings:
    //0 = not valid tile or occupied by Alien unit
    //1 = free tile
    //2 = current unit
    //3 = occupied by player unit
    public List<int> GetSurroundingTiles(Vector3 pos, Unit unit)
    {
        List<int> tileIndex = new List<int>();
        Vector3Int currentPos = new Vector3Int((int)pos.x, (int)pos.y, 0);
        
        for (int x = -1; x < 1; x++)
        {
            for (int y = -1; y < 1; y++)
            {
                var intPos = new Vector3Int(currentPos.x + x, currentPos.y + y, 0);
                var tileObj = _tilemap.GetInstantiatedObject(intPos);
                if (tileObj != null)
                {
                    if (_tilemap.GetColliderType(intPos) == Tile.ColliderType.None)
                    {
                        var unitFromTile = GetUnitFromTile(intPos);
                        if (unitFromTile == null)
                        {
                            // Free tile
                            tileIndex.Add(1);
                            continue;
                        }
                            
                            
                        {
                            if (unitFromTile == unit)
                            {
                                tileIndex.Add(2);
                            } else if (unitFromTile.unitType == Unit.UnitType.Alien)
                            {
                                tileIndex.Add(0);
                            }
                            else if(unitFromTile.unitType == Unit.UnitType.Marine)
                            {
                                tileIndex.Add(3);         
                            }
                            else
                            {
                                Debug.LogWarning("Wrong index");
                                tileIndex.Add(0);
                            }
                        }

                    }
                    else
                    {
                        // Wall
                        tileIndex.Add(0);
                    }
                }
                else
                {
                    // Not valid tile
                    tileIndex.Add(0);
                }
            }
        }
        return tileIndex;
    }

    public Tile GetTile(Vector3Int pos)
    {
        return _tilemap.GetTile(pos) as Tile;
    }
    
    /// <summary>
    /// Set the colour of a tile.
    /// </summary>
    /// <param name="colour">The desired colour.</param>
    /// <param name="position">The position of the tile.</param>
    /// <param name="tilemap">The tilemap the tile belongs to.</param>
    public void SetTileColour(Color colour, Vector3Int position, Tilemap tilemap)
    {
        // Flag the tile, inidicating that it can change colour.
        // By default it's set to "Lock Colour".
        tilemap.SetTileFlags(position, TileFlags.None);
 
        // Set the colour.
        tilemap.SetColor(position, colour);
    }

    public bool IsDoor(Vector3Int intPos)
    {
       var door = _tilemap.GetInstantiatedObject(intPos).GetComponent<IOpenable>() as DoorNode;
       return door != null;
    }

    public bool DoorIsOpen(Vector3Int intPos)
    {
        var door = _tilemap.GetInstantiatedObject(intPos).GetComponent<IOpenable>() as DoorNode;
        return door != null && door.isOpen;
    }
}
