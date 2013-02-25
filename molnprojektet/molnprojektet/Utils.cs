using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace molnprojektet
{
    enum Arm { Left, Right };

    // Angle in radians
    struct WindPuffMessage
    {
        private float direction;
        private Arm arm;

        public Arm Arm
        {
            get { return arm; }
            set { arm = value; }
        }

        public float Direction
        {
            get { return direction; }
            set { direction = value; }
        }
    }

    class Utils
    {
        public static double CalculateAngle(Vector3 vector1, Vector3 vector2)
        {
            vector1.Normalize();
            vector2.Normalize();
            
            Vector3 crossProduct = Vector3.Cross(vector1, vector2);

            double crossProductLength = crossProduct.Z;

            double dotProduct = Vector3.Dot(vector1, vector2);

            double angle = Math.Atan2(crossProductLength, dotProduct);

            return angle;
        }

        public static void AddToSpritePosition(Sprite sprite, Vector2 vector)
        {
            Vector2 newPosition = sprite.Position + vector;           
            sprite.Position = newPosition;
        }

        public static void AddToSpritePosition(Sprite sprite, float x, float y)
        {
            Vector2 newPosition = sprite.Position;
            newPosition.X += x;
            newPosition.Y += y;
            sprite.Position = newPosition;
        }

        public static double TicksToSeconds(long ticks)
        {
            return (double)ticks / (double)10000000;
        }
    }
}
