// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using HoloCAD.UI.Docs2D;
using UnityEngine;

namespace HoloCAD.UI
{
    /// <summary> Класс, реагирующий на события нажатий кнопок в меню. </summary>
    public class Menu : MonoBehaviour
    {
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

        /// <summary> Открытие файла с двумерным документом. </summary>
        public void Open2D()
        {
            Controller2D.OpenFile();
        }

        /// <summary> Создаёт новую трубу. </summary>
        public void NewTube()
        {
            TubeManager.CreateTube().StartPlacing();
        }
    }
}
