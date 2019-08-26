using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActive : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetUnitActive()
    {
        _spriteRenderer.enabled = true;
    }
    
    public void SetUnitInactive()
    {
        _spriteRenderer.enabled = false;
    }
}
