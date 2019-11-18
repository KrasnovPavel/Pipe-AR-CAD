using HoloCore;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MarksEditor
{
    public class MarkPlaceController : Singleton<MarkPlaceController>
    {
        public GameObject Target;
    
        public void PlaceTheMark()
        {
            if (EventSystem.current.IsPointerOverGameObject() ||
                MarksController.Instance.SelectedMark == null) return;
            //TODO: Придумать, как лучше блокировать RayCast через интерфейс
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10.0f,1<<30))
            {
                MarksController.Instance.SelectedMark.transform.localPosition = hit.point;
                MarksController.Instance.SelectedMark.transform.up = hit.normal;
            }
        
        
        }
    
    }
}
