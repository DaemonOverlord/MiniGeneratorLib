namespace MiniGenerator
{
    public class Border
    {
        public int PerimeterLength { get { return (Perimeter == null) ? 0 : Perimeter.Length; } }
        public int EntryLength { get { return (Entry == null) ? 0 : Entry.Length; } }
        public int ExitLength { get { return (Exit == null) ? 0 : Exit.Length; } }

        public int EntryVertex { get; set; }
        public int ExitVertex { get; set; }

        public Vector[] Perimeter { get; set; }
        public Vector[] Entry { get; set; }
        public Vector[] Exit { get; set; }

        public bool InterferesWith(Vector v)
        {
            for (int i = 0; i < EntryLength; i++)
            {
                if (v.Equals(Entry[i]))
                {
                    return true;
                }
            }

            for (int j = 0; j < ExitLength; j++)
            {
                if (v.Equals(Exit[j]))
                {
                    return true;
                }
            }

            return false;
        }
    }
}