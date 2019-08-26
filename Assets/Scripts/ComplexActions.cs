using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class ComplexActions : MonoBehaviour
{
    public WeaponStats weaponStats;
    public IntVariable ammoUI;
    private Unit _unit;
    private TilemapController _tilemapController;
    [SerializeField] private List<Vector3Int> _tiles = new List<Vector3Int>();
    [SerializeField] private int ammo;
    [SerializeField] private int maxAmmo;

    void Start()
    {
        _unit = GetComponent<Unit>();
        _tilemapController = FindObjectOfType<TilemapController>();
        ammo = maxAmmo;
    }

    public void Act(IEnumerable<Unit> enemies)
    {
        ReturnToIdle();
    }

    private void ReturnToIdle()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _unit.ReturnToIdle(); ClearTargetingTiles();
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            _unit.ReturnToIdle(); ClearTargetingTiles();
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            _unit.ReturnToIdle(); ClearTargetingTiles();
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            _unit.ReturnToIdle(); ClearTargetingTiles();
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ClearTargetingTiles();
            _unit.ReturnToIdle();
        }
    }

    public void FindTargets()
    {
        ClearTargetingTiles();
        _unit.enemyTargets.list.Clear();
        
        // Left Side 
        {
            var startingPos = _unit.TargetPos + transform.right;
            FindEnemiesInLine(10, startingPos);
        }
        // Middle
        {
            var startingPos = _unit.TargetPos;
            FindEnemiesInLine(10, startingPos);
        }
        // Right
        {
            var startingPos = _unit.TargetPos - transform.right;
            FindEnemiesInLine(10, startingPos);
        }
    }

    public void ClearTargetingTiles()
    {
        foreach (var tile in _tiles)
        {
            _tilemapController.SetTileColour(Color.white, tile, _unit.GetTileMap());
        }

        _tiles.Clear();
    }

    void FindEnemiesInLine(int p_distance, Vector3 startingPos)
    {
        for (int distance = 1; distance < p_distance; distance++)
        {
            Vector3 pos = startingPos + transform.up * distance;
            Vector3Int intPos = new Vector3Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), 0);


            // Only shoot through floor tiles
            if (!_tilemapController.IsWalkable(intPos, _unit.GetTileMap()))
            {
                break;
            }

            // Don't let shooting through the closed doors
            if (_tilemapController.IsDoor(intPos))
            {
                if (!_tilemapController.DoorIsOpen(intPos))
                {
                    break;
                }
            }

            _tilemapController.SetTileColour(Color.yellow, intPos, _unit.GetTileMap());
            if (!_tiles.Contains(intPos))
            {
                _tiles.Add(intPos);
            }

            var enemy = _tilemapController.GetUnitFromTile(intPos);

            if (enemy == null) continue;
            if (enemy.health <= 0) continue;

            // Friendly units block line of sight if its enemy add to target list
            if (enemy.unitType != _unit.unitType)
            {
                _unit.enemyTargets.list.Add(enemy);
                continue;
            }
            break;
        }
    }

    public void UpdateAmmo()
    {
        ammoUI.Value = ammo;
    }

    public void Reload()
    {
        if (ammo < maxAmmo)
        {
            ammo = maxAmmo;
            ammoUI.Value = ammo;
            _unit.UpdateMovementPoints(-_unit.APrules.reloading);
            EventManager.TriggerEvent("Reload");
        }
        else
        {
            EventManager.TriggerEvent("Negative");
        }
    }

    public void Shoot()
    {
        if (ammo <= 0)
        {
            //Reload();
            EventManager.TriggerEvent("NoAmmo");
            return;
        }
            
        if (_unit.enemyTargets.list.Count() > 0)
        {
            EventManager.TriggerEvent(weaponStats.name);

            if (weaponStats.shotsPerBurst > 1)
            {
                int counter = 0;
                foreach (var enemy in _unit.enemyTargets.list)
                {
                    if (ammo <= 0)
                    {
                        break;
                    }
                    enemy.TakeDamage(1);
                    ammo--;
                    ammoUI.Value = ammo;
                    counter++;

                    if (counter >= weaponStats.shotsPerBurst)
                    {
                        break;
                    }
                }
            }
            else
            {
                _unit.enemyTargets.list[0].TakeDamage(weaponStats.damage);
            }
                
            ammo--;
            ammoUI.Value = ammo;

            _unit.UpdateMovementPoints(-_unit.APrules.playerAttacking);
            _unit.ReturnToIdle();
            ClearTargetingTiles();
        }
        else
        {
            FindTargets();
            if (_unit.enemyTargets.list.Count() == 0)
            {
                EventManager.TriggerEvent("Negative");
                _unit.ReturnToIdle();
                ClearTargetingTiles();
            }
        }
    }
}