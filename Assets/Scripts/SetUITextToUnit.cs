using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUITextToUnit : MonoBehaviour
{
    private PlayerInteractions _pi;
    private Camera _cam;
    private Vector3 _point;
    // Start is called before the first frame update
    void Start()  {
        _cam = FindObjectOfType<Camera>();
        _pi = FindObjectOfType<PlayerInteractions>();
        if (_pi == null)
        {
            Debug.Log("not PlayerInteractions added to ui text");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_pi.activeUnit == null) return;
        var position = _pi.activeUnit.transform.position;
        _point = _cam.WorldToScreenPoint(new Vector3(position.x, position.y, 0));
        transform.position = _point;
    }
}
