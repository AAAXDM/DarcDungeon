using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DarkDungeon
{
    public class BSP : MonoBehaviour
    {
        #region Fields
        [SerializeField] int mapWidth = 20;
        [SerializeField] int mapHeight = 10;
        [SerializeField] GroundSO groundSO;
        [SerializeField] GroundSO wallSo;

        GameObject allGround;
        GameObject borders;
        List<Leaf> resultLeafs = new List<Leaf>();
        List<Edge> edges = new List<Edge>();
        List<Graph> graphs = new List<Graph>();

        int startXPos = 0;
        int startYPos = 0;
        int passageWidth = 1;
        int coef = 1;
        int cellularAutomationCount = 5;
        #endregion

        #region Core Methods
        void Awake()
        {
            allGround = new GameObject("AllGround");
            allGround.transform.position = Vector3.zero;
            borders = new GameObject("Borders");
            borders.transform.position = Vector3.zero;
            Leaf root = new Leaf(startXPos, startYPos, mapWidth, mapHeight);
            resultLeafs.Add(root);
        }

        void Start()
        {
            SplitLeafs();
            CreateRooms();

            FillAllGraphs();
            ExecuteCarascalAlgorithm();
            CalculatePassages();

            InitializeCellularAutomation();
            RunCellularAutomation();
            DeleteIslands();

            GenerateGround();
            GenerateWalls();
        }
        #endregion

        #region Methods
        void SplitLeafs()
        {
            bool isSplited = true;
            int count = resultLeafs.Count;

            while (isSplited)
            {
                isSplited = false;
                for (int i = 0; i < count; i ++)
                {
                    if (resultLeafs[i].Split())
                    {
                        resultLeafs.Add(resultLeafs[i].LeftChild);
                        resultLeafs.Add(resultLeafs[i].RightChild);
                        count = count + 2;
                        isSplited = true;
                    }
                }
            }

            resultLeafs = resultLeafs.Where(x => x.IsLastLeaf == true).ToList();
        }

        void CreateRooms()
        {
            foreach (Leaf leaf in resultLeafs)
            {
                leaf.CreateRoom();
            }
        }

        void FillAllGraphs()
        {
            for (int i = 0; i < resultLeafs.Count; i ++)
            {
                Graph graph = new Graph();

                for (int j = 0; j < resultLeafs.Count; j++)
                {
                    if (i == j) continue;

                    foreach (Point point in resultLeafs[j].AllPoints)
                    {
                        if (point.x == resultLeafs[i].x)
                        {
                            if (point.y >= resultLeafs[i].y && point.y <= resultLeafs[i].y + resultLeafs[i].height)
                            {
                                Edge edge = new Edge(resultLeafs[i].center, resultLeafs[j].center);
                                graph.AddEdge(edge);
                            }
                        }

                        else if (point.y == resultLeafs[i].y)
                        {
                            if (point.x >= resultLeafs[i].x && point.x <= resultLeafs[i].x + resultLeafs[i].width)
                            {
                                Edge edge = new Edge(resultLeafs[i].center, resultLeafs[j].center);
                                graph.AddEdge(edge);
                            }
                        }
                    }
                }

                graphs.Add(graph);
            }
        }

        void ExecuteCarascalAlgorithm()
        {
            CarascalAlgorythm carascalAlgorythm = new CarascalAlgorythm();

            foreach (Graph graph in graphs)
            {
                carascalAlgorythm.AddGraph(graph);
            }

            carascalAlgorythm.SortEdges();

            while (edges.Count < resultLeafs.Count - 1)
            {
                if (carascalAlgorythm.Edges.Count == 0) break;
                if (carascalAlgorythm.CanUseEdge(carascalAlgorythm.Edge))
                {
                    List<int> usingSetsNumbers = carascalAlgorythm.FindUsingSets(carascalAlgorythm.Edge);
                    if (usingSetsNumbers.Count == 0)
                    {
                        Set set = new Set();
                        set.AddUsingVertexes(carascalAlgorythm.Edge);
                        carascalAlgorythm.AddSet(set);
                    }
                    else if (usingSetsNumbers.Count == 1)
                    {
                        carascalAlgorythm.AddUsingVertexInSet(carascalAlgorythm.Edge, usingSetsNumbers[0]);
                    }
                    else
                    {
                        carascalAlgorythm.MergeSets(usingSetsNumbers[0], usingSetsNumbers[1]);
                    }
                    edges.Add(carascalAlgorythm.Edge);
                    carascalAlgorythm.RemoveEdge(carascalAlgorythm.Edge);
                }
            }
        }

        void CalculatePassages()
        {
            foreach (Edge edge in edges)
            {
                Leaf[] leafs = SelectPassageRooms(edge);
                Leaf startLeaf = leafs[0];
                Leaf endLeaf = leafs[1];
                bool isFirstRectXLow = startLeaf.Room.endX < endLeaf.Room.x;

                Leaf startPassage = isFirstRectXLow ? startLeaf : endLeaf;
                Leaf endPassage = isFirstRectXLow ? endLeaf : startLeaf;
                UserRect startRect = startPassage.Room;
                UserRect endRect = endPassage.Room;
                UserRect startPassageRect;
                UserRect endPassageRect;

                bool isJointY = TryToFindJointCoordinate(startRect.y, startRect.height, endRect.y, endRect.height);
                bool isJointX = TryToFindJointCoordinate(startRect.x, startRect.width, endRect.x, endRect.width);

                if (isJointY)
                {
                    int randomHeight = CalculatePassagePoint(startRect.y, endRect.y, startRect.height, endRect.height);                   
                    int startWidth = startPassage.EndX - startRect.endX + coef;
                    int endWidth = endRect.x - endPassage.x;
                    startPassageRect = new UserRect(startRect.endX, randomHeight, startWidth, passageWidth);
                    endPassageRect = new UserRect(endPassage.x, randomHeight, endWidth, passageWidth);                   
                }
                else if (isJointX) 
                {
                    int randomWidth = CalculatePassagePoint(startRect.x, endRect.x, startRect.width, endRect.width);                                       
                    bool isStartLower = endRect.y > startRect.endY? true : false;
                    int startY = isStartLower ? startRect.endY : startPassage.y;
                    int startHeight = isStartLower ? startPassage.EndY - startRect.endY + coef : startRect.y - startPassage.y;
                    int endY = isStartLower ? endPassage.y : endRect.endY;
                    int endHeight = isStartLower ? endRect.y - endPassage.y : endPassage.EndY - endRect.endY + coef;
                    startPassageRect = new UserRect(randomWidth, startY, passageWidth, startHeight);
                    endPassageRect = new UserRect(randomWidth, endY, passageWidth, endHeight);                   
                }
                else
                {
                    CalculateAngularPassage(startPassage, endPassage);
                    continue;
                }

                startPassage.AddPassage(startPassageRect);
                endPassage.AddPassage(endPassageRect);
            }
        }

        void CalculateAngularPassage(Leaf startPassage, Leaf endPassage)
        {
            bool isFirstRectXLow = startPassage.Room.endX < endPassage.Room.x;

            int randomStartHeight = Random.Range(startPassage.Room.y, startPassage.Room.endY + 1);
            int randomEndHeight = Random.Range(endPassage.Room.y, endPassage.Room.endY + 1);
            int lowerHeight = randomStartHeight < randomEndHeight ? randomStartHeight : randomEndHeight;
            int higherHeight = randomStartHeight < randomEndHeight ? randomEndHeight : randomStartHeight;
            int yDifference = higherHeight - lowerHeight + coef;
            int startLeafWidth = startPassage.EndX - startPassage.Room.endX + coef;
            int endWidth = endPassage.Room.x - endPassage.x + coef;   
            int xDifference = endPassage.Room.x - startPassage.Room.endX + coef; 
            
            bool isHorizontal = isFirstRectXLow ? startPassage.EndX < startPassage.Room.endX + xDifference : endPassage.EndX < endPassage.Room.endX + xDifference;
            bool isStartHigher = startPassage.Room.y > endPassage.Room.endY ? true : false;

            if (isHorizontal)
            {
               // Debug.Log("isHorizontal " + isStartHigher);
                int endY = isStartHigher ? randomEndHeight : randomStartHeight;
                UserRect startPassageX = new UserRect(startPassage.Room.endX, randomStartHeight, startLeafWidth, passageWidth);
                UserRect endPassageX = new UserRect(endPassage.x, randomEndHeight, endWidth, passageWidth);
                UserRect endPassageY = new UserRect(endPassage.x, endY, passageWidth, yDifference);
                startPassage.AddPassage(startPassageX);
                endPassage.AddPassage(endPassageX);
                endPassage.AddPassage(endPassageY);
            }  
            
            else
            {
                Debug.Log("isVertical " + isStartHigher);
                int startY = isStartHigher ? startPassage.y : randomStartHeight;
                int startHeight = isStartHigher ? randomStartHeight - startPassage.y : startPassage.EndY - randomStartHeight + 1;
                int endY = isStartHigher ? randomEndHeight : endPassage.y;
                int endHeight = isStartHigher ? endPassage.EndY - randomEndHeight + coef : randomEndHeight - endPassage.y;
                UserRect startPassageX = new UserRect(startPassage.Room.endX + coef, randomStartHeight, xDifference, passageWidth);
                UserRect startPassageY = new UserRect(startPassage.Room.endX + xDifference, startY, passageWidth, startHeight);
                UserRect endPassageY = new UserRect(startPassage.Room.endX + xDifference, endY, passageWidth, endHeight);
                startPassage.AddPassage(startPassageX);
                startPassage.AddPassage(startPassageY);
                endPassage.AddPassage(endPassageY);
            }
        }

        bool TryToFindJointCoordinate(int firstStart, int firstLenght, int secondStart, int secondLenght)
        {
            bool isJoint = false;

            if (firstStart + firstLenght >= secondStart + secondLenght && firstStart < secondStart + secondLenght)
            {               
                isJoint = true;             
            }

            if (secondStart + secondLenght >= firstStart + firstLenght && secondStart < firstStart + firstLenght)
            {
                isJoint = true;
            }

            return isJoint;
        }

        int CalculatePassagePoint(int startPoint, int endPoint, int startLenght, int endLenght)
        {
            int start = startPoint > endPoint ? startPoint : endPoint;
            int lenght1 = startPoint + startLenght;
            int lenght2 = endPoint + endLenght;
            int resultLenght = lenght1 < lenght2 ? lenght1 : lenght2;

            return Random.Range(start,resultLenght);
        }

        void InitializeCellularAutomation()
        {
            foreach (Leaf leaf in resultLeafs)
            {
                leaf.GenerateCellularAutomation();
                leaf.FillCellularAutomation();
            }
        }

        void RunCellularAutomation()
        {
            foreach (Leaf leaf in resultLeafs)
            {
                for (int i = 0; i < cellularAutomationCount; i++)
                {
                    leaf.CellularAutomaton.RunCellularAutomation();
                }
            }
        }

        void DeleteIslands()
        {
            foreach (Leaf leaf in resultLeafs)
            {
                leaf.CellularAutomaton.DeleteIslands();
            }
        }
        void GenerateGround()
        {

            foreach (Leaf leaf in resultLeafs)
            {
                int y = 0;

                foreach (CellState[] cellStates in leaf.CellularAutomaton.StateField)
                {
                    int x = 0;

                    foreach (CellState cellState in cellStates)
                    {
                        if (cellState == CellState.alive || cellState == CellState.definitelyAlive)
                        {
                            GameObject ground = GenerateGroundSprite();
                            ground.transform.position = new Vector3(leaf.x + x, leaf.y + y, 2);
                        }

                        x++;
                    }

                    y ++;
                }
            }
        }

        void GenerateWalls()
        {
            foreach (Leaf leaf in resultLeafs)
            {
                List<Point> points = leaf.CellularAutomaton.GetBorderPositions(leaf, out List<Point> border);

                foreach (Point point in points)
                {
                    GameObject ground = GenerateWallSprite();
                    ground.AddComponent<BoxCollider2D>();
                    ground.transform.position = new Vector3(point.x,point.y,2);
                }

                foreach (Point point in border)
                {
                    GameObject ground = new GameObject("Border");
                    ground.AddComponent<BoxCollider2D>();
                    ground.transform.position = new Vector3(point.x, point.y, 2);
                    ground.transform.SetParent(borders.transform);
                }
            }
        }
        #endregion

        #region Support Methods
        Leaf[] SelectPassageRooms(Edge edge)
        {
            Leaf startLeaf = null;
            Leaf endLeaf = null;

            foreach (Leaf leaf in resultLeafs)
            {
                if (leaf.center == edge.vertexA)
                {
                    startLeaf = leaf;
                }
                if (leaf.center == edge.vertexB)
                {
                    endLeaf = leaf;
                }
                if (startLeaf != null && endLeaf != null)
                {
                    break;
                }
            }

            return new Leaf[] { startLeaf, endLeaf };
        }

        GameObject GenerateSprite(Sprite sprite)
        {
            GameObject gameObject = new GameObject("Ground");
            SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            gameObject.transform.SetParent(allGround.transform);

            return gameObject;
        }

        GameObject GenerateGroundSprite()
        {
            Sprite sprite = groundSO.GetRandomSprite();
            return GenerateSprite(sprite);
        }

        GameObject GenerateWallSprite()
        {
            Sprite sprite = wallSo.GetRandomSprite();
            return GenerateSprite(sprite);
        }
        #endregion
    }
}
