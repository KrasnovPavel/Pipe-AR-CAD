using HoloCore;
using UnityEngine;

namespace MarksEditor
{
    public class InputController : Singleton<InputController>
    {
        public float Speed;

        public float RotationSpeed;

        public Camera MainCamera;

    
    
        // Update is called once per frame
        void Update()
        {
            Transform mainTransform = MainCamera.transform;
            if (Input.GetKey(KeyCode.Mouse1))
            {
                float X = Input.GetAxis("Mouse X");
                float Y = Input.GetAxis("Mouse Y");
                mainTransform.Rotate(new Vector3(-Y *RotationSpeed, X*RotationSpeed  , 0));
                X =  mainTransform.eulerAngles.x;
                Y =  mainTransform.eulerAngles.y;
                mainTransform.rotation = Quaternion.Euler(X, Y, 0);
            }
            if (Input.GetKey(KeyCode.Mouse0))
            {
                MarkPlaceController.Instance.PlaceTheMark();
            }
        
            if(Input.GetKey(KeyCode.UpArrow))
            {
                mainTransform.Translate(new Vector3(0,0,Speed * Time.deltaTime));
            }
            if(Input.GetKey(KeyCode.RightArrow))
            {
                mainTransform.Translate(new Vector3(Speed * Time.deltaTime,0,0));
            }
            if(Input.GetKey(KeyCode.LeftArrow))
            {
                mainTransform.Translate(new Vector3(-Speed * Time.deltaTime,0,0));
            }
            if(Input.GetKey(KeyCode.DownArrow))
            {
                mainTransform.Translate(new Vector3(0,0,-Speed * Time.deltaTime));
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
            if(Input.GetKeyDown(KeyCode.Q))
            {
                MarksController.Instance.MoveMark(6);
            }
            
            if(Input.GetKeyDown(KeyCode.E))
            {
                MarksController.Instance.MoveMark(7);
            }
        }
    }
}
