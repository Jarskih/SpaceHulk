using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    private CameraFollow _cameraFollow;
    
   [SerializeField] private float speed = 5;
   [SerializeField] private int padding = 5;

   void Start()
   {
       _cameraFollow = FindObjectOfType<CameraFollow>();
   }

    // Update is called once per frame
    void Update()
    {
        if (_cameraFollow.followEnabled)
        {
            return;
        }
        
        // Mouse
        if (Input.mousePosition.x < padding)
        {
            transform.position += Time.deltaTime * speed * new Vector3(-1, 0, 0);
        } else if (Input.mousePosition.x > Screen.width - padding)
        {
            transform.position += Time.deltaTime * speed * new Vector3(1, 0,0 );
        }
        
        if (Input.mousePosition.y < padding)
        {
            transform.position += Time.deltaTime * speed * new Vector3(0, -1, 0);
        } else if (Input.mousePosition.y > Screen.height - padding)
        {
            transform.position += Time.deltaTime * speed * new Vector3(0, 1,0 );
        }
        
        
        // Keyboard
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += Time.deltaTime * speed * new Vector3(-1, 0, 0);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += Time.deltaTime * speed * new Vector3(1, 0,0 );
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += Time.deltaTime * speed * new Vector3(0, 1,0 );
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += Time.deltaTime * speed * new Vector3(0, -1, 0);
        }


        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -20, 20), Mathf.Clamp(transform.position.y, -20, 20), transform.position.z);
    }
}
