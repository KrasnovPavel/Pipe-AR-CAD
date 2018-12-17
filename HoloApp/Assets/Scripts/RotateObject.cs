using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System;

namespace HoloCAD
{
    [ExecuteInEditMode]

    public class RotateObject : MonoBehaviour
    {

        public string RotateText = "Some text";
        public string Radius = "Some text";
        public int textSize = 14;
        public Font textFont;
        public Color textColor = Color.white;
        public float textHeight = 0.06f;
        public GameObject grandChild;


        private void Update()
        {
            if (GetComponent<BaseTube>().IsSelected == true)
            {
                if (gameObject.GetComponent<BendedTube>() != true)
                {
                    RotateText = "Длины трубы " + GetComponent<DirectTube>().Length + " м";
                }

                else
                {
                    if (GetComponent<BendedTube>().UseSecondRadius == true)
                    {
                        Radius = "второй";
                    }
                    else
                    {
                        Radius = "первый";
                    }
                    RotateText = "Угол погиба " + GetComponent<BendedTube>().Angle + "°\r\n Угол поворота " + Math.Round(gameObject.transform.rotation.eulerAngles.y) +
                        "°\r\n Радиус: " + Radius;
                }
            }

            if (GetComponent<BaseTube>().IsSelected == false)
            {
                RotateText = "";
            }

        }


    private void Start()
        {
            enabled = true;
        }

        void Awake()
        {
            grandChild = this.gameObject.transform.GetChild(1).gameObject;
#if ENABLE_WINMD_SUPPORT
            _recognizer = new GestureRecognizer();
            _recognizer.Tapped += args =>
            {
               OnClick();
            };
            _recognizer.StartCapturingGestures();
#endif
        }

        void OnClick()
        {
            Debug.Log("Нашли нажатие");
        }

        void OnGUI()
        {

            GUI.depth = 9999;

            GUIStyle style = new GUIStyle();
            style.fontSize = textSize;
            style.richText = true;
            if (textFont) style.font = textFont;
            style.normal.textColor = textColor;
            style.alignment = TextAnchor.MiddleCenter;


            Vector3 worldPosition = new Vector3(grandChild.transform.position.x, grandChild.transform.position.y + textHeight, grandChild.transform.position.z);
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
            screenPosition.y = Screen.height - screenPosition.y;

            GUI.Label(new Rect(screenPosition.x, screenPosition.y, 0, 0), RotateText, style);
        }
    }
}