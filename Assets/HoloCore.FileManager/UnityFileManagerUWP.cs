// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

#if ENABLE_WINMD_SUPPORT

using System.Threading.Tasks;
using System;
using System.IO;
using Windows.Storage;
using Windows.Storage.Pickers;
using System.Collections.Generic;

using Application = UnityEngine.WSA.Application;

namespace HoloCore.FileManager
{
    public class UnityFileManagerUWP : IUnityFileManager
    {
        /// <inheritdoc />
        public string[] Extensions { get; set; }

        /// <inheritdoc />
        public string OpenedFilePath => _file.Path;

        /// <summary> Конструктор. </summary>
        /// <param name="extensions"> Список допустимых форматов файла. </param>
        public UnityFileManagerUWP(string[] extensions)
        {
            Extensions = extensions;
        }

        /// <inheritdoc />
        public async Task<(byte[] Data, string Path)> OpenAsync()
        {
            var result = new TaskCompletionSource<(byte[] Data, StorageFile File)>();
            Application.InvokeOnUIThread(() => PickAndReadFileOnDevice(result), false);
            var file = await result.Task;
            _file = file.File;
            return (file.Data, file.File.Path);
        }

        /// <inheritdoc />
        public async Task<string> SaveAsync(byte[] data)
        {
            if (_file == null)
            {
                return await SaveAsAsync(data);
            }
            await FileIO.WriteBytesAsync(_file, data);
            return _file.Path;
        }

        /// <inheritdoc />
        public async Task<string> SaveAsAsync(byte[] data)
        {
            var result = new TaskCompletionSource<StorageFile>();
            Application.InvokeOnUIThread(() => PickAndWriteFileOnDevice(data, result), false);
            _file = await result.Task;
            return _file.Path;
        }

        /// <inheritdoc />
        public async Task<(byte[] Data, string Path)> PickAndReadAsync()
        {
            var result = new TaskCompletionSource<(byte[] Data, StorageFile File)>();
            Application.InvokeOnUIThread(() => PickAndReadFileOnDevice(result), false);
            var file = await result.Task;
            return (file.Data, file.File.Path);
        }

        /// <inheritdoc />
        public async Task<string> PickAndWriteAsync(byte[] data)
        {
            var result = new TaskCompletionSource<StorageFile>();
            Application.InvokeOnUIThread(() => PickAndWriteFileOnDevice(data, result), false);
            var file = await result.Task;
            return file.Path;
        }

        #region Private definitions

        private StorageFile _file;

        private enum PickType
        {
            Open,
            Save
        }

        /// <summary> Выбор и чтение файла в очках. </summary>
        /// <param name="taskCompletionSource"> Преобразователь callback в Task. </param>
        private async void PickAndReadFileOnDevice(TaskCompletionSource<(byte[] Data, StorageFile File)> taskCompletionSource)
        {
            StorageFile file = await PickFileAsync(PickType.Open);
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
            var data = (result, file);
            taskCompletionSource.SetResult(data);
        }

        /// <summary> Выбор и запись файла в очках. </summary>
        /// <param name="data"> Данные. </param>
        private async void PickAndWriteFileOnDevice(byte[] data,TaskCompletionSource<StorageFile> taskCompletionSource)
        {
            StorageFile file = await PickFileAsync(PickType.Save);
            if (file == null)
            {
                taskCompletionSource.SetResult(null);
                return;
            }
            taskCompletionSource.SetResult(file);
            await FileIO.WriteBytesAsync(file, data);
        }

        /// <summary> Выбор файла в очках для открытия. </summary>
        /// <param name="pickType"> Тип диалога выбора файла. </param>
        /// <returns> Файл. </returns>
        private async Task<StorageFile> PickFileAsync(PickType pickType)
        {
            switch (pickType)
            {
                case PickType.Open:
                    FileOpenPicker openPicker = new FileOpenPicker();
                    openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                    foreach (var e in Extensions)
                    {
                        openPicker.FileTypeFilter.Add("." + e);
                    }
                    return await openPicker.PickSingleFileAsync();
                case PickType.Save:
                    FileSavePicker savePicker = new FileSavePicker();
                    savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                    var ext = new List<string>();
                    foreach (var e in Extensions)
                    {
                        ext.Add("." + e);
                    }
                    savePicker.FileTypeChoices.Add("", ext);
                    return await savePicker.PickSaveFileAsync();
                default:
                    return null;
            }
        }
        
        #endregion
    }
}
#endif
