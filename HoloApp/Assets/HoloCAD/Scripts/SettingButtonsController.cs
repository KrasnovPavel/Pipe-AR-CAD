using System.Collections.Generic;
using HoloCAD.UnityTubes;
using HoloCore;
using HoloCore.UI;
using UnityEngine;

namespace HoloCAD
{
    public class SettingButtonsController : Singleton<SettingButtonsController>
    {
        /// <summary> Список трехмерных кнопок для смены цели привязки со стен на модель и наоборот. </summary>
        [Tooltip("Список трехмерных кнопок для смены цели привязки со стен на модель и наоборот")]
        public List<Button3D> ChangeTargetColliderButtons;
        
        void Start()
        {
            foreach (Button3D button in ChangeTargetColliderButtons)
            {
                button.OnClick += delegate { TubeUnityManager.UseSpatialMapping = !TubeUnityManager.UseSpatialMapping; };
            }
        }
    }
}
