// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Text;
using System.Threading.Tasks;

namespace HoloCore.FileManager
{
    /// <summary>
    /// Класс, предоставляющий единый интерфейс для выбора, открытия, чтения данных из файла
    /// и записи данных в файл на разных платформах.
    /// </summary>
    public static class UnityFileManager
    {
        /// <summary> Создает UnityFileManager соответствующий текущему окружению. </summary>
        /// <param name="extensions"> Допустимые форматы файлов. </param>
        /// <returns> Интерфейс файлового менеджера. </returns>
        public static IUnityFileManager Create(string[] extensions)
        {
#if ENABLE_WINMD_SUPPORT
            return new UnityFileManagerUWP(extensions);
#else
            return new UnityFileManagerSFB(extensions);
#endif
        }

        /// <summary> Кроссплатформенный выбор и чтение текстового файла. </summary>
        /// <param name="extensions"> Допустимые форматы файлов. </param>
        /// <returns> Байтовый массив с содержимым файла, путь к файлу. </returns>
        public static async Task<(string Data, string Path)> PickAndReadTextFileAsync(string[] extensions)
        {
            var fileManager = Create(extensions);
            var file = await fileManager.PickAndReadAsync();
            return (Encoding.UTF8.GetString(file.Data), file.Path);
        }

        /// <summary> Кроссплатформенный выбор и чтение бинарного файла. </summary>
        /// <param name="extensions"> Допустимые форматы файлов. </param>
        /// <returns> Байтовый массив с содержимым файла, путь к файлу. </returns>
        public static async Task<(byte[] Data, string Path)> PickAndReadBinaryFileAsync(string[] extensions)
        {
            var fileManager = Create(extensions);
            return await fileManager.PickAndReadAsync();
        }
        
        /// <summary> Кроссплатформенный выбор и запись текстового файла. </summary>
        /// <param name="data"> Данные для записи. </param>
        /// <param name="extensions"> Допустимые форматы файлов. </param>
        /// <returns> Путь к выбранному файлу. </returns>
        public static async Task<string> PickAndWriteTextFileAsync(string data, string[] extensions)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(data);
            var fileManager = Create(extensions);
            return await fileManager.PickAndWriteAsync(byteArray);
        }
        
        /// <summary> Кроссплатформенный выбор и запись бинарного файла. </summary>
        /// <param name="data"> Данные для записи. </param>
        /// <param name="extensions"> Допустимые форматы файлов. </param>
        /// <returns> Путь к выбранному файлу. </returns>
        public static async Task<string> PickAndWriteBinaryFileAsync(byte[] data, string[] extensions)
        {
            var fileManager = Create(extensions);
            return await fileManager.PickAndWriteAsync(data);
        }
    }
}