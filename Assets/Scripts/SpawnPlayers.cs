using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject player1;
    private Tilemap _tilemap;
    
    public void SpawnPlayer()
    {
        var tilePos = new Vector3Int(-9, 9, 0);
        //var tilePos = GetRandomTilePos();
        Instantiate(this.player1, tilePos, Quaternion.identity);
    }

    private Vector3 GetRandomTilePos()
    {
        if (_tilemap == null)
        {
            _tilemap = FindObjectOfType<Tilemap>();
        }

        var maxCount = _tilemap.transform.childCount;
        Vector3 tilePos = Vector3.zero;

        while (tilePos == Vector3.zero)
        {
            var tile = _tilemap.transform.GetChild(GetRandomIndex(maxCount));
            if (_tilemap.GetColliderType(new Vector3Int((int)tile.transform.position.x, (int)tile.transform.position.y, 0)) == Tile.ColliderType.None)
            {
                tilePos = tile.transform.position;
            }
        }
        return tilePos;
    }
    
    int GetRandomIndex(int maxCount)
    {
        return Random.Range(0, maxCount + 1);
    }
}
