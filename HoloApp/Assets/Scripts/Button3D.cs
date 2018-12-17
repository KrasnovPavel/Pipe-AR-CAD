using UnityEngine;

namespace HoloCAD
{

    public class Button3D : MonoBehaviour
    {

        public enum State
        {
            Disabled, Enabled, Hovered, Pressed
        }

        private State _state;

        public void SetState(State newState)
        {
            this._state = newState;
        }

        public void OnClick()
        {
            Debug.Log("OnClick");
        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
