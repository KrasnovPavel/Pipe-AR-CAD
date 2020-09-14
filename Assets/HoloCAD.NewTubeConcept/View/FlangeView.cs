using System;
using UnityEngine;
using HoloCAD.NewTubeConcept.Model;

namespace HoloCAD.NewTubeConcept.View
{
    public class FlangeView : MonoBehaviour
    {
        public Flange flange;

        private void Update()
        {
            if (transform.hasChanged)
            {
                flange.Move(transform.position, transform.forward);
                transform.hasChanged = false;
            }
        }
    }
}