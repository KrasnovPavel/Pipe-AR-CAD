// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

#if ENABLE_WINMD_SUPPORT
#else
using System.Threading.Tasks;
using SFB;
using UnityEngine;
using Application = UnityEngine.WSA.Application;

namespace HoloCore.IO
{
    /// <summary>
    /// Класс для выбора, открытия, сохранения, чтения и записи файлов и использованием StandaloneFileBrowser.
    /// </summary>
    public class UnityFileManagerSFB : IUnityFileManager
    {
        /// <inheritdoc />
        public string[] Extensions { get; set; }

        /// <inheritdoc />
        public string OpenedFilePath { get; private set; }

        /// <summary> Конструктор. </summary>
        /// <param name="extensions"> Список допустимых форматов файла. </param>
        public UnityFileManagerSFB(string[] extensions)
        {
            Extensions = extensions;
        }

        /// <inheritdoc />
        public async Task<(byte[] Data, string Path)> OpenAsync()
        {
            var result = new TaskCompletionSource<(byte[] Data, string Path)>();
            Application.InvokeOnUIThread(() => PickAndReadFile(result), false);
            var file = await result.Task;
            OpenedFilePath = file.Path;
            return await result.Task;
        }

        /// <inheritdoc />
        public async Task<string> SaveAsync(byte[] data)
        {
            if (string.IsNullOrEmpty(OpenedFilePath))
            {
                return await SaveAsAsync(data);
            }

            System.IO.File.WriteAllBytes(OpenedFilePath, data);
            return OpenedFilePath;
        }

        /// <inheritdoc />
        public async Task<string> SaveAsAsync(byte[] data)
        {
            var result = new TaskCompletionSource<string>();
            Application.InvokeOnUIThread(() => PickAndWriteFile(data, result), false);
            OpenedFilePath = await result.Task;
            return await result.Task;
        }

        /// <inheritdoc />
        public async Task<(byte[] Data, string Path)> PickAndReadAsync()
        {
            var result = new TaskCompletionSource<(byte[] Data, string Path)>();
            Application.InvokeOnUIThread(() => PickAndReadFile(result), false);
            return await result.Task;
        }

        /// <inheritdoc />
        public async Task<string> PickAndWriteAsync(byte[] data)
        {
            var result = new TaskCompletionSource<string>();
            Application.InvokeOnUIThread(() => PickAndWriteFile(data, result), false);
            return await result.Task;
        }

        #region Private definitons

        private enum PickType
        {
            Open,
            Save
        }

        /// <summary> Выбор и чтение бинарного файла на компьютере. </summary>
        /// <param name="taskCompletionSource"> Преобразователь callback в Task. </param>
        private void PickAndReadFile(TaskCompletionSource<(byte[] Data, string Path)> taskCompletionSource)
        {
            string path = PickFileAsync(PickType.Open);
            if (string.IsNullOrEmpty(path))
            {
                taskCompletionSource.SetResult((null, null));
                return;
            }

            var data = (System.IO.File.ReadAllBytes(path), path);
            taskCompletionSource.SetResult(data);
        }

        /// <summary> Выбор и запись бинарного файла на компьютере. </summary>
        /// <param name="data"> Данные. </param>
        /// <param name="taskCompletionSource"> Преобразователь callback в Task. </param>
        private void PickAndWriteFile(byte[] data, TaskCompletionSource<string> taskCompletionSource)
        {
            string path = PickFileAsync(PickType.Save);
            taskCompletionSource.SetResult(path);
            if (string.IsNullOrEmpty(path)) return;
            System.IO.File.WriteAllBytes(path, data);
        }

        /// <summary> Выбор файла на компьютере для открытия. </summary>
        /// <param name="pickType"> Тип диалога выбора файла. </param>
        /// <returns> Путь к файлу. </returns>
        private string PickFileAsync(PickType pickType)
        {
            Cursor.visible = true;
            var extensionFilter = new ExtensionFilter("", Extensions);
            switch (pickType)
            {
                case PickType.Open:
                    var paths = StandaloneFileBrowser.OpenFilePanel("", "", new[] {extensionFilter}, false);
                    return paths.Length == 0 ? "" : paths[0];
                case PickType.Save:
                    return StandaloneFileBrowser.SaveFilePanel("", "", "", new[] {extensionFilter});
                default:
                    return "";
            }
        }

        #endregion
    }
}
#endif
