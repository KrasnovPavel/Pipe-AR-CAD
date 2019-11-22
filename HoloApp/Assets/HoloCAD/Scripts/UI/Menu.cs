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
    }
}