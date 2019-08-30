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

    public void Spawn(int number)
    {
        for (int i = 0; i < number; i++)
        {
            var instance = Instantiate(enemyPrefab, spawners[i].transform.localPosition, Quaternion.identity);
            instance.transform.SetParent(GameObject.FindGameObjectWithTag("Instances").transform);
        }
    }
}
