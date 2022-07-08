using System.Collections.Generic;
using System.Threading.Tasks;

namespace DarkDungeon
{
    public class Cave
    {
        #region Fields
        List<CaveLine> caveLines;
        int diffenetlyAliveCount;
        int maxX;
        #endregion

        #region Properties
        public IReadOnlyCollection<CaveLine> CaveLines => caveLines;
        public int DiffenetlyAliveCount => diffenetlyAliveCount;
        public int MaxX => maxX;
        public CaveLine LastCaveLine => caveLines[caveLines.Count - 1];
        #endregion

        #region Public Methods
        public Cave()
        {
            caveLines = new List<CaveLine>();
            diffenetlyAliveCount = 0;
        }

        public void SortCaveLines()
        {
            caveLines.Sort();
            maxX = caveLines[caveLines.Count - 1].X;
        }

        public void AddCaveLine(CaveLine caveLine)
        {
            caveLines.Add(caveLine);
            diffenetlyAliveCount += caveLine.DiffenetlyAliveCount;
            maxX = caveLine.X;
        }

        public bool CanMergeCaves(Cave cave)
        {
            CaveLine caveLine = caveLines.Find(x => x.X == cave.MaxX + 1);
            if(caveLine != null)
            {
                if (caveLine.TryToMerge(cave.LastCaveLine))
                {
                    return true;
                }
            }            
            return false;
        }

        public void MergeCaves(Cave cave)
        {
            diffenetlyAliveCount += cave.DiffenetlyAliveCount;
            Parallel.ForEach(cave.CaveLines, caveLine => caveLines.Add(caveLine));           
            SortCaveLines();
        }

        public void TryToAddCaveLine(CaveLine caveLine, out bool isAdded)
        {
           isAdded = false;

           foreach (CaveLine thisCaveLine in caveLines)
           {
                if (thisCaveLine.TryToMerge(caveLine))
                {
                    AddCaveLine(caveLine);
                    isAdded = true;
                    SortCaveLines();
                    break;
                }               
           }
        }
        #endregion
    }
}