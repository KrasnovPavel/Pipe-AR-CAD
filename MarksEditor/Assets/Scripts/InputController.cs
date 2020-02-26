// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using UnityEngine;

namespace GLTFConverter
{
    /// <summary> Контроллер ввода с клавиатуры и мыши. </summary>
    public class InputController : Singleton<InputController>
    {
        /// <summary> Скорость движения камеры. </summary>
        [Tooltip("Скорость движения камеры.")] 
        public float Speed;

        /// <summary> Скорость вращения камеры. </summary>
        [Tooltip("Скорость вращения камеры.")] 
        public float RotationSpeed;

        #region Unity events

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        // Update is called once per frame
        private void Update()
        {
            Transform mainTransform = _mainCamera.transform;
            if (Input.GetKey(KeyCode.Mouse1))
            {
                float x = Input.GetAxis("Mouse X");
                float y = Input.GetAxis("Mouse Y");
                mainTransform.Rotate(new Vector3(-y * RotationSpeed, x * RotationSpeed, 0));
                x = mainTransform.eulerAngles.x;
                y = mainTransform.eulerAngles.y;
                mainTransform.rotation = Quaternion.Euler(x, y, 0);
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                MarkPlaceController.Instance.PlaceTheMark();
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                mainTransform.Translate(new Vector3(0, 0, Speed * Time.deltaTime));
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                mainTransform.Translate(new Vector3(Speed * Time.deltaTime, 0, 0));
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                mainTransform.Translate(new Vector3(-Speed * Time.deltaTime, 0, 0));
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                mainTransform.Translate(new Vector3(0, 0, -Speed * Time.deltaTime));
            }

            if (Input.GetKey(KeyCode.W))
            {
                MarksController.Instance.MoveMark(MarksController.Directions.Forward);
            }

            if (Input.GetKey(KeyCode.S))
            {
                MarksController.Instance.MoveMark(MarksController.Directions.Backward);
            }

            if (Input.GetKey(KeyCode.A))
            {
                MarksController.Instance.MoveMark(MarksController.Directions.Left);
            }

            if (Input.GetKey(KeyCode.D))
            {
                MarksController.Instance.MoveMark(MarksController.Directions.Right);
            }

            if (Input.GetKey(KeyCode.R))
            {
                MarksController.Instance.MoveMark(MarksController.Directions.Up);
            }

            if (Input.GetKey(KeyCode.F))
            {
                MarksController.Instance.MoveMark(MarksController.Directions.Down);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                MarksController.Instance.MoveMark(MarksController.Directions.RotationRight);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                MarksController.Instance.MoveMark(MarksController.Directions.RotationLeft);
            }
        }

        #endregion
        
        #region Private defenition

        /// <summary> Объект камеры </summary>
        private Camera _mainCamera;

        #endregion
    }
}