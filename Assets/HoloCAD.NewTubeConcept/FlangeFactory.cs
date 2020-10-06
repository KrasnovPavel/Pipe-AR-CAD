using HoloCAD.NewTubeConcept.Model;
using HoloCAD.NewTubeConcept.View;
using UnityEngine;

namespace HoloCAD.NewTubeConcept
{
    public class FlangeFactory
    {
        public static Flange Create(bool startPlacement = false)
        {
            var res = new Flange(GCMSystemBehaviour.System);
            var view = GameObject.Instantiate(TubePrefabsContainer.Instance.FlangeView).GetComponent<FlangeView>();
            view.flange = res;
            // ReSharper disable once PossibleNullReferenceException
            var camera = Camera.main.transform;
            res.Origin = camera.position + camera.forward;
            res.Normal = -camera.forward;
            if (startPlacement) view.StartPlacement();
            return res;
        }
    }
}