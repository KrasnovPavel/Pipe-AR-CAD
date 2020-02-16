using HoloCore;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GLTFConverter
{
    /// <summary>Класс, отвечающий за привязку метки к поверхности</summary>
    public class MarkPlaceController : Singleton<MarkPlaceController>
    {
        /// <summary>Группа объектов, которые привязываются к метке </summary>
        public GameObject Target;

        /// <summary>Обробаетывает клик на поверхность </summary>
        public void PlaceTheMark()
        {
            if (EventSystem.current.IsPointerOverGameObject() ||
                MarksController.Instance.SelectedMark == null) return;
            //TODO: Придумать, как лучше блокировать RayCast через интерфейс
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10.0f, 1 << 30))
            {
                Transform transformOfSelectedMark = MarksController.Instance.SelectedMark.transform;
                transformOfSelectedMark.localPosition = hit.point;
                transformOfSelectedMark.up = hit.normal;
                if (transformOfSelectedMark.hasChanged)
                {
                    MarksController.Instance.ParamPanelOfSelectedMark.MarkTransformIntoInput();
                }
            }
        }
    }
}