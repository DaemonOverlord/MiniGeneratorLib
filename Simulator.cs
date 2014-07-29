using System.Collections.Generic;

namespace MiniGenerator
{
    /// <summary>
    /// Mini theoretical tester
    /// </summary>
    public class Simulator
    {
        private int[,] mini;
        private Path path;
        private GeneratorSettings settings;
        private List<Vector> jumppoints;

        /// <summary>
        /// Gets adjacent positions of a vector
        /// </summary>
        private List<Vector> Adjacent(Vector v, List<Vector> list, int width, int height)
        {
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

        /// <summary>
        /// Is the mini solvable without blockage
        /// </summary>
        public bool Linear()
        {
            List<Vector> list = new List<Vector>();
            List<Vector> seeds = new List<Vector>();
            seeds.AddRange(path.Border.Entry);
            list.AddRange(path.Border.Entry);

            while (seeds.Count > 0)
            {
                Vector current = seeds[0];
                for (int j = 0; j < path.Border.ExitLength; j++)
                {
                    if (current.Equals(path.Border.Exit[j]))
                    {
                        return true;
                    }
                }

                seeds.RemoveAt(0);

                List<Vector> adj = Adjacent(current, list, mini.GetLength(0), mini.GetLength(1));
                seeds.AddRange(adj);
                list.AddRange(adj);
            }

            return false;
        }

        /// <summary>
        /// Not implemented, theoetical prediction of possibility
        /// </summary>
        public float Possibility()
        {
            if (!Linear())
            {
                return 0f;
            }
            return 1f;
        }

        public Simulator(GeneratorSettings settings, int[,] mini, Path path)
        {
            this.settings = settings;
            this.mini = mini;
            this.path = path;
            jumppoints = new List<Vector>();
        }
    }
}