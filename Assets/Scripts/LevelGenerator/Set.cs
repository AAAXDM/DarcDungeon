using System.Collections.Generic;
using UnityEngine;

namespace DarkDungeon
{
    public class Set
    {
        #region Fields
        List<Vector2> usingVertexes;
        #endregion

        #region Properties
        public IReadOnlyCollection<Vector2> UsingVretexes => usingVertexes;
        #endregion

        #region Public Methods
        public Set()
        {
            usingVertexes = new List<Vector2>();
        }

        public void AddUsingVertexes(Edge edge)
        {
            AddUsingVertex(edge.vertexA);

            AddUsingVertex(edge.vertexB);
        }

        public void AddUsingVertex(Vector2 vertex)
        {
            if (!usingVertexes.Contains(vertex))
            {
                usingVertexes.Add(vertex);
            }
        }

        public bool Contains(Vector2 vertex)
        {
            return usingVertexes.Contains(vertex);
        }
        #endregion
    }
}