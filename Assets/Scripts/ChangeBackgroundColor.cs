using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBackgroundColor : MonoBehaviour
{
    private TurnSystem turnSystem;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        turnSystem = GameObject.FindGameObjectWithTag("TurnSystem").GetComponent<TurnSystem>();
    }

    void Update()
    {
       // cam.backgroundColor = turnSystem.GetCurrentColor();
    }
}
