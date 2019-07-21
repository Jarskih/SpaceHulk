using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    public GameObject enemyPrefab;
    [SerializeField]
    private Spawner[] _spawners;
    
    // Start is called before the first frame update
    void Start()
    {
        _spawners = GameObject.FindGameObjectWithTag("Spawners").GetComponentsInChildren<Spawner>();
    }

    public void Spawn(int number)
    {
        for (int i = 0; i < number; i++)
        {
            Instantiate(enemyPrefab, _spawners[i].transform.localPosition, Quaternion.identity);
        }
    }
}
