using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    private Unit _unit;   
    // Start is called before the first frame update
    void Start()
    {
        _unit = GetComponent<Unit>();
    }

    public void RespawnUnit()
    {
        transform.position = _unit.startingPos;
        _unit.UpdateCurrentTile(_unit.startingPos);
    }
}
