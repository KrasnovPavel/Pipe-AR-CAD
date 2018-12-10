using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.Receivers;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCAD
{
    /// <inheritdoc />
    /// <summary>
    /// Базовый класс трубы. От него наследуются все остальные классы труб.
    /// </summary>
    public class BaseTube : InteractionReceiver
    {
        private bool _isSelected;
    
        /// <value> Объект содержащий меш трубы. </value>
        protected GameObject Tube;
        
        /// <value> Объект указывающий конечную точку трубы. </value>
        protected GameObject EndPoint;
        
        /// <value> Панель с кнопками. </value>
        protected GameObject ButtonBar;
        
        /// <value> Надпись с информацией о трубе. </value>
        public GameObject Label;

        /// <value> Цвет трубы </value>
        private static readonly Color DefaultTubeColor = new Color(1f, 1f, 0f, 1f);
        
        /// <value> Цвет трубы когда она выбрана. </value>
        private static readonly Color SelectedTubeColor = new Color(1f, 0f, 0f, 1f);

        /// <value> Состояние трубы. Выбрана она или нет. </value>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected == value) return;
            
                _isSelected = value;
                Tube.GetComponent<MeshRenderer>().material.SetColor("_GridColor", 
                                                                    _isSelected ? SelectedTubeColor : DefaultTubeColor);
                if (ButtonBar != null)
                {
                    ButtonBar.SetActive(_isSelected);
                }
            }
        }
    
        /// <value> Диаметр трубы </value>
        public float Diameter;

        /// <summary>
        /// Функция инициализирующая трубу в Unity. 
        /// </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью
        /// <c> base.Start()</c>.
        /// </remarks>
        protected void Start()
        {
            tag = "Tube";
            Tube = transform.Find("Tube").gameObject;
            EndPoint = transform.Find("End Point").gameObject;
            Transform bb = transform.Find("Button Bar");
            if (bb == null)
            {
                bb = EndPoint.transform.Find("Button Bar");
            }
            ButtonBar = bb.gameObject;
            Label = ButtonBar.transform.Find("Label").gameObject;
        }

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
            switch (obj.name)
            {
                case "AddBendButton":
                    TubeManager.CreateTube(EndPoint.transform, Diameter, true);
                    break;
                case "AddTubeButton":
                    TubeManager.CreateTube(EndPoint.transform, Diameter, false);
                    break;
                case "RemoveButton":
                    Destroy(gameObject);
                    break;
            }
        }
    }
}

