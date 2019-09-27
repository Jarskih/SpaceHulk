using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUITextToUnit : MonoBehaviour
{
    private TurnSystem turns;
    private Camera _cam;
    private Vector3 _point;
    // Start is called before the first frame update
    void Start()  {
        _cam = FindObjectOfType<Camera>();
        turns = FindObjectOfType<TurnSystem>();
        if (turns == null)
        {
            Debug.Log("not turn system added to ui text");
        }
    }

    // Update is called once per frame
    void Update()
    {
        var position = turns.activeUnit.transform.position;
        _point = _cam.WorldToScreenPoint(new Vector3(position.x, position.y, 0));
        transform.position = _point;
    }
}
