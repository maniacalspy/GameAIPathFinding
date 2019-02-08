using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameAIPathfinding
{
    class Program
    {
        static void Main(string[] args)
        {


            bool [,] map = new bool[10, 10] { {true, true, true, true, true, true , true, true, true, true},
                                       { true, true, false, true, true, true, true, true, true, true },
                                        {true, true, true, true, true, false, false, false, true, true},
                                        {true, false, false, false, true, true , true, true, true, true},
                                        {true, true, true, true, true, true , true, false, false, false},
                                        {false, false, false, true, true, true , true, true, true, true},
                                        {true, true, true, true, true, false, false, true, true, true},
                                        {true, true, true, true, true, true , true, true, true, true},
                                        {true, false, false, false, false, true , true, false, false, false},
                                        {true, true, true, true, true, true , true, true, true, true}
            };

            Maze m = new Maze(map,0,0,9,9);

            Agent a = new Agent(m);

            a.Findpath();

            Console.ReadKey();
        }

    } //END OF PROGRAM CLASS

    class Maze
    {
        public bool[,] map;
        public int XStart, YStart, XGoal, YGoal;

        public Maze() : this(new bool[10, 10], 0, 0, 9, 9)
        {
        }

        public Maze(bool[,] m, int xstart, int ystart, int xgoal, int ygoal)
        {
            map = m;
            XStart = xstart;
            YStart = ystart;
            XGoal = xgoal;
            YGoal = ygoal;
        }
    }//END OF MAZE CLASS

    class Node
    {
        public bool IsWall = false;
        public Node Parent;
        public int GScore;
        public int HScore;
        public int FScore;
        public int X;
        public int Y;

        //default constructor to make nodes with arbitrarily high scores, so that when you first check the node you'll update it
        public Node()
        {
            IsWall = false;
            Parent = null;
            GScore = 10000;
            HScore = 10000;
            FScore = 20000;
            X = 100;
            Y = 100;
        }

        //constructor
        public Node(bool newwall, Node newparent, int newGscore, int newHscore, int newFscore, int newX, int newY)
        {
            IsWall = newwall;
            Parent = newparent;
            GScore = newGscore;
            HScore = newHscore;
            FScore = newFscore;
            X = newX;
            Y = newY;
        }
    }//END OF NODE CLASS 

    class Agent
    {
        int CurrentX;
        int CurrentY;
        int NewNodeX;
        int NewNodeY;

        Node CurrentNode;
        public Maze MyMaze;

        int HorizontalWeight = 10;
        int DiagonalWeight = 14;


        Node[,] WorkingMaze;

        List<Node> OpenNodes = new List<Node>();
        List<Node> ClosedNodes = new List<Node>();

        public Agent() : this(new Maze())
        {

        }

        public Agent(Maze m)
        {
            MyMaze = m;
            WorkingMaze = new Node[MyMaze.map.GetLength(0), MyMaze.map.GetLength(1)];
            for (int x = 0; x < WorkingMaze.GetLength(0); x++)
            {
                for (int y = 0; y < WorkingMaze.GetLength(1); y++)
                {
                    WorkingMaze[x, y] = new Node();
                }
            }
        }


        public void Findpath()
        {
            CurrentX = MyMaze.XStart;
            CurrentY = MyMaze.YStart;

            //Create a new node for the starting position
            WorkingMaze[CurrentX, CurrentY] = new Node(false, null, 0, CalcHScore(CurrentX, CurrentY), CalcHScore(CurrentX, CurrentY), CurrentX, CurrentY);

            //Set the starting node's parent to be itself, helps us break a loop later
            WorkingMaze[CurrentX, CurrentY].Parent = WorkingMaze[CurrentX, CurrentY];

            //Add starting node to open
            OpenNodes.Add(WorkingMaze[CurrentX, CurrentY]);

            //While not currently standing on the goal. MAIN WHILE LOOP
            while (CurrentX != MyMaze.XGoal || CurrentY != MyMaze.YGoal)
            {
                Node templowest = new Node(); //make a node with the large default values to basically be a placeholder
                                              //check for the lowest fscore in open
                foreach (Node n in OpenNodes)
                {
                    if (n.FScore <= templowest.FScore)
                    {
                        if (n.GScore < templowest.GScore) templowest = n; //prefer the lower GScore
                    }
                }

                //set the new current node and update our position, open list, and closed list
                CurrentNode = templowest;
                CurrentX = CurrentNode.X;
                CurrentY = CurrentNode.Y;
                OpenNodes.Remove(CurrentNode);
                ClosedNodes.Add(CurrentNode);

                //if we're done now, make a list of the path there and print it
                if (CurrentNode.X == MyMaze.XGoal && CurrentNode.Y == MyMaze.YGoal)
                {
                    List<String> path = new List<String>();
                    while (CurrentNode.Parent != CurrentNode)
                    {
                        path.Add($"{CurrentNode.X}, {CurrentNode.Y}");
                        CurrentNode = CurrentNode.Parent;
                    }
                    Console.WriteLine($"{MyMaze.XStart}, {MyMaze.YStart}");
                    for (int i = path.Count - 1; i >= 0; i--)
                    {
                        Console.WriteLine(path[i]);
                    }
                    return; //EXIT POINT OF FINDPATH
                }

                //check all the nodes around our location to look for walls and unexplored nodes
                for (NewNodeX = CurrentX - 1; NewNodeX <= CurrentX + 1; NewNodeX++)
                {
                    for (NewNodeY = CurrentY - 1; NewNodeY <= CurrentY + 1; NewNodeY++)
                    {
                        if (NewNodeX != CurrentX || NewNodeY != CurrentY)
                        {
                            if (NewNodeX >= 0 && NewNodeX <= 9 && NewNodeY >= 0 && NewNodeY <= 9)
                            {
                                if (!MyMaze.map[NewNodeX, NewNodeY]) WorkingMaze[NewNodeX, NewNodeY].IsWall = true;

                                if (!(WorkingMaze[NewNodeX, NewNodeY].IsWall) && !ClosedNodes.Contains(WorkingMaze[NewNodeX, NewNodeY]))
                                {
                                    if (calcScore(WorkingMaze[NewNodeX, NewNodeY]) || !OpenNodes.Contains(WorkingMaze[NewNodeX, NewNodeY]))
                                    {
                                        WorkingMaze[NewNodeX, NewNodeY].Parent = CurrentNode;
                                        WorkingMaze[NewNodeX, NewNodeY].X = NewNodeX;
                                        WorkingMaze[NewNodeX, NewNodeY].Y = NewNodeY;
                                        if (!OpenNodes.Contains(WorkingMaze[NewNodeX, NewNodeY])) OpenNodes.Add(WorkingMaze[NewNodeX, NewNodeY]);
                                    } //END OF CALCSCORE AND CHECKING OPENNODES IF
                                }//END OF ISWALL AND CHECKING CLOSEDNODES IF
                            }//END OF CHECKING IF COORDINATES ARE INSIDE THE MAZE IF
                        }//END OF CHECKING IF NODE COORDINATES ARE DIFFERENT IF
                    }//END OF Y COORDINATE FOR LOOP
                }//END OF X COORDINATE FOR LOOP

            }//END OF WHILE LOOP
        }//END OF FINDPATH METHOD

        //calculate all the scores for a given node, returns true if a node's score is updated, otherwise returns false
        public bool calcScore(Node n)
        {
            int tempGscore = WorkingMaze[CurrentX, CurrentY].GScore;
            if (NewNodeX != CurrentX && NewNodeY != CurrentY) tempGscore += DiagonalWeight;
            else tempGscore += HorizontalWeight;

            int tempHscore = CalcHScore(NewNodeX, NewNodeY);

            int tempFscore = tempGscore + tempHscore;

            if (n.FScore > tempFscore)
            {
                n.GScore = tempGscore;
                n.HScore = tempHscore;
                n.FScore = tempFscore;
                return true;
            }
            return false;
        }

        //calculate the H score given a set of coordinates
        public int CalcHScore(int x, int y)
        {
            int Score = 0;
            int XDist = MyMaze.XGoal - x;
            int YDist = MyMaze.YGoal - y;
            while (XDist != 0 && YDist != 0)
            {
                if (XDist < 0) XDist += 1;
                else XDist -= 1;
                if (YDist < 0) YDist += 1;
                else YDist -= 1;
                Score += DiagonalWeight;
            }
            while (XDist != 0 || YDist != 0)
            {
                if (XDist < 0) XDist += 1;
                else if (XDist > 0) XDist -= 1;
                if (YDist < 0) YDist += 1;
                else if (YDist > 0) YDist -= 1;
                Score += HorizontalWeight;
            }
            return Score;
        }
    }//END OF AGENT CLASS
}//END OF NAMESPACE

