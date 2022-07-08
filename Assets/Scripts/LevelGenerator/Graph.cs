using System.Collections.Generic;

namespace DarkDungeon
{
    public class Graph 
    {
        #region Fields
        List<Edge> edges;
        #endregion

        #region Properties
        public IReadOnlyCollection<Edge> Edges => edges;
        #endregion

        #region Public Methods
        public Graph()
        {
            edges = new List<Edge>();
        }

        public void AddEdge(Edge edge)
        {
            edges.Add(edge);
        }
        #endregion
    }
}