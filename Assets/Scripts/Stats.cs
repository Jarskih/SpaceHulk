using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Stats : MonoBehaviour
{
    public int actionPoints;
    private IMove _movement;
    private ComplexActions _complexActions;
    private int health = 1;
    private Tilemap _tileMap;

    public Color color;
    
    
    void Start()
    {
        _movement = GetComponent<IMove>();
        _complexActions = GetComponent<ComplexActions>();
        _tileMap = GameObject.FindGameObjectWithTag("Grid").GetComponentInChildren<Tilemap>();
        StartCoroutine(SaveCurrentTile());
    }

    private IEnumerator SaveCurrentTile()
    {
        yield return new WaitForSeconds(0.1f);
        UpdateCurrentTile(GetCurrentTilePos());
    }

    Vector3Int GetCurrentTilePos()
    {
        return new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), (int)transform.position.z);
    }

    public void Movement()
    {
        _movement.Act();
    }

    public void Actions()
    {
        _complexActions.Act();
    }

    public void UpdateMovementPoints(int change)
    {
        actionPoints = Mathf.Clamp(actionPoints + change, 0, 4);
    }

    public void UpdateCurrentTile(Vector3Int newPos)
    {
        var currentTile = _tileMap.GetInstantiatedObject(GetCurrentTilePos());
        if (currentTile != null)
        {
            currentTile.GetComponent<Node>().unitOnNode = null;
        }
            
        var tileObject = _tileMap.GetInstantiatedObject(newPos);
        tileObject.GetComponent<Node>().unitOnNode = this;
    }

    public bool CheckIfTileIsFree(Vector3Int newPos)
    {
        return _tileMap.GetInstantiatedObject(newPos).GetComponent<Node>().unitOnNode == null;
    }
}
