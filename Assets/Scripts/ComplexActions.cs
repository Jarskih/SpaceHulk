using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
                    _unit.ChangeState(Unit.UnitState.Idle);
                }
            }
        }
    }

    void FindTargets()
    {
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

    void FindEnemiesInLine(int p_distance, Vector3 startingPos)
    {
        for (int distance = 1; distance < p_distance; distance++)
        {
            Vector3 pos = startingPos + transform.up * distance;
            Vector3Int intPos = new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);
            
            Debug.Log(intPos);
            
            // Only shoot through floor tiles
            if (!_tilemapController.IsFloor(intPos))
            {
                break;
            }
                
            StartCoroutine(ChangeTileColor(intPos));

            var enemy = _tilemapController.GetUnitFromTile(intPos);

            if (enemy == null) continue;
            if (enemy.isDead) continue;
            
            // Friendly units block line of sight if its enemy add to target list
            if (enemy.unitType != _unit.unitType)
            {
                _unit.enemyTargets.list.Add(enemy);
                continue;
            }
            break;
                
        }
    }

    private IEnumerator ChangeTileColor(Vector3Int pos)
    {
        _tilemapController.SetTileColour(Color.yellow, pos, _unit.GetTileMap());   
        yield return new WaitForSeconds(1f);
        _tilemapController.SetTileColour(Color.white, pos, _unit.GetTileMap());
    }
}