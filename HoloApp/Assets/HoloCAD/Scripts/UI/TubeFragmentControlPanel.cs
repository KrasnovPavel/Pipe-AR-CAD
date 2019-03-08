using UnityEngine;

namespace HoloCAD.UI
{
	/// <summary> Класс, отображающий кнопки и информацию о трубах. </summary>
	public abstract class TubeFragmentControlPanel : MonoBehaviour
	{
		/// <summary> Диаметр трубы. </summary>
		public virtual float Diameter { get; set; }

		/// <summary> Расчет местоположения панели кнопок. </summary>
		protected abstract void CalculateBarPosition();

		/// <summary> Расчет линии размеров. </summary>
		protected abstract void CalculateLine();

		/// <summary> Отображение текста. </summary>
		protected abstract void SetText();

		#region Unity event functions

		/// <summary> Функция, инициализирующая объект в Unity. </summary>
		/// <remarks>
		/// При переопределении в потомке обязательно должна вызываться с помощью <c> base.Start()</c>.
		/// </remarks>
		protected virtual void Start() 
		{
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
		}

		#endregion
	}
}
