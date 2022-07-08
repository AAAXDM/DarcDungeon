using System;
using UnityEngine;

namespace DarkDungeon
{
    public class Edge : IComparable<Edge>
    {
        #region Fields
        public readonly float edgeWeight;
        public readonly Vector2 vertexA;
        public readonly Vector2 vertexB;
        #endregion

        #region Public Methods
        public Edge(Vector2 vertexA, Vector2 vertexB)
        {
            this.vertexA = vertexA;
            this.vertexB = vertexB;
            edgeWeight = Math.Abs(vertexA.x - vertexB.x) + Math.Abs(vertexA.y - vertexB.y);
        }

        public bool Equals(Edge edge)
        {
            if (edgeWeight == edge.edgeWeight)
            {
                if (vertexA == edge.vertexA || vertexA == edge.vertexB)
                {
                    if(vertexB == edge.vertexB || vertexB == edge.vertexA)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public int CompareTo(Edge other)
        {
            if (other == null) Debug.LogError("Other edge == null");
            return edgeWeight.CompareTo(other.edgeWeight);
        }
        #endregion
    }
}
