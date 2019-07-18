using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float Speed;
    public float RotationSpeed;
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            float X = Input.GetAxis("Mouse X");
            float Y = Input.GetAxis("Mouse Y");
            transform.Rotate(new Vector3(-Y *RotationSpeed, X*RotationSpeed  , 0));
            X = transform.rotation.eulerAngles.x;
            Y = transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(X, Y, 0);
        }

        if(Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(new Vector3(0,0,Speed * Time.deltaTime));
        }
        if(Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(new Vector3(Speed * Time.deltaTime,0,0));
        }
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(new Vector3(-Speed * Time.deltaTime,0,0));
        }
        if(Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(new Vector3(0,0,-Speed * Time.deltaTime));
        }
        if(Input.GetKey(KeyCode.W))
        {
            MarksController.Instance.MoveMark(0);
        }
        if(Input.GetKey(KeyCode.S))
        {
            MarksController.Instance.MoveMark(1);
        }
        if(Input.GetKey(KeyCode.A))
        {
            MarksController.Instance.MoveMark(2);
        }
        if(Input.GetKey(KeyCode.D))
        {
            MarksController.Instance.MoveMark(3);
        }
        if(Input.GetKey(KeyCode.R))
        {
            MarksController.Instance.MoveMark(4);
        }
        if(Input.GetKey(KeyCode.F))
        {
            MarksController.Instance.MoveMark(5);
        }

    }
    
    
    
}
