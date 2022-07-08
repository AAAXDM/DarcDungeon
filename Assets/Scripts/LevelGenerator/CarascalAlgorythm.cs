using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DarkDungeon
{
    public class CarascalAlgorythm 
    {
        #region Fields
        List<Edge> edges;
        List<Set> sets;
        #endregion

        #region Properties
        public Edge Edge => edges[0];
        public IReadOnlyCollection<Edge> Edges => edges;
        public IReadOnlyCollection<Set> Sets => sets;
        #endregion

        #region Public Methods
        public CarascalAlgorythm()
        {
            edges = new List<Edge>();
            sets = new List<Set>();
        }

        public void AddGraph(Graph graph)
        {
            if (edges.Count == 0)
            {
                Parallel.ForEach(graph.Edges, edge => edges.Add(edge));
            }
            else
            {
                foreach(Edge edge in graph.Edges)
                {
                    if (!Contains(edge))
                    {
                        edges.Add(edge);
                    }
                }                         
            }
        }
        public void SortEdges()
        {
            edges.Sort();
        }

        public void AddSet(Set set)
        {
            sets.Add(set);
        }

        public void MergeSets(int a, int b)
        {            
            foreach(Vector2 vertex in sets[b].UsingVretexes)
            {
                sets[a].AddUsingVertex(vertex);
            }          
            sets.Remove(sets[b]);            
        }

        public bool CanUseEdge(Edge edge)
        {
            foreach(Set set in sets)
            {
                bool isContainsA = set.Contains(edge.vertexA);
                bool isContainsB = set.Contains(edge.vertexB);
                if (isContainsA && isContainsB)
                {
                    RemoveEdge(edge);
                    return false;
                } 
            }

            return true;
        }

        public List<int> FindUsingSets(Edge edge)
        {
            List<int> usingSetsNumbers = new List<int>();
            int count = sets.Count;

            Parallel.For(0, count, index =>
            {
                bool isContainsA = sets[index].Contains(edge.vertexA);
                bool isContainsB = sets[index].Contains(edge.vertexB);
                if (isContainsA || isContainsB)
                {
                    usingSetsNumbers.Add(index);
                }
            });
            
            return usingSetsNumbers;
        }

        public void RemoveEdge(Edge edge)
        {
            int count = edges.Count;
            Edge edgetoDelete = null;

            Parallel.For(0, count, (i, state) =>
             {
                 if (edge.Equals(edges[i]))
                 {
                     edgetoDelete = edges[i];
                     state.Break();
                 }
             });

            if (edgetoDelete != null)
            {
                edges.Remove(edgetoDelete);
            }
        }

        public void AddUsingVertexInSet(Edge edge, int index)
        {
            sets[index].AddUsingVertexes(edge);
        }
        #endregion

        #region Methods
        bool Contains(Edge newEdge)
        {
            foreach (Edge edge in edges)
            {
                if (newEdge.Equals(edge)) return true;
            }
            return false;
        }
        #endregion
    }
}