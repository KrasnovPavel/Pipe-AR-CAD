// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Threading.Tasks;

namespace HoloCore.FileManager
{
    /// <summary> Кроссплатформенный интерфейс выбора, открытия, сохранения, чтения и записи файлов. </summary>
    public interface IUnityFileManager
    {
        /// <summary> Список допустимых форматов файла. </summary>
        string[] Extensions { get; set; }
        
        /// <summary> Путь к открытому файлу. </summary>
        string OpenedFilePath { get; }
        
        /// <summary> Показывает диалог выбора файла и читает файл. </summary>
        /// <remarks> Запоминает выбранный файл. </remarks>
        /// <returns> Содержимое файла в виде массива байт. Путь к файлу. </returns>
        Task<(byte[] Data, string Path)> OpenAsync();
        
        /// <summary> Записывает данные в ранее запомненный файл. </summary>
        /// <param name="data"> Данные для записи. </param>
        /// <returns> Путь к файлу. </returns>
        Task<string> SaveAsync(byte[] data);
        
        /// <summary> Показывает диалог выбора файла и записывает данные в файл файл. </summary>
        /// <remarks> Запоминает выбранный файл. </remarks>
        /// <param name="data"> Данные для записи. </param>
        /// <returns> Путь к файлу. </returns>
        Task<string> SaveAsAsync(byte[] data);
        
        /// <summary> Показывает диалог выбора файла и читает файл. </summary>
        /// <remarks> Не запоминает выбранный файл. </remarks>
        /// <returns> Содержимое файла в виде массива байт. Путь к файлу. </returns>
        Task<(byte[] Data, string Path)> PickAndReadAsync();
        
        /// <summary> Показывает диалог выбора файла и записывает данные в файл файл. </summary>
        /// <remarks> Не запоминает выбранный файл. </remarks>
        /// <param name="data"> Данные для записи. </param>
        /// <returns> Путь к файлу. </returns>
        Task<string> PickAndWriteAsync(byte[] data);
    }
}