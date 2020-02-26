// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

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

        /// <summary> Событие измененения свойств объекта. </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public void ChangeIdOnTextMesh()
        {
            if (TextMeshOfNumber == null) return;
            TextMeshOfNumber.text = Convert.ToString(Id + 1);
        }
        
        #region Unity event functions

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

        #endregion

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