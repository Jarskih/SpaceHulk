using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ComplexActions : MonoBehaviour
{
    private Unit _unit;
    private TilemapController _tilemapController;

    void Start()
    {
        _unit = GetComponent<Unit>();
        _tilemapController = FindObjectOfType<TilemapController>();
    }

    public void Act(IEnumerable<Unit> enemies)
    {        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_unit.enemyTargets.list.Count() > 0)
            {
                EventManager.TriggerEvent("Shoot");

                foreach (var enemy in _unit.enemyTargets.list)
                {
                    enemy.TakeDamage(1);
                }

                _unit.UpdateMovementPoints(-_unit.APrules.playerAttacking);
                _unit.ChangeState(Unit.UnitState.Idle);
                _unit.enemyTargets.list.Clear();
            }
            else
            {
                FindTargets();
                if (_unit.enemyTargets.list.Count() == 0)
                {
                    EventManager.TriggerEvent("Negative");
                }
            }
        }
    }

    void FindTargets()
    {
        _unit.enemyTargets.list.Clear();
        for (int width = -1; width < 1; width++)
        {
            for (int distance = 1; distance < 10; distance++)
            {
                var transform1 = transform;
                Vector3 pos = transform1.position + transform1.up * distance + transform1.right * width;
                Vector3Int intPos = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), (int) pos.z);

                if (_unit.GetTileMap().GetColliderType(intPos) != Tile.ColliderType.None)
                {
                    break;
                }

                var enemy = _tilemapController.GetUnitFromTile(intPos);

                if (enemy == null)
                {
                    continue;
                }
                
                if (enemy.unitType == _unit.unitType)
                {
                    break;
                }
                
                if (enemy.isDead) continue;
                _unit.enemyTargets.list.Add(enemy);
                
            }
        }
    }
}