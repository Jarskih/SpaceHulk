using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    public GameObject enemyPrefab;
    [SerializeField]
    private Spawner[] spawners;

    // Start is called before the first frame update
    private void Start()
    {
        spawners = GameObject.FindGameObjectWithTag("Spawners").GetComponentsInChildren<Spawner>();
    }

    public void Spawn(int number, List<UnitStats> enemyTypes)
    {
        for (int i = 0; i < number; i++)
        {
            if (i > spawners.Length)
            {
                break;
            }
            
            var spawnerIndex = Random.Range(0, spawners.Length);
            if (spawners[spawnerIndex].spawned)
            {
                number++;
                continue;
            }
            
            var instance = Instantiate(enemyPrefab, spawners[spawnerIndex].transform.localPosition, Quaternion.identity);
            instance.transform.SetParent(GameObject.FindGameObjectWithTag("Instances").transform);
            instance.GetComponent<Unit>().unitStats = enemyTypes[Random.Range(0, enemyTypes.Count)];

            spawners[spawnerIndex].spawned = true;
        }

        foreach (var spawner in spawners)
        {
            spawner.spawned = false;
        }
    }
}
