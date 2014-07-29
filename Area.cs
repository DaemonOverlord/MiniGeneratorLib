namespace MiniGenerator
{
    public class Area
    {
        public Vector UpperLeft;
        public Vector LowerRight;

        public Area(Vector v1, Vector v2)
        {
            UpperLeft = new Vector();
            LowerRight = new Vector();

            if (v1.X < v2.X)
            {
                UpperLeft.X = v1.X;
                LowerRight.X = v2.X;
            }
            else
            {
                UpperLeft.X = v2.X;
                LowerRight.X = v1.X;
            }

            if (v1.Y < v2.Y)
            {
                UpperLeft.Y = v1.Y;
                LowerRight.Y = v2.Y;
            }
            else
            {
                UpperLeft.Y = v2.Y;
                UpperLeft.Y = v1.Y;
            }
        }
    }
}