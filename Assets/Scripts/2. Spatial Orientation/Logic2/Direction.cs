using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Directions {
    public enum Direction 
    {
        North = 0,
        East = 90,
        South = 180,
        West = 270,
        NorthEast = 45,
        SouthEast = 135,
        SouthWest = 225,
        NorthWest = 315,
        Null = 360
    }

    public static class DirectionExt {
        public static Direction RotateDirection(this Direction direction, int angle) 
        {
            if (angle < 0) 
            {
                angle += 360;
            }

            int newAngle = (int) direction;
            newAngle = (newAngle + angle) % 360;
            return (Direction) newAngle;
        }

        public static int GetAngle(this Direction direction) 
        {
            return (int) direction;
        }

        public static Direction GetReverse(this Direction direction) 
        {
            return direction.RotateDirection(180);
        }
    }

}
