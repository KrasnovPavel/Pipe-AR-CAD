using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Класс, содержащий enum с направлениями движения метки </summary>
public static class MoveDirections
{
    /// <summary> Направления движения метки</summary>
    public enum Directions
    {
        Forward,
        Backward,
        Left,
        Right,
        Up,
        Down,
        RotationRight,
        RotationLeft
    }
}