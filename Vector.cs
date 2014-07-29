using System;

namespace MiniGenerator
{
    public class Vector
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vector Add(int x, int y)
        {
            return new Vector(X + x, Y + y);
        }

        public Vector Add(Vector v)
        {
            return new Vector(v.X + X, v.Y + Y);
        }

        public Vector Subtract(Vector v)
        {
            return new Vector(X - v.X, Y -  v.Y);
        }

        public double DistanceTo(Vector v)
        {
            return Math.Sqrt(((X - v.X) * (X - v.X)) + ((Y - v.Y) * (Y - v.Y)));
        }

        public void Normalize()
        {
            if (X < 0)
            {
                X = -1;
            }
            else if (X > 0)
            {
                X = 1;
            }
            else
            {
                X = 0;
            }

            if (Y < 0)
            {
                Y = -1;
            }
            else if (Y > 0)
            {
                Y = 1;
            }
            else
            {
                Y = 0;
            }
        }

        public void Invert()
        {
            X = -X;
            Y = -Y;
        }

        public bool IsEmpty()
        {
            return X == 0 && Y == 0;
        }

        public bool Equals(Vector v)
        {
            return v.X == X && v.Y == Y;
        }

        public override string ToString()
        {

            return X + ", " + Y;
        }

        public Vector()
        {
            X = 0;
            Y = 0;
        }

        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Vector Random(Random random, int minX, int maxX, int minY, int maxY)
        {
            return new Vector(random.Next(minX, maxX), random.Next(minY, maxY));
        }
    }
}