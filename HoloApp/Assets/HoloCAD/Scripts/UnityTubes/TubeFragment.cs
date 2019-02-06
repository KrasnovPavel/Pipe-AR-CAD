using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.Receivers;
using UnityEngine;

namespace HoloCAD.UnityTubes
{
    /// <inheritdoc />
    /// <summary> Базовый класс участка трубы. От него наследуются все остальные классы участков труб. </summary>
    public class TubeFragment : InteractionReceiver
    {
        private bool _isSelected;
        private bool _isPlacing;
        private static readonly int GridColor = Shader.PropertyToID("_GridColor");

        /// <summary> Цвет участка трубы. </summary>
        private static readonly Color DefaultTubeColor = new Color(1f, 1f, 0f, 1f);
        
        /// <summary> Цвет участка трубы когда она выбрана. </summary>
        private static readonly Color SelectedTubeColor = new Color(1f, 0f, 0f, 1f);
        
        /// <summary> Поле хранящее диаметр участка трубы. </summary>
        /// <remarks> ВНИМАНИЕ!!! НАПРЯМУЮ ОБРАЩАТЬСЯ К НЕМУ ЗАПРЕЩЕНО!!! Используйте <see cref="Diameter"/>. </remarks>
        protected float _diameter;

        /// <summary> Объект, содержащий меш участка трубы. </summary>
        protected GameObject Tube;
        
        /// <summary> Объект, указывающий конечную точку участка трубы. </summary>
        protected GameObject EndPoint;
        
        /// <summary> Панель с кнопками. </summary>
        protected GameObject ButtonBar;
        
        /// <summary> Надпись с информацией о участке трубы. </summary>
        public GameObject Label;

        /// <summary> Объект, отображающий текстовые данные о участке трубе. </summary>
        protected TextMesh LabelText { get; private set; }

        /// <summary> Флаг, находится ли участок трубы в режиме перемещения. </summary>
        public bool IsPlacing
        {
            get { return _isPlacing; }
            set
            {
                _isPlacing = value;
                Tube.GetComponent<MeshCollider>().enabled = !_isPlacing;
            }
        }

        /// <summary> Труба-хозяин этого участка. </summary>
        public Tube Owner;
        
        /// <summary> Состояние участка трубы. Выбрана он или нет. </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected == value) return;
            
                _isSelected = value;
                Tube.GetComponent<MeshRenderer>().material.SetColor(GridColor, 
                                                                    _isSelected ? SelectedTubeColor : DefaultTubeColor);
                if (ButtonBar != null)
                {
                    ButtonBar.SetActive(_isSelected);
                }
            }
        }

        /// <summary> Диаметр участка трубы. </summary>
        public virtual float Diameter
        {
            get { return _diameter; }
            set { _diameter = value; }
        }

        /// <summary> Функция, инициализирующая участок трубы в Unity. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.Start()</c>.
        /// </remarks>
        protected virtual void Start()
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
            LabelText = Label.GetComponent<TextMesh>();
            Diameter = Owner.Data.diameter;
        }
        
        /// <summary> Функция, выполняющаяся в Unity каждый кадр. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.Update()</c>.
        /// </remarks>
        protected virtual void Update()
        {
        }

        /// <summary> Обработчик нажатия на кнопку из HoloToolKit. </summary>
        /// <param name="obj"> Нажатая кнопка. </param>
        /// <param name="eventData"> Информация о событии. </param>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.InputDown()</c>.
        /// </remarks>
        protected override void InputDown(GameObject obj, InputEventData eventData)
        {
            switch (obj.name)
            {
                case "AddBendButton":
                    Owner.CreateBendedTubeFragment(EndPoint.transform);
                    break;
                case "AddTubeButton":
                    Owner.CreateDirectTubeFragment(EndPoint.transform);
                    break;
                case "AddStartButton":
                    TubeManager.CreateTube();
                    break;
                case "RemoveButton":
                    Destroy(gameObject);
                    break;
            }
        }

        /// <summary> Обработчик удаления этого участка трубы. </summary>
        protected void OnDestroy()
        {
            Owner.OnFragmentRemoved(this);
        }
    }
}
