namespace MiniGenerator
{
    public class GeneratorSettings
    {
        /// <summary>
        /// Not implemented yet
        /// </summary>
        public float Difficulty { get; set; }

        /// <summary>
        /// Straightness of generated mini path
        /// </summary>
        public int Flushness { get; set; }

        /// <summary>
        /// Block used to generate mini border
        /// </summary>
        public int DefaultBlockId { get; set; }

        /// <summary>
        /// Should generator create a block border
        /// </summary>
        public bool CreateBorder { get; set; }

        public GeneratorSettings()
        {
            Difficulty = 0.5f;
            Flushness = 2;
            DefaultBlockId = BlockIds.Blocks.Basic.CYAN;
            CreateBorder = true;
        }

        /// <summary>
        /// Default generator settings
        /// </summary>
        public static GeneratorSettings DEFAULT = new GeneratorSettings();
    }
}