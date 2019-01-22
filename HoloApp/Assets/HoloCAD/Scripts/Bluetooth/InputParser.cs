using System;
using UnityEngine;

namespace HoloCAD.Bluetooth
{
	public class InputParser : MonoBehaviour {
		private enum BluetoothState
		{
			Disconnected,
			InSearch,
			Connected
		}

		[Serializable]
		private class InputJson
		{
			public string Axis;
			public float Value;
		}

		private BluetoothMessenger _bluetoothMessenger;

		private BluetoothState _state = BluetoothState.Disconnected;

		public void StartSearch()
		{
			if (_state == BluetoothState.Disconnected)
			{
				_state = BluetoothState.InSearch;
			}
		}

		public void Disconnect()
		{
			if (_state == BluetoothState.Connected)
			{
				_state = BluetoothState.Disconnected;
				//TODO: actual disconnect
			}
		}

		public void MessageReceived(string message)
		{
			InputJson json = JsonUtility.FromJson<InputJson>(message);
			InputManager.SetAxis(json.Axis, json.Value);
		}
	}
}
