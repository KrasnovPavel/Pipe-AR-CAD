// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Threading.Tasks;
#if ENABLE_WINMD_SUPPORT
using System;
using System.IO;
using Windows.Storage;
using Windows.Storage.Pickers;
using System.Collections.Generic;
#else
using SFB;
using UnityEngine;
#endif
using Application = UnityEngine.WSA.Application;

namespace HoloCAD.IO
{
    /// <summary>
    /// Класс, предоставляющий единый интерфейс для выбора, открытия, чтения данных из файла
    /// и записи данных в файл на разных платформах.
    /// </summary>
    public static class UnityFileManager
    {
        /// <summary> Кроссплатформенный выбор и чтение текстового файла. </summary>
        /// <param name="extensions"> Допустимые форматы файлов. </param>
        /// <returns> Строка с содержимым файла, путь к файлу. </returns>
        public static async Task<(string Data, string Path)> PickAndReadTextFileAsync(string[] extensions)
        {
            var result = new TaskCompletionSource<(string Data, string Path)>();
            Application.InvokeOnUIThread(() => PickAndReadTextFileOnDevice(extensions, result), false);
            return await result.Task;
        }

        /// <summary> Кроссплатформенный выбор и чтение бинарного файла. </summary>
        /// <param name="extensions"> Допустимые форматы файлов. </param>
        /// <returns> Байтовый массив с содержимым файла, путь к файлу. </returns>
        public static async Task<(byte[] Data, string Path)> PickAndReadBinaryFileAsync(string[] extensions)
        {
            var result = new TaskCompletionSource<(byte[] Data, string Path)>();
            Application.InvokeOnUIThread(() => PickAndReadBinaryFileOnDevice(extensions, result), false);
            return await result.Task;
        }

        /// <summary> Кроссплатформенный выбор и запись текстового файла. </summary>
        /// <param name="data"> Данные для записи. </param>
        /// <param name="extensions"> Допустимые форматы файлов. </param>
        /// <returns> Путь к выбранному файлу. </returns>
        public static async Task<string> PickAndWriteTextFileAsync(string data, string[] extensions)
        {
            var result = new TaskCompletionSource<string>();
            Application.InvokeOnUIThread(() => PickAndWriteTextFileOnDevice(data, extensions, result), false);
            return await result.Task;
        }

        /// <summary> Кроссплатформенный выбор и запись бинарного файла. </summary>
        /// <param name="data"> Данные для записи. </param>
        /// <param name="extensions"> Допустимые форматы файлов. </param>
        /// <returns> Путь к выбранному файлу. </returns>
        public static async Task<string> PickAndWriteBinaryFileAsync(byte[] data, string[] extensions)
        {
            var result = new TaskCompletionSource<string>();
            Application.InvokeOnUIThread(() => PickAndWriteBinaryFileOnDevice(data, extensions, result), false);
            return await result.Task;
        }

        #region Private definitions

        private enum PickType
        {
            Open,
            Save
        }

#if ENABLE_WINMD_SUPPORT
        /// <summary> Чтение текстового файла в очках. </summary>
        /// <param name="extensions"> Допустимые форматы файла. </param>
        /// <param name="taskCompletionSource"> Преобразователь callback в Task. </param>
        private static async void PickAndReadTextFileOnDevice(string[] extensions, 
                                                              TaskCompletionSource<(string Data, string Path)> 
                                                                  taskCompletionSource)
        {
            StorageFile file = await PickFileAsync(extensions, PickType.Open);
            if (file == null)
            {
                taskCompletionSource.SetResult((null, null));
                return;
            }

            var data = (await FileIO.ReadTextAsync(file), file.Path);
            taskCompletionSource.SetResult(data);
        }

        /// <summary> Выбор и чтение бинарного файла в очках. </summary>
        /// <param name="extensions"> Допустимые форматы файла. </param>
        /// <param name="taskCompletionSource"> Преобразователь callback в Task. </param>
        private static async void PickAndReadBinaryFileOnDevice(string[] extensions,
                                                                TaskCompletionSource<(byte[] Data, string Path)>
                                                                    taskCompletionSource)
        {
            StorageFile file = await PickFileAsync(extensions, PickType.Open);
            if (file == null)
            {
                taskCompletionSource.SetResult((null, null));
                return;
            }

            byte[] result;
            using (Stream stream = await file.OpenStreamForReadAsync())
            {
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    result = memoryStream.ToArray();
                }
            }
            var data = (result, file.Path);
            taskCompletionSource.SetResult(data);
        }
        
        /// <summary> Выбор и запись текстового файла в очках. </summary>
        /// <param name="data"> Данные. </param>
        /// <param name="extensions"> Допустимые форматы файла. </param>
        private static async void PickAndWriteTextFileOnDevice(string data, string[] extensions, 
                                                               TaskCompletionSource<string> taskCompletionSource)
        {
            StorageFile file = await PickFileAsync(extensions, PickType.Save);
            if (file == null)
            {
                taskCompletionSource.SetResult("");
                return;
            }
            taskCompletionSource.SetResult(file.Path);
            await FileIO.WriteTextAsync(file, data);
        }


        /// <summary> Выбор и запись бинарного файла в очках. </summary>
        /// <param name="data"> Данные. </param>
        /// <param name="extensions"> Допустимые форматы файла. </param>
        private static async void PickAndWriteBinaryFileOnDevice(byte[] data, string[] extensions,
                                                                 TaskCompletionSource<string> taskCompletionSource)
        {
            StorageFile file = await PickFileAsync(extensions, PickType.Save);
            if (file == null)
            {
                taskCompletionSource.SetResult("");
                return;
            }
            taskCompletionSource.SetResult(file.Path);
            await FileIO.WriteBytesAsync(file, data);
        }

        /// <summary> Выбор файла в очках для открытия. </summary>
        /// <param name="extensions"> Допустимые форматы файла. </param>
        /// <param name="pickType"> Тип диалога выбора файла. </param>
        /// <returns> Файл. </returns>
        private static async Task<StorageFile> PickFileAsync(string[] extensions, PickType pickType)
        {
            switch (pickType)
            {
                case PickType.Open:
                    FileOpenPicker openPicker = new FileOpenPicker();
                    openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                    foreach (var e in extensions)
                    {
                        openPicker.FileTypeFilter.Add("." + e);
                    }
                    return await openPicker.PickSingleFileAsync();
                case PickType.Save:
                    FileSavePicker savePicker = new FileSavePicker();
                    savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                    var ext = new List<string>();
                    foreach (var e in extensions)
                    {
                        ext.Add("." + e);
                    }
                    savePicker.FileTypeChoices.Add("", ext);
                    return await savePicker.PickSaveFileAsync();
                default:
                    return null;
            }
        }
#else
        /// <summary> Выбор и чтение текстового файла на компьютере. </summary>
        /// <param name="extensions"> Допустимые форматы файла. </param>
        /// <param name="taskCompletionSource"> Преобразователь callback в Task. </param>
        private static void PickAndReadTextFileOnDevice(string[] extensions,
                                                        TaskCompletionSource<(string Data, string Path)>
                                                            taskCompletionSource)
        {
            string path = PickFileAsync(extensions, PickType.Open);
            if (string.IsNullOrEmpty(path))
            {
                taskCompletionSource.SetResult((null, null));
                return;
            }

            var data = (System.IO.File.ReadAllText(path), path);
            taskCompletionSource.SetResult(data);
        }

        /// <summary> Выбор и чтение бинарного файла на компьютере. </summary>
        /// <param name="extensions"> Допустимые форматы файла. </param>
        /// <param name="taskCompletionSource"> Преобразователь callback в Task. </param>
        private static async void PickAndReadBinaryFileOnDevice(string[] extensions,
                                                                TaskCompletionSource<(byte[] Data, string Path)>
                                                                    taskCompletionSource)
        {
            string path = PickFileAsync(extensions, PickType.Open);
            if (string.IsNullOrEmpty(path))
            {
                taskCompletionSource.SetResult((null, null));
                return;
            }

            var data = (System.IO.File.ReadAllBytes(path), path);
            taskCompletionSource.SetResult(data);
        }

        /// <summary> Выбор и запись текстового файла на компьютере. </summary>
        /// <param name="data"> Данные. </param>
        /// <param name="extensions"> Допустимые форматы файла. </param>
        /// <param name="taskCompletionSource"> Преобразователь callback в Task. </param>
        private static async void PickAndWriteTextFileOnDevice(string data, string[] extensions,
                                                               TaskCompletionSource<string> taskCompletionSource)
        {
            string path = PickFileAsync(extensions, PickType.Save);
            taskCompletionSource.SetResult(path);
            if (string.IsNullOrEmpty(path)) return;
            System.IO.File.WriteAllText(path, data);
        }

        /// <summary> Выбор и запись бинарного файла на компьютере. </summary>
        /// <param name="data"> Данные. </param>
        /// <param name="extensions"> Допустимые форматы файла. </param>
        /// <param name="taskCompletionSource"> Преобразователь callback в Task. </param>
        private static async void PickAndWriteBinaryFileOnDevice(byte[] data, string[] extensions,
                                                                 TaskCompletionSource<string> taskCompletionSource)
        {
            string path = PickFileAsync(extensions, PickType.Save);
            taskCompletionSource.SetResult(path);
            if (string.IsNullOrEmpty(path)) return;
            System.IO.File.WriteAllBytes(path, data);
        }

        /// <summary> Выбор файла на компьютере для открытия. </summary>
        /// <param name="extensions"> Допустимые форматы файла. </param>
        /// <param name="pickType"> Тип диалога выбора файла. </param>
        /// <returns> Путь к файлу. </returns>
        private static string PickFileAsync(string[] extensions, PickType pickType)
        {
            Cursor.visible = true;
            var extensionFilter = new ExtensionFilter("", extensions);
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
#endif

        #endregion
    }
}