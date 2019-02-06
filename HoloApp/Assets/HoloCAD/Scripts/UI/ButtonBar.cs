using UnityEngine;

namespace HoloCAD.UI
{
    /// <inheritdoc />
    /// <summary>
    /// Панель кнопок.
    /// </summary>
    public class ButtonBar : MonoBehaviour {
        /// <summary> Смещение панели кнопок по направлению к камере. </summary>
        public float Offset;

        private Camera _camera;

        /// <summary>
        /// Функция, инициализирующая объект в Unity. 
        /// </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью <c> base.Start()</c>.
        /// </remarks>
        protected void Start()
        {
            _camera = Camera.main;
        }

        /// <summary>
        /// Функция, выполняющаяся в Unity каждый кадр. 
        /// </summary>
        /// <remarks>
        /// При переопределении в потомке обязательно должна вызываться с помощью
        /// <c> base.Update()</c>.
        /// </remarks>
        protected void Update ()
        {
            if (_camera != null)
            {
                Vector3 direction = (_camera.transform.position - transform.parent.position).normalized * Offset;
                Quaternion rotation = Quaternion.FromToRotation(Vector3.back, direction);
                transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
                transform.position = new Vector3(direction.x, 0, direction.z) + transform.parent.position;
            }
        }
    }
}

