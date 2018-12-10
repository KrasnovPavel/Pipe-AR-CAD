using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Input;

namespace HoloCAD
{
    /// <inheritdoc />
    /// <summary>
    /// Класс реализующий фланец трубы.
    /// </summary>
    public class TubeStart : BaseTube
    {
        private const float Length = 0.03f;
        private bool _isPlacing;
        private GestureRecognizer _recognizer;
        
        /// <value> Объект содержащий коллайдер комнаты. </value>
        public GameObject SpatialMapping;
    
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
            EndPoint.transform.localPosition = new Vector3(0, 0, Length);
            Diameter = 0.05f;
            TubeManager.AddTube(this);
            TubeManager.SelectTube(this);
        }
    
        void Awake()
        {
#if ENABLE_WINMD_SUPPORT
            _isPlacing = true;
            _recognizer = new GestureRecognizer();
            _recognizer.Tapped += args =>
            {
                if (_isPlacing)
                {
                    _isPlacing = false;
                    _recognizer.StopCapturingGestures();
                }
            };
            _recognizer.StartCapturingGestures();
#endif
        }
    
        /// <summary>
        /// Функция выполняющаяся в Unity каждый кадр. 
        /// </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью
        /// <c> base.Update()</c>.
        /// </remarks>
        protected void Update()
        {
            Tube.transform.localScale = new Vector3(Diameter, Length, Diameter);
            if (Diameter < 0)
            {
                Diameter = 0.05f;
            }
            Label.GetComponent<TextMesh>().text = "Диаметр: " + Diameter.ToString("0.00") + "м.";
    
            Tube.GetComponent<MeshCollider>().enabled = !_isPlacing;
            SpatialMapping.GetComponent<SpatialMappingCollider>().enableCollisions = _isPlacing;
            if (_isPlacing)
            {
                Place();
            }
        }

        private void Place()
        {
            Vector3 headPosition = Camera.main.transform.position;
            Vector3 gazeDirection = Camera.main.transform.forward;
    
            RaycastHit hitInfo;
            if (!Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f)) return;
            
            transform.position = hitInfo.point + Vector3.up * 0.02f;
            transform.rotation = Quaternion.LookRotation(hitInfo.normal);
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
                case "IncreaseDiameterButton":
                    Diameter += 0.01f;
                    break;
                case "DecreaseDiameterButton":
                    Diameter -= 0.01f;
                    break;
                case "PlacingButton":
                    _isPlacing = true;
                    break;
            }
        }
    }
}
