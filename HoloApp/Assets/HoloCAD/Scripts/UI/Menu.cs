// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using UnityEngine;

namespace HoloCAD.UI
{
    /// <summary> Класс, реагирующий на события нажатий кнопок в меню. </summary>
    public class Menu : MonoBehaviour
    {
        /// <summary> Префаб обозревателя 2D документов. </summary>
        public GameObject Viewer2DPrefab;
        
        /// <summary> Сохранение сцены. </summary>
        public void SaveScene()
        {
            TubeManager.SaveScene();
        }

        /// <summary> Загрузка сцены. </summary>
        public void LoadScene()
        {
            TubeManager.LoadScene();
        }

        /// <summary> Сохранение сцены отдельным файлом. </summary>
        public void SaveSceneAs()
        {
            TubeManager.SaveSceneAs();
        }

        /// <summary> Выход из программы. </summary>
        public void QuitApp()
        {
            Application.Quit();
        }

        public void Open2D()
        {
            Transform t = Camera.main.transform;
            var pos = Quaternion.AngleAxis(t.rotation.eulerAngles.y, Vector3.up) * Vector3.forward * 1.5f + t.position;
            Instantiate(Viewer2DPrefab, pos, Quaternion.AngleAxis(t.rotation.eulerAngles.y, Vector3.up));
        }
    }
}