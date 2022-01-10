using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public float rotateSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
    }

    void Rotate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        transform.Rotate(new Vector3(0, horizontalInput * rotateSpeed * Time.deltaTime, 0));
    }
}
