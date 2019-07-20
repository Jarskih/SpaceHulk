using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexActions : MonoBehaviour
{
    private Stats stats;
    
    void Start()
    {
        stats = GetComponent<Stats>();
    }
    
    public void Act()
    {
        if(Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("shoot");
            stats.UpdateMovementPoints(-1);
        }
    }
}
