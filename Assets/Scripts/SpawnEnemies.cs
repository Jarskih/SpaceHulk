using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    public GameObject enemyPrefab;
    [SerializeField]
    private Spawner[] spawners;
    
    // Start is called before the first frame update
    void Start()
    {
        spawners = GameObject.FindGameObjectWithTag("Spawners").GetComponentsInChildren<Spawner>();
    }

    public void Spawn(int number)
    {
        for (int i = 0; i < number; i++)
        {
            Instantiate(enemyPrefab, spawners[i].transform.localPosition, Quaternion.identity);
        }
    }
}
