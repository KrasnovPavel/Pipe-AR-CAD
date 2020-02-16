using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace GLTFConverter
{
    public class Mark : MonoBehaviour, INotifyPropertyChanged
    {
        public TextMesh TextMeshOfNumber;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected { get; set; }

        /// <summary> Событие измененения свойств объекта </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        void Start()
        {
            ChangeIdOnTextMesh();

            PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == nameof(Id))
                {
                    ChangeIdOnTextMesh();
                }
            };
        }

        private void Update()
        {
            if (transform.hasChanged)
            {
                transform.hasChanged = false;
            }
        }

        public void ChangeIdOnTextMesh()
        {
            if (TextMeshOfNumber == null) return;
            TextMeshOfNumber.text = Convert.ToString(Id + 1);
        }

        #region Private defenitions

        private int _id;

        #endregion

        #region Protected defenitions

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}