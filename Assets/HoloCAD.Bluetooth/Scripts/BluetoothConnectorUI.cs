// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Collections;
using System.Collections.Specialized;
using HoloCore;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace HoloCAD.Bluetooth
{
    /// <summary> Класс виджета подключения к Bluetooth-устройству. </summary>
    public class BluetoothConnectorUI : Singleton<BluetoothConnectorUI>
    {
        /// <summary> Виджета-коллекция кнопок. </summary>
        public Transform ScrollBar;

        /// <summary> Префаб кнопки выбора устройства. </summary>
        public GameObject ButtonPrefab;

        /// <summary> Виджет-индикатор загрузки </summary>
        public GameObject ProgressIndicator;

        /// <summary> Кнопка для отключения устройства. </summary>
        public GameObject DeviceButton;

        /// <summary> Кнопка для обновления списка устройств. </summary>
        public GameObject UpdateButton;

        /// <summary> Текстовое поле. </summary>
        public GameObject TextLabel;

        /// <summary> Контейнер для вложенных виджетов. </summary>
        public GameObject Container;

        /// <summary> Расстояние от пользователя на котором появится этот виджет. </summary>
        [Range(0.1f, 2)]
        public float DistanceFromHead = 0.7f;

        /// <summary> Вызывает этот виджет. </summary>
        public static void ShowConnectionWindow()
        {
            Instance.Container.SetActive(true);
            var tr = Instance.Container.transform;
            tr.localPosition = Instance._camera.position + Instance._camera.forward * Instance.DistanceFromHead;
            
            tr.LookAt(Instance._camera);
            Vector3 angles = tr.rotation.eulerAngles;
            tr.rotation = Quaternion.Euler(new Vector3(0, angles.y, 0));

            Instance.Rescan();
        }

        /// <summary> Выполняет поиск устройств. </summary>
        public void Rescan()
        {
            BluetoothDataProvider.FindDevices();
            _progressIndicator.OpenAsync();
            _progressIndicator.Message = "Поиск устройств";
        }

        /// <summary> Сворчивает виджет. </summary>
        public void Hide()
        {
            Instance.Container.SetActive(false);
        }

        #region Unity event functions
        
        /// <summary> Функция, выполняющаяся после инициализизации виджета в Unity. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.Start()</c>.
        /// </remarks>
        void Start()
        {
            // ReSharper disable once PossibleNullReferenceException
            _camera = Camera.main.transform;
            _progressIndicator = ProgressIndicator.GetComponent<ProgressIndicatorOrbsRotator>();

            var deviceButton = DeviceButton.GetComponent<Interactable>();
            deviceButton.OnClick.AddListener(BluetoothDataProvider.DisconnectFromDevice);

            BluetoothDataProvider.EnumerationEnded += delegate { _progressIndicator.CloseAsync(); };
            BluetoothDataProvider.DeviceConnected += DeviceConnected;
            BluetoothDataProvider.DeviceDisconnected += DeviceDisconnected;
            BluetoothDataProvider.ConnectingToDevice += delegate {
                                                            _progressIndicator.OpenAsync();
                                                            _progressIndicator.Message = "Соединение"; 
                                                        };

            BluetoothDataProvider.FindDevices();
            BluetoothDataProvider.Devices.CollectionChanged +=
                delegate(object sender, NotifyCollectionChangedEventArgs args)
                {
                    if (args.Action == NotifyCollectionChangedAction.Add)
                    {
                        AddButtons(args.NewItems);
                    }
                    else if (args.Action == NotifyCollectionChangedAction.Remove)
                    {
                        RemoveButtons(args.OldItems);
                    }

                    Instance.ScrollBar.GetComponent<ScrollingObjectCollection>().UpdateCollection();
                };
        }

        #endregion
       
        #region Private definitions

        /// <summary> Обработчик подключения к устройству. </summary>
        /// <param name="deviceName"> Имя подключенного устройства. </param>
        private void DeviceConnected(string deviceName)
        {
            DeviceButton.SetActive(true);
            TextLabel.SetActive(true);
            UpdateButton.SetActive(false);
            ProgressIndicator.SetActive(false);
            DeviceButton.GetComponent<ButtonConfigHelper>().MainLabelText = deviceName;
            ScrollBar.gameObject.SetActive(false);
        }

        /// <summary> Обработчик отключения от устройства. </summary>
        /// <param name="deviceName"> Имя отключенного устройства. </param>
        private void DeviceDisconnected(string deviceName)
        {
            DeviceButton.SetActive(false);
            TextLabel.SetActive(false);
            UpdateButton.SetActive(true);
            ProgressIndicator.SetActive(true);
            _progressIndicator.Message = "Поиск устройств";
            ScrollBar.gameObject.SetActive(true);
            Rescan();
        }

        /// <summary> Добаваляет кнопки для подключения к устройству. </summary>
        /// <param name="deviceNames"> Имена добавляемых устройств. </param>
        private static void AddButtons(IList deviceNames)
        {
            foreach (string deviceName in deviceNames)
            {
                var go = Instantiate(Instance.ButtonPrefab, Instance.ScrollBar);
                go.name = deviceName;
                var buttonConfig = go.GetComponent<ButtonConfigHelper>();
                buttonConfig.MainLabelText = deviceName;
                buttonConfig.SeeItSayItLabelText = deviceName;
                buttonConfig.SetQuadIconByName("IconAdd");
                go.GetComponent<PressableButton>().ReleaseOnTouchEnd = false;
                go.GetComponent<Interactable>().OnClick
                  .AddListener(delegate { BluetoothDataProvider.ConnectToDevice(deviceName); });
            }
        }

        /// <summary> Удаляет кнопки для подключения к устройству. </summary>
        /// <param name="deviceNames"> Имена удаляемых устройств. </param>
        private static void RemoveButtons(IList deviceNames)
        {
            // TODO: Когда ScrollObjectCollection выйдет из статуса Experimental проверить,
            // TODO: что удаление кнопок работает корректно. Сейчас оно не работает.
            // TODO: См.: https://github.com/microsoft/MixedRealityToolkit-Unity/issues/8064
            foreach (string deviceName in deviceNames)
            {
                Destroy(Instance.ScrollBar.Find("Container").Find(deviceName)?.gameObject);
            }
        }

        private Transform _camera;

        private ProgressIndicatorOrbsRotator _progressIndicator;

        #endregion
    }
}