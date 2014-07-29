using System;
using System.Collections.Generic;

namespace MiniGenerator
{
    public enum ArrowType
    {
        Up = BlockIds.Action.Gravity.UP,
        Left = BlockIds.Action.Gravity.LEFT,
        Right = BlockIds.Action.Gravity.RIGHT,
        Down = BlockIds.Action.Gravity.DOWN,
        Zero = BlockIds.Action.Gravity.ZERO        
    }

    public class PhysicsFill
    {
        /// <summary>
        /// Arrow Position Evaluation
        /// </summary>
        public class ArrowPosition 
        {
            /// <summary>
            /// Evaluation score
            /// </summary>
            public Score Score { get; set; }

            /// <summary>
            /// Evaluation position
            /// </summary>
            public Vector Position { get; set; }
        }

        /// <summary>
        /// Arrow Score
        /// </summary>
        public class Score
        {
            /// <summary>
            /// Score value
            /// </summary>
            public int Value { get; set; }

            /// <summary>
            /// Arrow Type
            /// </summary>
            public ArrowType Type { get; set; }

            public Score(ArrowType t, int v)
	        {
                Type = t;
                Value = v;
	        }

            public static Score operator +(Score s, int value)
            {
                return new Score(s.Type, s.Value + value);
            }
        }

        private Path path;
        private Random random;
        private GeneratorSettings settings;

        /// <summary>
        /// Gets adjacent positions relative to a vector
        /// </summary>
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

        /// <summary>
        /// Gets a vector constructed from two adjacent vectors in a path
        /// </summary>
        private Vector GetPathVector(Vector cur, Vector next)
        {
            Vector sub = next.Subtract(cur);
            sub.Normalize();
            return sub;
        }

        /// <summary>
        /// Gets the normalized vector of the mini's path
        /// </summary>
        private Vector GetPathVector()
        {
            Vector vect = new Vector();
            for (int i = 0; i < path.Border.EntryLength; i++)
            {
                vect = vect.Add(path.Border.Entry[i]);
            }

            for (int j = 0; j < path.Border.ExitLength; j++)
            {
                vect = vect.Subtract(path.Border.Exit[j]);
            }

            vect.Normalize();
            return vect;
        }

        /// <summary>
        /// Creates a gravity threshold based on the mini's entry point
        /// </summary>
        private void CreatePhysicsLayout(ArrowType type, int[,] buffer)
        {
            int w = buffer.GetLength(0);
            int h = buffer.GetLength(1);
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (buffer[x, y] == 0)
                    {
                        buffer[x, y] = (int)type;
                    }
                }
            }
        }

        /// <summary>
        /// Average distance from vector to border exit points
        /// </summary>
        private double DistanceToExit(Vector v)
        {
            double dis = 0;
            for (int i = 0; i < path.Border.ExitLength; i++)
            {
                dis += v.DistanceTo(path.Border.Exit[i]);
            }

            return dis / path.Border.ExitLength;
        }

        /// <summary>
        /// Analyzes position of arrow
        /// </summary>
        private ArrowPosition AnalyzePosition(Vector position, Vector pathV, int[,] buffer)
        {
            Score upScore = new Score(ArrowType.Up, 0);
            Score downScore = new Score(ArrowType.Down, 0);
            Score leftScore = new Score(ArrowType.Left, 0);
            Score rightScore = new Score(ArrowType.Right, 0);
            Score zeroScore = new Score(ArrowType.Zero, 0);


            if (buffer[position.X, position.Y] != settings.DefaultBlockId)
            {
                double dis = DistanceToExit(position);
                if (path.PathBuffer[position.X, position.Y] != 0)
                {
                    zeroScore += 2;
                }
                else if (dis < 5)
                {
                    int amount = (int)dis;
                    switch (path.Border.EntryVertex)
                    {
                        case 0:
                            downScore += amount;
                            break;
                        case 1:
                            leftScore += amount;
                            break;
                        case 2:
                            upScore += amount;
                            break;
                        case 3:
                            rightScore += amount;
                            break;
                    }
                }

                List<Vector> adj = Adjacent(position, buffer.GetLength(0), buffer.GetLength(1));
                for (int i = 0; i < adj.Count; i++)
                {
                    if (buffer[adj[i].X, adj[i].Y] == settings.DefaultBlockId ||
                        path.PathBuffer[adj[i].X, adj[i].Y] != 0)
                    {
                        Vector v = adj[i].Subtract(position);
                        v.Normalize();
                        v.Invert();

                        if (v.X < 0)
                        {
                            leftScore += 1;
                        }
                        else if (v.X > 0)
                        {
                            rightScore += 1;
                        }
                    }
                }
            }

            List<Score> sortList = new List<Score>(new Score[] { upScore, downScore, leftScore, rightScore, zeroScore });
            sortList.Sort(new Comparison<Score>(delegate(Score x, Score y) { return -x.Value.CompareTo(y.Value); }));
            return new ArrowPosition() { Score = sortList[0], Position = position };
        }

        /// <summary>
        /// Overlays physics threshold on mini
        /// </summary>
        public void Overlay(int[,] buffer)
        {
            switch (path.Border.EntryVertex)
            {
                case 0:
                    CreatePhysicsLayout(ArrowType.Up, buffer);
                    break;
                case 1:
                    CreatePhysicsLayout(ArrowType.Right, buffer);
                    break;
                case 2:
                    CreatePhysicsLayout(ArrowType.Down, buffer);
                    break;
                case 3:
                    CreatePhysicsLayout(ArrowType.Left, buffer);
                    break;
            }
        }

        /// <summary>
        /// Creates and calculates physics to fill mini
        /// </summary>
        /// <param name="buffer">Mini array buffer</param>
        public void Create(int[,] buffer)
        {
            int w = buffer.GetLength(0);
            int h = buffer.GetLength(1);

            Vector pathV = GetPathVector();
            for (int x = 1; x < w - 1; x++)
            {
                for (int y = 1; y < h - 1; y++)
                {
                    Vector cur = new Vector(x, y);
                    ArrowPosition pos = AnalyzePosition(cur, pathV, buffer);
                    if (pos.Score.Value > 0)
                    {
                        buffer[cur.X, cur.Y] = (int)pos.Score.Type;
                    }
                }
            }

            Overlay(buffer);
        }

        public PhysicsFill(GeneratorSettings settings, Path path, Random random)
        {
            this.settings = settings;
            this.path = path;
            this.random = random;
        }
    }
}
