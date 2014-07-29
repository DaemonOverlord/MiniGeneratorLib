using System;
using System.Collections.Generic;

namespace MiniGenerator.Obstacles
{
    /// <summary>
    /// Arrow fill object
    /// </summary>
    public class ArrowGroup : IObstacle
    {
        private Path path;
        private GeneratorSettings settings;
        private Random random;

        private int size;
        private ArrowType type;

        private List<Vector> Adjacent(Vector v, List<Vector> list, int[,] mini)
        {
            int width = mini.GetLength(0);
            int height = mini.GetLength(1);
            List<Vector> adj = new List<Vector>();

            Vector[] adjUnchecked = new Vector[4] 
            {
                v.Add(0, -1),
                v.Add(1, 0),
                v.Add(0, 1),
                v.Add(-1, 0)
            };

            for (int i = 0; i < adjUnchecked.Length; i++)
            {
                if (adjUnchecked[i].X >= 0 && adjUnchecked[i].X < width &&
                    adjUnchecked[i].Y >= 0 && adjUnchecked[i].Y < height)
                {
                    if (mini[adjUnchecked[i].X, adjUnchecked[i].Y] != settings.DefaultBlockId)
                    {
                        bool found = false;
                        for (int j = 0; j < list.Count; j++)
                        {
                            if (list[j].Equals(adjUnchecked[i]))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            adj.Add(adjUnchecked[i]);
                        }
                    }
                }
            }

            return adj;
        }

        public void Implement(int[,] buffer)
        {
            List<Vector> total = new List<Vector>();
            Vector cur = Vector.Random(random, 1, buffer.GetLength(0) - 1, 1, buffer.GetLength(1) - 1);

            int count = 0;
            while (count < size)
            {
                buffer[cur.X, cur.Y] = (int)type;
                total.Add(cur);
                List<Vector> adj = Adjacent(cur, total, buffer);
                if (adj.Count == 0)
                {
                    break;
                }

                cur = adj[random.Next(adj.Count)];
                count++;
            }
        }

        public ArrowGroup(Path path, GeneratorSettings settings, Random random)
        {
            this.path = path;
            this.settings = settings;
            this.random = random;

            this.size = random.Next(5, 20);
            switch (path.Border.EntryVertex)
            {
                case 0:
                    type = ArrowType.Down;
                    break;
                case 1:
                    type = ArrowType.Left;
                    break;
                case 2:
                    type = ArrowType.Up;
                    break;
                case 3:
                    type = ArrowType.Right;
                    break;
            }
            
        }
    }
}
