// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.ComponentModel;
using HoloCAD.UnityTubes;
using HoloCore;
using HoloCore.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCAD.UI
{
    /// <summary> Класс, отображающий кнопки и информацию о трубах. </summary>
    public abstract class TubeFragmentControlPanel : MonoBehaviour, ISelectable 
    {
        /// <summary> Панель с кнопками и информацией о трубе. </summary>
        [Tooltip("Панель с кнопками и информацией о трубе.")]
        [CanBeNull] public Transform ButtonBar;
        
        [CanBeNull] public TextMesh TextLabel;
        
        /// <summary> Кнопка добавления погиба. </summary>
        [Tooltip("Кнопка добавления погиба.")]
        [CanBeNull] public Button3D AddBendFragmentButton;

        /// <summary> Кнопка добавления прямого участка трубы. </summary>
        [Tooltip("Кнопка добавления прямого участка трубы.")]
        [CanBeNull] public Button3D AddDirectFragmentButton;

        /// <summary> Кнопка удаления этого участка трубы. </summary>
        [Tooltip("Кнопка удаления этого участка трубы.")]
        [CanBeNull] public Button3D RemoveThisFragmentButton;
        
        /// <summary> Кнопка добавления объекта отображения расстояния между трубами. </summary>
        [Tooltip("Кнопка добавления объекта отображения расстояния между трубами.")]
        [CanBeNull] public Button3D ConnectTubesButton;
        
        /// <summary> Кнопка перехода на следующий отрезок трубы. </summary>
        [Tooltip("Кнопка перехода на следующий отрезок трубы.")]
        [CanBeNull] public Button3D NextFragmentButton;
        
        /// <summary> Кнопка перехода на предыдущий отрезок трубы. </summary>
        [Tooltip("Кнопка перехода на предыдущий отрезок трубы.")]
        [CanBeNull] public Button3D PreviousFragmentButton;

        /// <summary> Участок трубы, которому принадлежит эта панель. </summary>
        protected TubeFragment BaseFragment;

        /// <inheritdoc/>
        public bool IsSelected => SelectableObject.SelectedObject == gameObject;

        /// <inheritdoc/>
        public void ToggleSelection()
        {
            GetComponent<SelectableObject>().ToggleSelection();
        }
        
        /// <inheritdoc/>
        public virtual void SelectThis()
        {
            GetComponent<SelectableObject>().SelectThis();
        }

        /// <inheritdoc/>
        public virtual void DeselectThis()
        {
            GetComponent<SelectableObject>().DeselectThis();
        }

        /// <inheritdoc/>
        public virtual void OnSelect()
        {
            GamepadController.SubscribeToClick(GamepadController.InputAxis.JoystickY, 
                                               null, 
                                               BaseFragment.TogglePlacing);
            GamepadController.SubscribeToClick(GamepadController.InputAxis.JoystickB,
                                               null,
                                               BaseFragment.Owner.CreateTubesConnector);
            GamepadController.SubscribeToClick(GamepadController.InputAxis.LeftBumper,
                                               null,
                                                BaseFragment.AddBendFragment);
            GamepadController.SubscribeToClick(GamepadController.InputAxis.RightBumper,
                                               null,
                                               BaseFragment.AddDirectFragment);
            GamepadController.SubscribeToClick(GamepadController.InputAxis.JoystickX,
                                               null,
                                               BaseFragment.CreateTube);
            GamepadController.SubscribeToClick(GamepadController.InputAxis.DPADRight,
                                               null,
                                               BaseFragment.SelectChild);
            GamepadController.SubscribeToClick(GamepadController.InputAxis.DPADLeft,
                                               null,
                                               BaseFragment.SelectParent);
            GamepadController.SubscribeToClick(GamepadController.InputAxis.DPADDown,
                                               null,
                                               BaseFragment.RemoveThisFragment);
        }

        /// <inheritdoc/>
        public virtual void OnDeselect()
        {
            
            GamepadController.UnsubscribeFromClick(GamepadController.InputAxis.JoystickY, 
                                                   null, 
                                                   BaseFragment.TogglePlacing);
            GamepadController.UnsubscribeFromClick(GamepadController.InputAxis.JoystickB,
                                                   null,
                                                   BaseFragment.Owner.CreateTubesConnector);
            GamepadController.UnsubscribeFromClick(GamepadController.InputAxis.LeftBumper,
                                                   null,
                                                   BaseFragment.AddBendFragment);
            GamepadController.UnsubscribeFromClick(GamepadController.InputAxis.RightBumper,
                                                   null,
                                                   BaseFragment.AddDirectFragment);
            GamepadController.UnsubscribeFromClick(GamepadController.InputAxis.JoystickX,
                                                   null,
                                                   BaseFragment.CreateTube);
            GamepadController.UnsubscribeFromClick(GamepadController.InputAxis.DPADRight,
                                                   null,
                                                   BaseFragment.SelectChild);
            GamepadController.UnsubscribeFromClick(GamepadController.InputAxis.DPADLeft,
                                                   null,
                                                   BaseFragment.SelectParent);
            GamepadController.UnsubscribeFromClick(GamepadController.InputAxis.DPADDown,
                                                   null,
                                                   BaseFragment.RemoveThisFragment);
        }
        
        /// <summary> Расчет местоположения панели кнопок. </summary>
        protected abstract void CalculateBarPosition();

        /// <summary> Функция, инициализирующая кнопки. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.InitButtons()</c>.
        /// </remarks>
        protected virtual void InitButtons()
        {
            if (AddBendFragmentButton != null)
            {
                AddBendFragmentButton.OnClick += delegate { BaseFragment.AddBendFragment(); };
            }
            if (AddDirectFragmentButton != null)
            {
                AddDirectFragmentButton.OnClick += delegate { BaseFragment.AddDirectFragment(); };
            }
            if (RemoveThisFragmentButton != null)
            {
                RemoveThisFragmentButton.OnClick += delegate { BaseFragment.RemoveThisFragment(); };
            }
            if (ConnectTubesButton != null)
            {
                ConnectTubesButton.OnClick += delegate { BaseFragment.Owner.CreateTubesConnector(); };
            }
            if (NextFragmentButton != null)
            {
                NextFragmentButton.OnClick += delegate { BaseFragment.SelectChild(); };
            }
            if (PreviousFragmentButton != null)
            {
                PreviousFragmentButton.OnClick += delegate { BaseFragment.SelectParent(); };
            }
        }

        /// <summary> Функция, для отслеживания и переключения состояния основных кнопок. </summary>
        /// <param name="fragment"> Участок трубы. </param>
        protected void CheckIsButtonsEnabled(TubeFragment fragment)
        {
            fragment.Owner.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == nameof(Tube.HasTubesConnector))
                {
                    CheckConnectButton(fragment);
                }
            };
            TubeUnityManager.Instance.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == nameof(TubeUnityManager.ActiveTubesConnector))
                {
                    CheckConnectButton(fragment);
                }
            };
            fragment.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == nameof(TubeFragment.Child))
                {
                    CheckChildButtons(fragment);
                }
            }; 
            CheckConnectButton(fragment);
            CheckChildButtons(fragment);
        }
        
        /// <summary> Включает или выключает кнопку "Соединить". </summary>
        /// <param name="fragment"> Участок трубы. </param>
        protected void CheckConnectButton(TubeFragment fragment)
        {
            if (ConnectTubesButton != null) ConnectTubesButton.SetEnabled(!fragment.Owner.HasTubesConnector 
                                                                          && !TubeUnityManager.HasActiveTubesConnector);
        }

        /// <summary> Включает или выключает кнопки, работающие с детьми. </summary>
        /// <param name="fragment"> Участок трубы. </param>
        protected void CheckChildButtons(TubeFragment fragment)
        {
            if (AddBendFragmentButton != null) AddBendFragmentButton.SetEnabled(!fragment.HasChild);
            if (AddDirectFragmentButton != null) AddDirectFragmentButton.SetEnabled(!fragment.HasChild);
            if (NextFragmentButton != null) NextFragmentButton.SetEnabled(fragment.HasChild);
        }

        #region Unity event functions
        
        /// <summary> Функция, выполняющаяся после инициализизации участка трубы в Unity. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.Start()</c>.
        /// </remarks>
        protected virtual void Start()
        {
            InitButtons();
        }
    
        /// <summary> Функция, выполняющаяся в Unity каждый кадр. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.Update()</c>.
        /// </remarks>
        protected virtual void Update() 
        {
            CalculateBarPosition();
        }
        
        /// <summary> Функция, выполняющаяся в Unity при выключении объекта. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.OnDisable()</c>.
        /// </remarks>
        protected virtual void OnDisable()
        {
            if (ButtonBar != null) ButtonBar.gameObject.SetActive(false);
        }

        /// <summary> Функция, выполняющаяся в Unity при выключении объекта. </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.OnEnable()</c>.
        /// </remarks>
        protected virtual void OnEnable()
        {
            if (ButtonBar != null) ButtonBar.gameObject.SetActive(true);
        }

        #endregion
    }
}
