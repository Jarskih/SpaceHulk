using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Camera _cam;
    private Vector3 _target;
    public bool followEnabled;
    public float maxDist = 0.3f;
    
    // Start is called before the first frame update
    void Start()
    {
        _cam = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (followEnabled)
        {
            _cam.transform.position = Vector3.MoveTowards(_cam.transform.position, new Vector3(_target.x, _target.y, _cam.transform.position.z), maxDist);
        }
    }

    public void SetTarget(Vector3 target)
    {
        _target = target;
    }
}
