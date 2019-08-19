using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ComplexActions : MonoBehaviour
{
    private Stats _stats;
    private TilemapController _tilemapController;

    void Start()
    {
        _stats = GetComponent<Stats>();
        _tilemapController = FindObjectOfType<TilemapController>();
    }

    public void Act(IEnumerable<Stats> enemies)
    {        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_stats.enemyTargets.list.Count() > 0)
            {
                EventManager.TriggerEvent("Shoot");

                foreach (var enemy in _stats.enemyTargets.list)
                {
                    enemy.TakeDamage(1);
                }

                _stats.UpdateMovementPoints(-_stats.APrules.playerAttacking);
                _stats.ChangeState(Stats.UnitState.Idle);
                _stats.enemyTargets.list.Clear();
            }
            else
            {
                FindTargets();
                if (_stats.enemyTargets.list.Count() == 0)
                {
                    EventManager.TriggerEvent("Negative");
                }
            }
        }
    }

    void FindTargets()
    {
        _stats.enemyTargets.list.Clear();
        for (int width = -1; width < 1; width++)
        {
            for (int distance = 1; distance < 10; distance++)
            {
                var transform1 = transform;
                Vector3 pos = transform1.position + transform1.up * distance + transform1.right * width;
                Vector3Int intPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), (int) pos.z);

                if (_stats.GetTileMap().GetColliderType(intPos) != Tile.ColliderType.None)
                {
                    break;
                }

                var enemy = _tilemapController.GetUnitFromTile(intPos);

                if (enemy == null)
                {
                    continue;
                }
                
                if (enemy.unitType == _stats.unitType)
                {
                    break;
                }
                
                if (enemy.isDead) continue;
                _stats.enemyTargets.list.Add(enemy);
                
            }
        }
    }
}