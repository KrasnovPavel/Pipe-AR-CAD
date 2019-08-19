// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using JetBrains.Annotations;
using UnityEngine;

#if ENABLE_WINMD_SUPPORT
	using System;
	using System.Collections.Generic;
    using Windows.Storage;
    using Windows.Storage.Pickers;
#endif

/// <summary> Класс, рассчитывающий и отображающий расстояние между соединяемыми трубами. </summary>
[RequireComponent(typeof(LineRenderer))]
 public sealed class Meter : MonoBehaviour
{
	// ReSharper disable once NotNullMemberIsNotInitialized
	/// <summary> Первая метка. </summary>
	[Tooltip("Первая метка.")]
	[NotNull] public Transform First;
	
	// ReSharper disable once NotNullMemberIsNotInitialized
	/// <summary> Вторая метка. </summary>
	[Tooltip("Вторая метка.")]
	[NotNull] public Transform Second;
	
	/// <summary> Расстояние между метками. </summary>
	public float DeltaLength { get; private set; }
	
	/// <summary> Угол между метками. </summary>
	public float DeltaAngle { get; private set; }
	
	public void ChooseFile()
	{
		UnityEngine.WSA.Application.InvokeOnUIThread(PickFile, true);
	}

	public void SaveCurrentData()
	{
#if ENABLE_WINMD_SUPPORT
		FileIO.AppendTextAsync(_file, $"{DeltaLength:0.000}; {DeltaAngle:000.0};"); 
#endif
	}
	
	#region Unity event functions

	/// <summary> Функция, инициализирующая объект в Unity. </summary>
	private void Start()
	{
		_camera = Camera.main.transform;
		_renderer = GetComponent<LineRenderer>();
		_renderer.useWorldSpace = true;
		_label = transform.Find("Label").GetComponent<TextMesh>();
	}

	private void Update()
	{
		Vector3 pos1 = First.position;
		Vector3 pos2 = Second.position;
		
		DeltaLength = (pos1 - pos2).magnitude;
		DeltaAngle = Vector3.Angle(First.forward, Second.forward);
		_renderer.SetPositions(new [] { pos1, pos2 });
		
		_label.transform.LookAt(_camera);
		_label.transform.Rotate(0, 180, 0);
		_label.transform.position = pos1 + (pos2 - pos1) / 2;
		_label.text = $"L: {DeltaLength:0.000}м, Δα: {DeltaAngle:000.0}°";
	}

	#endregion

	#region Private definitions

	private LineRenderer _renderer;
	private TextMesh _label;
	private Transform _camera;
	
#if ENABLE_WINMD_SUPPORT
	private StorageFile _file;
#endif

	private async void PickFile()
	{
#if ENABLE_WINMD_SUPPORT
		FileSavePicker savePicker = new FileSavePicker();
		savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
		savePicker.FileTypeChoices.Add("Meter csv", new List<string>() { ".csv" });
		_file = await savePicker.PickSaveFileAsync();
		await FileIO.WriteTextAsync(_file, "L, м; Δα, °;\n"); 
#endif
	}

	#endregion
}
