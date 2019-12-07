using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashHandler : MonoBehaviour
{
    private SpriteRenderer _renderer;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.enabled = false;
    }

    public void Flash()
    {
        StartCoroutine("EnableFlash");
    }

    private IEnumerator EnableFlash()
    {
        _renderer.enabled = true;
        yield return new WaitForSeconds(0.1f);
        _renderer.enabled = false;
    }
}