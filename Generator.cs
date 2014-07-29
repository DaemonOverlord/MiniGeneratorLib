using MiniGenerator.Obstacles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniGenerator
{
    public class Generator
    {
        private Random random;

        public int Seed { get; set; }

        public bool ValidSize(int size)
        {
            return 0 < size;
        }

        private List<Vector> AdjacentSorted(Vector v, Vector[] goal, int[,] pathBuffer, int width, int height)
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
                    if (pathBuffer[adjUnchecked[i].X, adjUnchecked[i].Y] == 0)
                    {
                        adj.Add(adjUnchecked[i]);
                    }
                }
            }

            VectorSort sorter = new VectorSort(goal);
            adj.Sort(sorter);

            return adj;
        }

        public Border GenerateBorder(int width, int height)
        {
            //Clockwise buffer
            Vector[] perimeter = new Vector[(width * 2) + (height * 2)];

            //Top
            for (int top = 0; top < width; top++)
            {
                perimeter[top] = new Vector(top, 0);
            }

            //Right
            for (int right = 0; right < height; right++)
            {
                perimeter[width + right] = new Vector(width - 1, right);
            }

            //Bottom
            for (int bottom = width - 1; bottom >= 0; bottom--)
            {
                perimeter[width + height + bottom] = new Vector(bottom, height - 1);
            }

            //Left
            for (int left = height - 1; left >= 0; left--)
            {
                perimeter[perimeter.Length - left - 1] = new Vector(0, left);
            }

            int min = Math.Min(width, height);
            int minL = Math.Min(min, 3);
            int maxL = Math.Min(5, min);

            int entryLength, exitLength;
            if (minL == maxL)
            {
                entryLength = minL;
                exitLength = minL;
            }
            else
            {
                entryLength = random.Next(minL, maxL);
                exitLength = random.Next(minL, maxL);
            }

            //Random pick indicies for entry and exit
            //Entry and side cannot be on same side
            List<int> vertices = new List<int>(Enumerable.Range(0, 4));

            int entryRandIndex = random.Next(vertices.Count);
            int entryVertex = vertices[entryRandIndex];
            vertices.RemoveAt(entryRandIndex);

            int exitRandIndex = random.Next(vertices.Count);
            int exitVertex = vertices[exitRandIndex];
            vertices.Clear();

            int entryVertexSIndex = (entryVertex == 0) ? 0 : (((entryVertex / 2) + (entryVertex % 2)) * width) + ((entryVertex / 2) * height);
            int exitVertexSIndex = (exitVertex == 0) ? 0 : (((exitVertex / 2) + (exitVertex % 2)) * width) + ((exitVertex / 2) * height);

            int entrySide = (entryVertex % 2 == 0) ? width : height;
            int randomEntryOff = (entrySide - entryLength > 0) ? random.Next(0, entrySide - entryLength) : 0;
            Vector[] entry = new Vector[entryLength];
            for (int en = 0; en < entry.Length; en++)
            {
                entry[en] = perimeter[entryVertexSIndex + randomEntryOff + en];
            }

            int exitSide = (exitVertex % 2 == 0) ? width : height;
            int randomExitOff = (exitSide - exitLength > 0) ? random.Next(0, exitSide - exitLength) : 0;
            Vector[] exit = new Vector[exitLength];
            for (int ex = 0; ex < exit.Length; ex++)
            {
                exit[ex] = perimeter[exitVertexSIndex + randomExitOff + ex];
            }

            return new Border() { Entry = entry, Exit = exit, EntryVertex = entryVertex, ExitVertex = exitVertex, Perimeter = perimeter };
        }

        public Path GeneratePath(GeneratorSettings settings, Border border, int width, int height)
        {
            /**
             * Create a non-linear path from the entry to the mini's goal
             * Create a non-linear path from the mini's goal to the xit
             */
            int flushness = (settings.Flushness < 1) ? 1 : settings.Flushness;

            //Vector[,] physicsBuffer = new Vector[width, height];
            //for (int x = 0; x < width; x++)
            //{
            //    for (int y = 0; y < height; y++)
            //    {
            //        physicsBuffer[x, y] = new Vector(0, 1);
            //    }
            //}

            int[,] pathBuffer = new int[width, height];

            Vector currentSeed = border.Entry[random.Next(border.EntryLength)];
            pathBuffer[currentSeed.X, currentSeed.Y] = 1;
            Vector[] goal = new Vector[] { Vector.Random(random, 0, width, 0, height) };

            int count = 1;
            bool reachedGoal = false;
            bool reachedExit = false;

            while (!reachedExit)
            {
                List<Vector> adjSorted = AdjacentSorted(currentSeed, goal, pathBuffer, width, height);
                if (adjSorted.Count == 0)
                {
                    break;
                }

                Vector next = adjSorted[random.Next(Math.Min(flushness, adjSorted.Count))];

                count++;
                pathBuffer[next.X, next.Y] = count;
                currentSeed = next;

                for (int i = 0; i < goal.Length; i++)
                {
                    if (goal[i].Equals(next))
                    {
                        if (!reachedGoal)
                        {
                            goal = border.Exit;
                            reachedGoal = true;
                        }
                        else
                        {
                            reachedExit = true;
                        }

                        break;
                    }
                }
            }

            return new Path() { Border = border, Goal = goal[0], PathBuffer = pathBuffer };
        }

        /// <summary>
        /// Generate mini buffer with default settings
        /// </summary>
        /// <param name="width">Width of mini inclusive of border</param>
        /// <param name="height">Height of mini inclusive of border</param>
        public int[,] Generate(int width, int height)
        {
            return Generate(width, height, GeneratorSettings.DEFAULT);
        }

        /// <summary>
        /// Generate mini buffer with custom settings
        /// </summary>
        /// <param name="width">Width of mini</param>
        /// <param name="height">Height of mini</param>
        /// <param name="settings">Generator parameters</param>
        public int[,] Generate(int width, int height, GeneratorSettings settings)
        {
            /**
             *  -Check size
             *  -Create buffer
             *  -Choose obstacles/challenges to be used
             *  -Create a path buffer for the potential paths the player can trace
             *  -Fill blocks and create physics buffer for calculating arrows and such
             *  -Perform test?
             *  -Return result
             */

            //Check size
            //Check width
            if (!ValidSize(width))
            {
                throw new ArgumentOutOfRangeException("width", "Width must be greater than zero!");
            }
            //Check height
            if (!ValidSize(height))
            {
                throw new ArgumentOutOfRangeException("height", "Height must be greater than zero!");
            }

            //Create buffer
            int[,] miniBuffer = new int[width, height];

            //Create entry and exit points
            Border border = GenerateBorder(width, height);
            if (settings.CreateBorder)
            {
                //Create border blocks
                for (int i = 0; i < border.PerimeterLength; i++)
                {
                    miniBuffer[border.Perimeter[i].X, border.Perimeter[i].Y] = settings.DefaultBlockId;
                }

                //Erase entry
                for (int en = 0; en < border.EntryLength; en++)
                {
                    miniBuffer[border.Entry[en].X, border.Entry[en].Y] = BlockIds.Action.Keys.GREEN;
                }

                //Erase exit
                for (int ex = 0; ex < border.ExitLength; ex++)
                {
                    miniBuffer[border.Exit[ex].X, border.Exit[ex].Y] = BlockIds.Action.Keys.RED;
                }
            }

            //Generate path
            Path path = GeneratePath(settings, border, width, height);

            //Generate platforms
            int min = (int)Math.Sqrt(width * height) / 4;
            int platforms = random.Next(min, min * 2);
            for (int i = 0; i < min; i++)
            {
                Platform p = new Platform(settings, path, random);
                int[,] save = (int[,])miniBuffer.Clone();
                p.Implement(miniBuffer);

                Simulator sim = new Simulator(settings, miniBuffer, path);
                if (!sim.Linear())
                {
                    miniBuffer = save;
                }
            }

            //Generate physics
            PhysicsFill fill = new PhysicsFill(settings, path, random);
            fill.Create(miniBuffer);

            //Generate boosts
            int minBoosts = min;
            int c = random.Next(minBoosts, minBoosts + 2);
            for (int i = 0; i < c; i++)
            {
                ArrowGroup p = new ArrowGroup(path, settings, random);
                p.Implement(miniBuffer);
            }

            return miniBuffer;
        }

        public Generator() : this(DateTime.Now.Millisecond)
        {
            
        }

        public Generator(int seed)
        {
            random = new Random(seed);

            Seed = seed;
        }

        /// <summary>
        /// Random enumeration value
        /// </summary>
        public static object RandomEnum(Random random, Type enumType)
        {
            Array ar = Enum.GetValues(enumType);
            int index = random.Next(ar.Length);
            return ar.GetValue(index);
        }
    }
}