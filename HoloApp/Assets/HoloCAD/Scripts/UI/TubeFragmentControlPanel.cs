// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using HoloCore.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace HoloCAD.UI
{
	/// <summary> Класс, отображающий кнопки и информацию о трубах. </summary>
	public abstract class TubeFragmentControlPanel : MonoBehaviour
	{
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
		
		/// <summary> Расчет местоположения панели кнопок. </summary>
		protected abstract void CalculateBarPosition();

		/// <summary> Расчет линий размеров. </summary>
		protected abstract void CalculateLine();

		/// <summary> Отображение текста. </summary>
		protected abstract void SetText();

		/// <summary> Функция, инициализирующая кнопки. </summary>
		/// <remarks>
		/// При переопределении в потомке обязательно должна вызываться с помощью <c> base.InitButtons()</c>.
		/// </remarks>
		protected abstract void InitButtons();

		/// <summary> Функция, проверяющая какие кнопки должны быть выключены. </summary>
		protected abstract void CheckIsButtonsEnabled();

		#region Unity event functions

		/// <summary> Функция, инициализирующая объект в Unity. </summary>
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
			SetText();
			CalculateBarPosition();
			CalculateLine();
			CheckIsButtonsEnabled();
		}

		#endregion
	}
}
