using System;
using System.Collections.Generic;

namespace MiniGenerator
{
    /// <summary>
    /// Sorts vectors based on distance to goal
    /// </summary>
    public class VectorSort : IComparer<Vector>
    {
        private Vector[] goal;

        public int Compare(Vector x, Vector y)
        {
            //-1 is closer

            double closestX = double.MaxValue;
            double closestY = double.MaxValue;
            for (int i = 0; i < goal.Length; i++)
            {
                closestX = Math.Min(x.DistanceTo(goal[i]), closestX);
                closestY = Math.Min(y.DistanceTo(goal[i]), closestY);
            }

            return closestX.CompareTo(closestY);
        }

        public VectorSort(Vector[] goal)
        {
            this.goal = goal;
        }
    }
}