﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HoloCAD
{
    public class InputManager : MonoBehaviour
    {

        void Awake()
        {
#if ENABLE_WINMD_SUPPORT
            _recognizer = new GestureRecognizer();
            _recognizer.Tapped += args =>
            {
               OnClick();
            };
            _recognizer.StartCapturingGestures();
#endif
        }

        private void OnClick()
        {


            // Ray ray = GetComponent<Camera>().ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y,1));
            RaycastHit _hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out _hit, Mathf.Infinity))
            {
                if (_hit.transform.GetComponent<Button3D>())
                {
                    _hit.transform.GetComponent<Button3D>().OnClick();
                }
            }

        }


        // Use this for initialization
        void Start()
        {

        }
    }
}
