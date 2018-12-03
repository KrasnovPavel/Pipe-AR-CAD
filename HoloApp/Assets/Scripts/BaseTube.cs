using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.Receivers;
using UnityEngine;

namespace HoloCAD
{
    public class BaseTube : InteractionReceiver
    {
        private bool _isSelected;
    
        protected GameObject Tube;
        protected GameObject EndPoint;
        protected GameObject ButtonBar;
        public GameObject Label;
        public bool HasChild;

        private static readonly Color DefaultTubeColor = new Color(1f, 1f, 0f, 1f);
        private static readonly Color SelectedTubeColor = new Color(1f, 0f, 0f, 1f);

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
    
        public float Diameter;

        protected override void InputDown(GameObject obj, InputEventData eventData)
        {
            switch (obj.name)
            {
                case "AddBendButton":
                    TubeManager.CreateTube(EndPoint.transform, Diameter, true);
                    HasChild = true;
                    break;
                case "AddTubeButton":
                    TubeManager.CreateTube(EndPoint.transform, Diameter, false);
                    HasChild = true;
                    break;
                case "RemoveButton":
                    Destroy(gameObject);
                    break;
            }
        }

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
    }
}

