using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGenerator.Obstacles
{
    public enum PlatformUse
    {
        BlockPath,
        AccessPath
    }

    public enum Placement
    {
        OnWall,
        InAir
    }

    /// <summary>
    /// Linear block platform obstacle
    /// </summary>
    public class Platform : IObstacle
    {
        /// <summary>
        /// Platform's position
        /// </summary>
        public class PlatformPosition
        {
            /// <summary>
            /// Score platform received in position
            /// </summary>
            public int Score { get; set; }

            /// <summary>
            /// Postion vector
            /// </summary>
            public Vector Position { get; set; }

            /// <summary>
            /// Vector direction
            /// </summary>
            public Vector Direction { get; set; }
        }

        private Path path;
        private Random random;
        private GeneratorSettings settings;

        /// <summary>
        /// Describes use of platform
        /// </summary>
        public PlatformUse Use { get; set; }

        /// <summary>
        /// Describes platform placement
        /// </summary>
        public Placement Placement { get; set; }

        public PlatformPosition Position;

        private bool Valid(Vector vector, int width, int height)
        {
            return vector.X >= 0 && vector.X < width &&
                   vector.Y >= 0 && vector.Y < height;
        }

        private List<Vector> Adjacent(Vector v, int width, int height)
        {
            List<Vector> adj = new List<Vector>();

            Vector[] adjUnchecked = new Vector[8] 
            {
                v.Add(-1, -1),
                v.Add(0, -1),
                v.Add(1, -1),
                v.Add(1, 0),
                v.Add(1, 1),
                v.Add(0, 1),
                v.Add(-1, 1),
                v.Add(-1, 0)
            };

            for (int i = 0; i < adjUnchecked.Length; i++)
            {
                if (adjUnchecked[i].X >= 0 && adjUnchecked[i].X < width &&
                    adjUnchecked[i].Y >= 0 && adjUnchecked[i].Y < height)
                {
                    adj.Add(adjUnchecked[i]);
                }
            }

            return adj;
        }

        private PlatformPosition AnalyzePosition(Path path, int[,] buffer, Vector position)
        {
            //if (buffer[position.X, position.Y] != 0)
            //{
            //    return null;
            //}

            int score = 0;
            if (position.DistanceTo(path.Goal) < 5)
            {
                score++;
            }

            if (path.PathBuffer[position.X, position.Y] != 0)
            {
                if (Use == PlatformUse.AccessPath)
                {
                    score += 3;
                }
                else
                {
                    score -= 3;
                }
            }

            int c = 0;
            Vector total = new Vector();
            List<Vector> adj = Adjacent(position, buffer.GetLength(0), buffer.GetLength(1));
            for (int i = 0; i < adj.Count; i++)
            {
                if (path.PathBuffer[adj[i].X, adj[i].Y] != 0)
                {
                    Vector dif = position.Subtract(adj[i]);
                    total.X += dif.X;
                    total.Y += dif.Y;
                    c++;

                    score++;
                }
            }

            total.Normalize();
            return new PlatformPosition() { Position = position, Score = score, Direction = total };
        }

        private List<Vector> CreateField(int width, int height)
        {
            List<Vector> field = new List<Vector>();
            for (int i = 0; i < width * height; i++)
            {
                int x = i % width;
                int y = i / width;

                Vector v = new Vector(x, y);
                if (x != 0 && x != width - 1 && y != 0 && y != height - 1)
                {
                    field.Add(v);
                }
            }

            return field;
        }

        public void Implement(int[,] buffer)
        {
            int w = buffer.GetLength(0);
            int h = buffer.GetLength(1);

            List<Vector> potential = new List<Vector>();
            if (Placement == Placement.OnWall)
            {
                potential.AddRange(path.Border.Perimeter);
            }
            else
            {
                potential.AddRange(CreateField(w, h));
            }

            int bestScore = 0;
            List<PlatformPosition> best = new List<PlatformPosition>();
            for (int i = 0; i < potential.Count; i++)
            {
                Vector vector = potential[i];
                if (path.Border.InterferesWith(vector))
                {
                    break;
                }

                PlatformPosition cur = AnalyzePosition(path, buffer, vector);
                if (cur != null)
                {
                    if (cur.Score > bestScore)
                    {
                        bestScore = cur.Score;
                        best.Clear();
                        best.Add(cur);
                    }
                    else if (cur.Score == bestScore)
                    {
                        best.Add(cur);
                    }
                }
            }

            if (best.Count == 0)
            {
                return;
            }

            PlatformPosition pos = best[random.Next(best.Count)];
            if (pos.Direction.IsEmpty())
            {
                pos.Direction = Vector.Random(random, -1, 2, -1, 2); 
            }

            Vector v = pos.Position;

            int length = random.Next(3, 6);
            for (int i = 0; i < length; i++)
            {
                if (!Valid(v, w, h))
                {
                    continue;
                }

                buffer[v.X, v.Y] = settings.DefaultBlockId;
                v = v.Add(pos.Direction);
            }

            this.Position = pos;
        }

        public Platform(GeneratorSettings settings, Path path, Random random)
        {
            this.settings = settings;
            this.path = path;
            this.random = random;

            this.Use = (PlatformUse)Generator.RandomEnum(random, typeof(PlatformUse));

            if (!settings.CreateBorder)
            {
                this.Placement = (Placement)Generator.RandomEnum(random, typeof(Placement));
            }
            else
            {
                this.Placement = Placement.InAir;
            }
        }

    }
}