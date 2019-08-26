using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedoController : MonoBehaviour
{
    private Button button;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CommandInvoker.CanRedo())
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }
}
