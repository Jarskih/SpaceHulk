using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VisibleEnemyUI : MonoBehaviour
{
    /*
    
    //public UnitList enemyList;
    public GameObject enemyIndicator;
    private int oldEnemyCount;
    private int enemyCount;
    private PlayerInteractions PI;

    private UnitController currentUnit;

    // Update is called once per frame
    private void Update()
    {
        if (PI == null)
        {
            PI = PlayerInteractions.GetInstance();
        }
        
        var enemies = PI.enemyList;
        enemyCount = enemies.Count;

        if (enemyCount != oldEnemyCount)
        {
            oldEnemyCount = enemyCount;
            foreach (var enemy in GetComponentsInChildren<SpottedEnemy>())
            {
                Destroy(enemy.gameObject);
            }

            foreach (var enemy in enemies)
            {
                if (enemy != null)
                {
                    var indicator = Instantiate(enemyIndicator, transform.position, Quaternion.identity);
                    indicator.transform.SetParent(transform);
                    var spottedEnemy = indicator.GetComponent<SpottedEnemy>();
                    spottedEnemy.controller = enemy.GetComponent<UnitController>();
                }
            }

            enemyCount = transform.childCount;
        }
    }
    */
}
