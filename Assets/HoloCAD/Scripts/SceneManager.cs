// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using HoloCAD.Tubes;
using HoloCAD.Tubes.UnityTubes;
using HoloCore.FileManager;

namespace HoloCAD
{
    /// <summary> Класс управления сценой. </summary>
    public class SceneManager
    {
        /// <summary> Сохраняет сцену в выбираемый пользователем файл. </summary>
        public static void SaveScene()
        {
#pragma warning disable 4014
            FileManager.SaveAsync( SceneSerialization.SerializeScheme(TubeManager.AllTubes));
#pragma warning restore 4014
        }

        public static void SaveSceneAs()
        {
#pragma warning disable 4014
            FileManager.SaveAsAsync( SceneSerialization.SerializeScheme(TubeManager.AllTubes));
#pragma warning restore 4014
        }
        
        /// <summary> Загрузить сцену из выбираемого пользователем файла. </summary>
        public static async void LoadScene()
        {
            var file = await FileManager.OpenAsync();
            SceneSerialization.DeserializeScheme(file.Data, TubeUnityManager.Instance.StartTubeMarks);
        }
        

        #region Private definitions

        private static readonly IUnityFileManager FileManager = UnityFileManager.Create(new[] {"json"});

        #endregion
    }
}