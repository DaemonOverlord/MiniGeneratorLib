namespace MiniGenerator
{
    public interface IObstacle
    {
        /// <summary>
        /// Implements an obstacle directly into the mini
        /// </summary>
        /// <param name="buffer">Mini array buffer</param>
        void Implement(int[,] buffer);
    }
}