using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float Speed;
    public float RotationSpeed;
    public float SelectedmMarkSpeed;
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
           Vector3 selectedMarkVector3 =  MarksController.Instance.SelectedMark.transform.forward * SelectedmMarkSpeed   ;
           MarksController.Instance.MoveMark(selectedMarkVector3);
       }
       if(Input.GetKey(KeyCode.S))
       {
           Vector3 selectedMarkVector3 = -  MarksController.Instance.SelectedMark.transform.forward * SelectedmMarkSpeed;
           MarksController.Instance.MoveMark(selectedMarkVector3);
       }
       if(Input.GetKey(KeyCode.A))
       {
           Vector3 selectedMarkVector3 = -  MarksController.Instance.SelectedMark.transform.right * SelectedmMarkSpeed;
           MarksController.Instance.MoveMark(selectedMarkVector3);
       }
       if(Input.GetKey(KeyCode.D))
       {
           Vector3 selectedMarkVector3 = MarksController.Instance.SelectedMark.transform.right * SelectedmMarkSpeed;
           MarksController.Instance.MoveMark(selectedMarkVector3);
       }
       if(Input.GetKey(KeyCode.R))
       {
           Vector3 selectedMarkVector3 = MarksController.Instance.SelectedMark.transform.up * SelectedmMarkSpeed;
           MarksController.Instance.MoveMark(selectedMarkVector3);
       }
       if(Input.GetKey(KeyCode.F))
       {
           Vector3 selectedMarkVector3 = -MarksController.Instance.SelectedMark.transform.up * SelectedmMarkSpeed;
           MarksController.Instance.MoveMark(selectedMarkVector3);
       }

    }
    
    
    
}
