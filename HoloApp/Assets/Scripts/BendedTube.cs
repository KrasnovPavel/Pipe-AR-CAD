using HoloToolkit.Unity.InputModule;
using System.Collections.Generic;
using UnityEngine;

namespace HoloCAD
{
    /// <inheritdoc />
    /// <summary>
    /// Класс реализующий погиб трубы.
    /// </summary>
    public class BendedTube : BaseTube
    {
        private List<Mesh> _meshes;
    
        private bool _useSecondRadius;
        private int _angle;
    
        /// <value> Первый из двух допустимых по ОСТ радиусов погиба. </value>
        [Range(0.035f, 1.2f)]
        public float FirstBendRadius = 0.07f;
        
        /// <value> Второй из двух допустимых радиусов погиба. </value>
        [Range(0.055f, 0.87f)]
        public float SecondBendRadius = 0.055f;
    
        /// <value> Угол погиба. </value>
        public int Angle
        {
            get { return _angle; }
            set
            {
                if (value < MeshFactory.DeltaAngle || value > 180)
                {
                    return;
                }
                _angle = value;
    
                SetMesh();
                Label.GetComponent<TextMesh>().text = "Угол погиба: " + _angle + "°";
            }
        }
    
        /// <value>
        /// Флаг, указывающий какой из двух радиусов используется.
        /// <c>true</c> если второй. 
        /// </value>
        public bool UseSecondRadius
        {
            get { return _useSecondRadius; }
            set
            {
                _useSecondRadius = value;
    
                SetMesh();
            }
        }
        
        /// <summary>
        /// Функция инициализирующая трубу в Unity. 
        /// </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью
        /// <c> base.Start()</c>.
        /// </remarks>
        protected new void Start()
        {
            base.Start();
            _meshes = MeshFactory.CreateMeshes(Diameter, FirstBendRadius, SecondBendRadius);
            _useSecondRadius = false;
            ButtonBar.GetComponent<ButtonBar>().Offset = 0.7f * Diameter;
            Tube.GetComponent<MeshRenderer>().material.SetFloat("_Diameter", Diameter);
            Angle = 90;
            TubeManager.SelectTube(this);
        }
    
        /// <inheritdoc />
        /// <summary>
        /// Обработчик нажатия на кнопку из HoloToolKit.
        /// </summary>
        /// <param name="obj">Нажатая кнопка</param>
        /// <param name="eventData">Информация о событии</param>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью
        /// <c> base.InputDown()</c>.
        /// </remarks>
        protected override void InputDown(GameObject obj, InputEventData eventData)
        {
            base.InputDown(obj, eventData);

            switch (obj.name)
            {
                case "IncreaseAngleButton":
                    Angle += MeshFactory.DeltaAngle;
                    break;
                case "DecreaseAngleButton":
                    Angle -= MeshFactory.DeltaAngle;
                    break;
                case "ClockwiseButton":
                    transform.localRotation *= Quaternion.Euler(0, 0, MeshFactory.DeltaAngle);
                    break;
                case "AnticlockwiseButton":
                    transform.localRotation *= Quaternion.Euler(0, 0, -MeshFactory.DeltaAngle);
                    break;
                case "ChangeRadiusButton":
                    UseSecondRadius = !UseSecondRadius;
                    break;
            }
        }
    
        private void SetMesh()
        {
            Tube.transform.localPosition = new Vector3(_useSecondRadius ? -SecondBendRadius : -FirstBendRadius, 0, 0);
    
            Quaternion rot = Quaternion.Euler(0, -Angle, 0);
            Vector3 pos = new Vector3(_useSecondRadius ? SecondBendRadius : FirstBendRadius, 0, 0);
            EndPoint.transform.localPosition = rot * pos - pos;
            EndPoint.transform.localRotation = rot;
            const int numberOfAngles = 180 / MeshFactory.DeltaAngle;
            Tube.GetComponent<MeshFilter>().mesh = _meshes[_angle / MeshFactory.DeltaAngle - 1 + (UseSecondRadius ? numberOfAngles : 0)];
            Tube.GetComponent<MeshCollider>().sharedMesh = Tube.GetComponent<MeshFilter>().mesh;
        }
    }
}
