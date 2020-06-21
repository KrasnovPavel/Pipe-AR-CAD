// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.ObjectModel;

#if ENABLE_WINMD_SUPPORT
using System.Linq;
using System.Collections.Generic;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;
using UnityEngine;
using Application = UnityEngine.WSA.Application;
#endif

namespace HoloCAD.Bluetooth
{
    /// <summary> Класс для подключения к лазерному дальномеру и получения данных с него. </summary>
    public static class BluetoothDataProvider
    {
        /// <summary> Событие, вызываемое при подключении устройства. </summary>
        public static event Action<string> DeviceConnected;

        /// <summary> Событие, вызываемое при отключении устройства. </summary>
        public static event Action<string> DeviceDisconnected;

        /// <summary> Событие, вызываемое при начале процесса подключения к устройству. </summary>
        public static event Action<string> ConnectingToDevice;

        /// <summary> Событие, вызываемое при завершении поиска устройств. </summary>
        public static event Action EnumerationEnded;

        /// <summary> Событие, вызываемое при получении новых данных от устройства. </summary>
        public static event Action<float> NewDataReceived;

        /// <summary> Коллекция имён найденных устройств. </summary>
        public static ObservableCollection<string> Devices { get; } = new ObservableCollection<string>();

        /// <summary> Запускает поиск устройств. </summary>
        public static void FindDevices()
        {
            Devices.Clear();

#if ENABLE_WINMD_SUPPORT
            _devices.Clear();
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };
            _deviceWatcher = DeviceInformation.CreateWatcher(
                                BluetoothLEDevice.GetDeviceSelectorFromPairingState(false),
                                requestedProperties,
                                DeviceInformationKind.AssociationEndpoint);

            _deviceWatcher.Added += DeviceWatcher_Added;
            _deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            _deviceWatcher.Start();
#endif
        }

        /// <summary> Подключение к указанному устройству. </summary>
        /// <param name="name"> Имя устройства. </param>
        public static async void ConnectToDevice(string name)
        {
#if ENABLE_WINMD_SUPPORT
            _connectedDevice = _devices.Find(d => d.Name == name);
            if (_connectedDevice == null) return;
            ConnectingToDevice?.Invoke(name);

            GattDeviceServicesResult result = await _connectedDevice.GetGattServicesAsync(BluetoothCacheMode.Uncached);
            _connectedDevice.ConnectionStatusChanged += ConnectedDevice_ConnectionStatusChanged;


            if (result.Status == GattCommunicationStatus.Success)
            {
                var services = result.Services;

                _connectedService = result.Services.First(s => s.Uuid == Guid.Parse("0000cbbb-0000-1000-8000-00805f9b34fb"));
                var characteristics = await _connectedService.GetCharacteristicsAsync();
                _connectedCharacteristic = characteristics.Characteristics.First(ch => ch.Uuid == Guid.Parse("0000cbb1-0000-1000-8000-00805f9b34fb"));
                _connectedCharacteristic.ValueChanged += Characteristic_ValueChanged;

                await _connectedCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);

                DeviceConnected?.Invoke(name);
            }
#endif
        }

        /// <summary> Отключиться от устройства. </summary>
        public static void DisconnectFromDevice()
        {
#if ENABLE_WINMD_SUPPORT
            _connectedCharacteristic = null;
            _connectedService.Dispose();
            _connectedService = null;
            _connectedDevice.Dispose();
            _connectedDevice = null;
            _devices.Clear();
            Devices.Clear();
#endif
        }

        #region Private definitions

#if ENABLE_WINMD_SUPPORT

        /// <summary> Обработчик завершения поиска устройств. </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            _deviceWatcher.Stop();
            Application.InvokeOnAppThread(() => EnumerationEnded?.Invoke(), false);
        }

        /// <summary> Обработчик события нахождения нового устройства. </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            _devices.Add(await BluetoothLEDevice.FromIdAsync(args.Id));
            Application.InvokeOnAppThread(() => Devices.Add(args.Name), false);
        }

        /// <summary> Обработчик получения новых данных с устройства. </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static async void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            var data = reader.ReadString(10).Trim();
            data = data.Substring(1, data.Length - 2);
            float length;
            if (float.TryParse(data, out length))
            {
                NewDataReceived?.Invoke(length);
            }
        }

        /// <summary> Обработчик отключения устройства. </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void ConnectedDevice_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
        {
            if (sender.ConnectionStatus == BluetoothConnectionStatus.Disconnected)
            {
                Application.InvokeOnAppThread(() => DeviceDisconnected?.Invoke(sender.Name), false);
            }
        }

        /// <summary> Список найденных устройств. </summary>
        private static List<BluetoothLEDevice> _devices = new List<BluetoothLEDevice>();

        // Ссылки на подключенное устройство и его сервис.
        // Необходимы для того, чтобы GC не удалил сервис,
        // иначе перестанут приходить данные с устройтсва.
        private static BluetoothLEDevice _connectedDevice;
        private static GattDeviceService _connectedService;
        private static GattCharacteristic _connectedCharacteristic;
        
        private static DeviceWatcher _deviceWatcher;
#endif

        #endregion
    }
}
