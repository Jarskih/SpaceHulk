using UnityEngine;

public class BloodCreator : MonoBehaviour
{
    public static void CreateBlood(Vector3 pos, GameObject blood)
    {
        var bloodInstance = Instantiate(blood, pos, Quaternion.identity);
        var rotation = Random.rotation;
        rotation.y = 0;
        rotation.x = 0;
        bloodInstance.transform.rotation = rotation;
        bloodInstance.transform.SetParent(GameObject.FindGameObjectWithTag("Instances").transform);
    }

    public static void CreateBlood(Vector3Int pos, GameObject blood)
    {
        var bloodInstance = Instantiate(blood, pos, Quaternion.identity);
        var rotation = Random.rotation;
        rotation.y = 0;
        rotation.x = 0;
        bloodInstance.transform.rotation = rotation;
        bloodInstance.transform.SetParent(GameObject.FindGameObjectWithTag("Instances").transform);
    }
}
