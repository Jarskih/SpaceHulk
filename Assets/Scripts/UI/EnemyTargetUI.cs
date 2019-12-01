using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetUI : MonoBehaviour
{
    public BoolVariable enableTargetingUI;
    public GameObject targetingUI;

    void Update()
    {
        if (enableTargetingUI.Value)
        {
            targetingUI.SetActive(true);   
        }
        else
        {
            targetingUI.SetActive(false);
        }
    }
}
