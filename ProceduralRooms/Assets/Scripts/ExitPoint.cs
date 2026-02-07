using UnityEngine;

namespace ProceduralRooms
{
    public class ExitPoint
    {
        public Vector2 Position;
        public ExitDir Direction;

        [HideInInspector]
        public Exit LinkedExit;

        public ExitPoint(Vector2 position, ExitDir direction)
        {
            Position = position;
            switch (direction)
            {
                case ExitDir.Up:
                    Direction = ExitDir.Down;
                    break;
                case ExitDir.Down:
                    Direction = ExitDir.Up;
                    break;
                case ExitDir.Left:
                    Direction = ExitDir.Right;
                    break;
                case ExitDir.Right:
                    Direction = ExitDir.Left;
                    break;
            }
        }
    }
}