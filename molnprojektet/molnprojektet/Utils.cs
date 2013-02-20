using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace molnprojektet
{
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

        public static void addToSpritePosition(Sprite sprite, Vector2 vector)
        {
            Vector2 newPosition = sprite.Position;
            newPosition.X += vector.X;
            newPosition.Y += vector.X;
            sprite.Position = newPosition;
        }

        public static void addToSpritePosition(Sprite sprite, float x, float y)
        {
            Vector2 newPosition = sprite.Position;
            newPosition.X += x;
            newPosition.Y += y;
            sprite.Position = newPosition;
        }
    }
}
