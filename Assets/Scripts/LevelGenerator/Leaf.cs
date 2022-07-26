using UnityEngine;
using System.Collections.Generic;

namespace DarkDungeon
{
    public class Leaf 
    {
        #region Fields
        const int minLeafSize = 15;

        UserRect room;
        List<UserRect> passages;
        Leaf leftChild;
        Leaf rightChild;
        Point[] allPoints;
        CellularAutomaton cellularAutomaton;

        int minRoomSize = 8;
        int maxRoomSize = 13;
        bool isFinalLeaf;

        public readonly Vector2 center;
        public readonly int x;
        public readonly int y;
        public readonly int width;
        public readonly int height;
        #endregion

        #region Properties
        public bool IsLastLeaf => isFinalLeaf;
        public Leaf LeftChild => leftChild;
        public Leaf RightChild => rightChild;
        public UserRect Room => room;
        public IReadOnlyCollection<UserRect> Passages => passages;
        public IReadOnlyCollection<Point> AllPoints => allPoints;
        public CellularAutomaton CellularAutomaton => cellularAutomaton;

        public int EndX => x + width - 1;
        public int EndY => y + height - 1;
        #endregion

        #region Public Methods
        public Leaf(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            center.x = x + (float)width / 2;
            center.y = y + (float)height / 2;
            passages = new List<UserRect>();

            CalculateAllPoints();
        }

        public bool Split()
        {
            bool hSplit = UserRandom.RandomBool();
            int maxSplitSize;
            int split;

            if (leftChild != null && rightChild != null) 
            {
                isFinalLeaf = false;
                return false;
            }

            if (width > height && (float)(width / height) >= 1.25)
            {
                hSplit = false;
            }

            else if (height > width && (float)(height / width) >= 1.25)
            {
                hSplit = true;
            }

            maxSplitSize = (hSplit ? height : width) - minLeafSize;

            if (maxSplitSize <= minLeafSize)
            {
                isFinalLeaf = true;
                return false;
            }

            split = Random.Range(minLeafSize, maxSplitSize + 1);

            if (hSplit)
            {
                leftChild = new Leaf(x, y,width,split);
                rightChild = new Leaf(x,y + split,width,height - split);
            }
            else
            {
                leftChild = new Leaf(x, y, split, height);
                rightChild = new Leaf(x + split, y, width - split, height);
            }

            return  true;
        }

        public void CreateRoom()
        {
            int indent = 3;
            int maxRoomWidth = width - indent < maxRoomSize ? width - indent : maxRoomSize;
            int maxRoomHeight = height - indent < maxRoomSize ? height - indent : maxRoomSize;
            Point roomSize = new Point(Random.Range(minRoomSize, maxRoomWidth), Random.Range(minRoomSize, maxRoomHeight));
            Point roomPosition = new Point(Random.Range(indent, width - roomSize.x), Random.Range(indent, height - roomSize.y));
            UserRect rect = new UserRect(x + roomPosition.x, y + roomPosition.y, roomSize.x, roomSize.y);
            room = rect;
        }

        public void AddPassage(UserRect passage)
        {
            passages.Add(passage);
        }

        public void GenerateCellularAutomation()
        {
            cellularAutomaton = new CellularAutomaton(width, height);
            cellularAutomaton.SetRoomSize(room);
        }

        public void FillCellularAutomation()
        {
            cellularAutomaton.FillStateField(this);
        }
        #endregion

        #region Methods
        void CalculateAllPoints()
        {
            allPoints = new Point[4];
            allPoints[0] = new Point(x, y);
            allPoints[1] = new Point(x, y + height);
            allPoints[2] = new Point(x + width, y);
            allPoints[3] = new Point(x + width, y + height);
        }
        #endregion
    }

}