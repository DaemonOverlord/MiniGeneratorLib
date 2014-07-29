using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGenerator
{
    public class Path
    {
        /// <summary>
        /// Path interactive goal
        /// </summary>
        public Vector Goal { get; set; }

        /// <summary>
        /// Path border
        /// </summary>
        public Border Border { get; set; }

        /// <summary>
        /// Path array buffer that stores the path
        /// </summary>
        public int[,] PathBuffer { get; set; }
    }
}
