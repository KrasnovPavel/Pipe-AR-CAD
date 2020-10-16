// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using HoloCAD.NewTubeConcept.View;
using HoloCore.Docs2D;
using UnityEngine;

namespace HoloCAD.UI
{
    /// <summary> Класс, реагирующий на события нажатий кнопок в меню. </summary>
    public class Menu : MonoBehaviour
    {
        /// <summary> Сохранение сцены. </summary>
        public void SaveScene()
        {
            SceneManager.SaveScene();
        }

        /// <summary> Загрузка сцены. </summary>
        public void LoadScene()
        {
            SceneManager.LoadScene();
        }

        /// <summary> Сохранение сцены отдельным файлом. </summary>
        public void SaveSceneAs()
        {
            SceneManager.SaveSceneAs();
        }

        /// <summary> Выход из программы. </summary>
        public void QuitApp()
        {
            Application.Quit();
        }

        /// <summary> Открытие файла с двумерным документом. </summary>
        public void Open2D()
        {
            Controller2D.OpenFile();
        }

        /// <summary> Создаёт новую трубу. </summary>
        public void NewTube()
        {
            var go = Instantiate(TubePrefabsContainer.Instance.FlangeView);
            go.GetComponent<FlangeView>().StartPlacement();
        }
    }
}
