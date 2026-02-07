using System;
using UnityEngine;

namespace ProceduralRooms
{
    [Serializable]
    public class ExitLocation
    {
        public ExitDir Direction;
        public Vector2 Position;
        public Quaternion Rotation;
        public Vector2 WorldPositionOffset;
    }

    public enum ExitDir
    {
        None,
        Up,
        Down,
        Left,
        Right
    }
}