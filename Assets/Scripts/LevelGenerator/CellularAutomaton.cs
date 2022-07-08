using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace DarkDungeon
{
    public class CellularAutomaton
    {
        #region Fields
        CellState[][] stateField;
        List<Cave> caves; 
        int selector = 4;
        int aliveCount = 5;
        #endregion

        #region Properties
        public IReadOnlyCollection<IReadOnlyCollection<CellState>> StateField => stateField;
        #endregion

        #region Public Methods
        public CellularAutomaton(int width, int height)
        {
            stateField = new CellState[height][];
            Parallel.For(0, height, index => stateField[index] = new CellState[width]);
        }

        public void FillStateField(Leaf leaf)
        {
            int height = stateField.Length;
            int width = stateField[0].Length;

            for(int i = 0; i < width; i ++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (i + leaf.x >= leaf.Room.x && i + leaf.x < leaf.Room.x + leaf.Room.width)
                    {
                        if (j + leaf.y >= leaf.Room.y && j + leaf.y < leaf.Room.y + leaf.Room.height)
                        {
                            stateField[j][i] = CellState.definitelyAlive;
                            continue;
                        }
                    }

                    TryToEqualWithPassage(leaf, i, j, out bool isEqual);

                    if (isEqual) continue;

                    if (i == 0 || i == width - 1) stateField[j][i] = CellState.definitelyDead;

                    else if (j == 0 || j == height - 1) stateField[j][i] = CellState.definitelyDead;

                    else SetCellState(i, j);
                }
            }
        }

        public void RunCellularAutomation()
        {
            NextGeneration();
        }

        public void SetRoomSize(UserRect rect)
        {
            aliveCount = rect.width * rect.height;
        }

        public void DeleteIslands()
        {
            List<CaveLine> caveLines = CalculateCaveLines();
            caves = new List<Cave>();
            Cave startCave = new Cave();
            startCave.AddCaveLine(caveLines[0]);
            caves.Add(startCave);

            CalculateCaves(caveLines);
            TryToMergeCaves();
            DeleteCellsInCaves();
        }

        public List<Point> GetBorderPositions(Leaf leaf, out List<Point> border)
        {
            List<Point> walls = new List<Point>();
            border = new List<Point>();
            int height = stateField.Length;
            int width = stateField[0].Length;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (stateField[j][i] == CellState.dead || stateField[j][i] == CellState.definitelyDead)
                    {
                        if (j != 0)
                        {
                            if (stateField[j - 1][i] == CellState.definitelyAlive || stateField[j - 1][i] == CellState.alive)
                            {
                                walls.Add(new Point(i + leaf.x, j + leaf.y));
                                continue;
                            }
                        }
                        if(NeedToAddBorder(i,j))
                        {
                            border.Add(new Point(i + leaf.x, j + leaf.y));
                        }
                    }    
                }
            }
            return walls;
        }
        #endregion

        #region Methods
        void CalculateCaves(List<CaveLine> caveLines)
        {
            int count = caveLines.Count;

            for (int j = 1; j < count; j++)
            {
                bool isAdded = false;
                foreach (Cave cave in caves)
                {
                    cave.TryToAddCaveLine(caveLines[j], out isAdded);
                    if (isAdded) break;
                }
                if (!isAdded)
                {
                    Cave cave = new Cave();
                    cave.AddCaveLine(caveLines[j]);
                    caves.Add(cave);
                }
            }

        }

        void TryToMergeCaves()
        {
            for (int i = 0; i < caves.Count - 1; i++)
            {
                for (int j = 0; j < caves.Count; j++)
                {
                    if (i == j) continue;
                    if (caves[i].CanMergeCaves(caves[j]))
                    {
                        caves[i].MergeCaves(caves[j]);
                        caves.Remove(caves[j]);
                    }
                }
            }
        }

        void DeleteCellsInCaves()
        {
            Parallel.ForEach(caves, cave =>
            {
                if (cave.DiffenetlyAliveCount < aliveCount)
                    DeleteCaveCells(cave);
            });
        }

        void TryToEqualWithPassage(Leaf leaf, int i, int j, out bool isEqual)
        {
            int passagePoint = 0;
            Parallel.ForEach(leaf.Passages, passage =>
            {
                if (i + leaf.x >= passage.x && i + leaf.x < passage.x + passage.width)
                {
                    if (j + leaf.y >= passage.y && j + leaf.y < passage.y + passage.height)
                    {
                        stateField[j][i] = CellState.definitelyAlive;
                        passagePoint++;
                    }
                }
            });
            isEqual = passagePoint > 0 ? true : false;
        }

        void SetCellState(int x, int y)
        {
            int randomInt = UnityEngine.Random.Range(0, 8);
            if (Enum.IsDefined(typeof(CellState), randomInt))
            {
                CellState cellState = (CellState)randomInt;
                if(cellState == CellState.definitelyDead)
                {
                    cellState = CellState.dead;
                }
                stateField[y][x] = cellState;
            }
            else
            {
                bool random = UserRandom.RandomBool();
                stateField[y][x] = random == true ? CellState.alive : CellState.dead;
            }
        }

        void NextGeneration()
        {
            int height = stateField.Length;
            int width = stateField[0].Length;
            CellState[][] cellStates = new CellState[height][];

            Parallel.For(0, height, index => cellStates[index] = new CellState[width]);

            Parallel.For(0, width, i =>
            {
               for (int j = 0; j < height; j++)
               {
                   CellState cellState = stateField[j][i];
                   if (cellState == CellState.definitelyAlive || cellState == CellState.definitelyDead)
                   {
                       cellStates[j][i] = stateField[j][i];
                       continue;
                   }
                   int neighboursCount = CountAllNeighbours(i, j, out int deadCells);
                   if (cellState == CellState.dead && neighboursCount > selector)
                   {
                       cellStates[j][i] = CellState.alive;
                   }
                   else if (cellState == CellState.alive && deadCells > selector)
                   {
                       cellStates[j][i] = CellState.dead;
                   }
                   else
                   {
                       cellStates[j][i] = stateField[j][i];
                   }
               }
            });
            stateField = cellStates;
        }

        int CountAllNeighbours(int x, int y, out int dead)
        {
            int count = 0;           
            int colStart = 0;
            int rowStart = 0;
            int colEnd = 1;
            int rowEnd = 1;
            int deadCells = 0;

            if (x != 0) colStart = -1;
            if (y != 0) rowStart = -1;
            if (x != stateField[0].Length -1) colEnd = 2;
            if (y != stateField.Length -1) rowEnd = 2;

            for(int i = colStart; i < colEnd; i++)
            {
                Parallel.For(rowStart, rowEnd, j =>
                {
                    int col = x + i;
                    int row = y + j;
                    bool isSelfChecking = col == x && row == y;

                    CellState cellState = stateField[row][col];

                    if ((cellState == CellState.alive || cellState == CellState.definitelyAlive) && !isSelfChecking)
                    {
                        count++;
                    }
                    if ((cellState == CellState.definitelyDead || cellState == CellState.dead) && !isSelfChecking)
                    {
                        deadCells++;
                    }
                });
            }
            dead = deadCells;
            return count;
        }       

        void DeleteCaveCells(Cave cave)
        {
            Parallel.ForEach(cave.CaveLines, caveLine =>
            {
                foreach (int y in caveLine.CavePoints)
                {
                    stateField[y][caveLine.X] = CellState.dead;
                }
            });       
        }

        List<CaveLine> CalculateCaveLines()
        {
            List<CaveLine> caveLines = new List<CaveLine>();
            int height = stateField.Length;
            int width = stateField[0].Length;
            int counter = 0;

            for(int i =0; i < width; i ++)
            {
               counter++;
               for (int j = 0; j < height; j++)
               {
                   if (stateField[j][i] == CellState.dead || stateField[j][i] == CellState.definitelyDead)
                   {
                       counter++;
                       continue;
                   }
                   if (caveLines.Count == 0 || counter > 0)
                   {
                       CaveLine caveLine = CreateNewCaveLine(i, j);
                       caveLines.Add(caveLine);
                       counter = 0;
                   }
                   else
                   {
                       if (stateField[j][i] == CellState.definitelyAlive)
                       {
                           caveLines[caveLines.Count - 1].IncreaseDiffenetlyAliveCount();
                       }
                       caveLines[caveLines.Count - 1].AddPoint(i, j);
                   }
               }
             }

            return caveLines;
        }

        CaveLine CreateNewCaveLine(int x, int y)
        {
            CaveLine caveLine = new CaveLine(new Point(x,y));

            if (stateField[y][x] == CellState.definitelyAlive)
            {
                caveLine.IncreaseDiffenetlyAliveCount();
            }

            return caveLine;
        }

        bool NeedToAddBorder(int x, int y)
        {
            if (y == stateField.Length - 1) return false;
            if (x != 0)
            {
                if (stateField[y][x - 1] == CellState.definitelyAlive || stateField[y][x - 1] == CellState.alive) return true;
            }
            if (x != stateField[0].Length - 1)
            {
                if (stateField[y][x + 1] == CellState.definitelyAlive || stateField[y][x + 1] == CellState.alive) return true;
            }
            if (stateField[y + 1][x] == CellState.definitelyAlive || stateField[y + 1][x] == CellState.alive) return true;
            return false;
        }
        #endregion
    }
}