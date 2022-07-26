using System.Collections.Generic;
using System;

namespace DarkDungeon
{
    public class CaveLine : IComparable<CaveLine>
    {
        #region Fields
        List<int> cavePoints;

        int diffenetlyAliveCount;
        int x;
        #endregion

        #region Fields
        public IReadOnlyCollection<int> CavePoints => cavePoints;
        public int DiffenetlyAliveCount => diffenetlyAliveCount;
        public int X => x;
        #endregion

        #region Public Methods
        public CaveLine(Point point)
        {
            cavePoints = new List<int>();
            x = point.x;
            cavePoints.Add(point.y);
            diffenetlyAliveCount = 0;
        }

        public void IncreaseDiffenetlyAliveCount()
        {
            diffenetlyAliveCount++;
        }

        public void AddPoint(int x, int y)
        {
            if (x != this.x) throw new Exception("Cant add point!");
            cavePoints.Add(y);
        }

        public bool TryToMerge(CaveLine caveLine)
        {           
            if (caveLine.X + 1 == x || caveLine.X - 1 == x)
            {
                foreach (int y in caveLine.CavePoints)
                {
                    if (CanMerge(y))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public int CompareTo(CaveLine other)
        {
            if (x > other.x) return 1;
            else if (x < other.x) return -1;
            else return 0;
        }
        #endregion

        #region Methods
        bool CanMerge(int y)
        {
            foreach(int linePoints in cavePoints)
            {
                if (linePoints == y) return true;
            }
            return false;
        }
        #endregion
    }
}