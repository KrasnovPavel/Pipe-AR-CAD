// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using Vuforia;

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

	public DefaultTrackableEventHandler t1;
	public DefaultTrackableEventHandler t2;
	
	/// <summary> Расстояние между метками. </summary>
	public float DeltaLength { get; private set; }
	
	/// <summary> Угол между метками. </summary>
	public float DeltaAngle { get; private set; }

	/// <summary> Расстояние до меток. </summary>
	public float[] Distances = new float[2];

	/// <summary> Угол между меткой и камерой. </summary>
	public float[] Angles = new float[2];

	/// <summary> Данные о том распознанна каждая метка или нет. </summary>
	public bool[] MarksDetected = new bool[2];
	
	public void ChooseFile()
	{
		UnityEngine.WSA.Application.InvokeOnUIThread(PickFile, true);
	}

	public void SaveCurrentData()
	{
#if ENABLE_WINMD_SUPPORT
		FileIO.AppendTextAsync(_file, $"L: {DeltaLength:0.000}м; Δα: {DeltaAngle:000.0}°;" +
		                              $"D1: {Distances[0]:0.000}м; D2: {Distances[1]:0.000}м;" +
		                              $"β1: {Angles[0]:000.0}°; β2: {Angles[1]:000.0}°;" +
		                              $"1: {MarksDetected[0]}; 2: {MarksDetected[1]};\n"); 
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
		Distances = new []
		{
			(_camera.position - pos1).magnitude,
			(_camera.position - pos2).magnitude,
		};

		Angles = new[]
		{
			Vector3.Angle(_camera.forward, First.forward),
			Vector3.Angle(_camera.forward, Second.forward)
		};

		MarksDetected = new[]
		{
			t1.TargetVisible,
			t2.TargetVisible
		};
		
		_renderer.SetPositions(new [] { pos1, pos2 });
		
		_label.transform.LookAt(_camera);
		_label.transform.Rotate(0, 180, 0);
		_label.transform.position = pos1 + (pos2 - pos1) / 2;
		_label.text = $"L: {DeltaLength:0.000}м; Δα: {DeltaAngle:000.0}°;\n" +
		              $"D1: {Distances[0]:0.000}м; D2: {Distances[1]:0.000}м;\n" +
		              $"β1: {Angles[0]:000.0}°; β2: {Angles[1]:000.0}°;\n" +
		              $"1: {(MarksDetected[0] ? "Отслеживается" : "Не отслеживается")}; " +
		              $"2: {(MarksDetected[1] ? "Отслеживается" : "Не отслеживается")}; ";
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
		await FileIO.WriteTextAsync(_file, "L, м; Δα, °; D1, м; D2, м; FirstVisible; SecondVisible; β1, °; β2, °;\n"); 
#endif
	}

	#endregion
}
