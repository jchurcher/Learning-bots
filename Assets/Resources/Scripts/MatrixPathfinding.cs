using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixPathfinding : MonoBehaviour
{
    public static List<Node> ApplyAStar(Grid grid)
    {
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();

        openList.Add(grid.start);

        bool endSearch = false;

        while (true)
        {
            if(openList.Count == 0)
            {
                break;
            }
            if (endSearch)
            {
                break;
            }

            openList.Sort(SortNodesByf);

            Node q = openList[0];
            openList.RemoveAt(0);

            List<Node> successors = new List<Node>();

            for (int i=-1; i<2; i++)
            {
                for (int j=-1; j<2; j++)
                {
                    if (i == 0 && j == 0)
                        continue;

                    Node s;

                    try
                    {
                        Vector2 newCoord = q.GetCoord() + new Vector2(i, j);
                        s = grid.GetNode(newCoord);
                    }
                    catch
                    {
                        continue;
                    }

                    if (s.value.Equals(ObjectStates.Wall))
                        continue;

                    if (i * j == 0)
                        s.g = 10;
                    else
                        s.g = 14;

                    successors.Add(s);
                }
            }

            foreach(Node node in successors)
            {
                if (node.value.Equals(ObjectStates.End))
                {
                    endSearch = true;
                    break;
                }
                else
                {
                    float h = Vector2.Distance(node.GetCoord(), grid.end.GetCoord());
                    float g = q.g + node.g;
                    float f = h + g;

                    node.h = h;
                    node.g = g;
                    node.f = f;
                }

                bool skipFlag = false;
                foreach(Node i in openList)
                {
                    if (i.GetCoord() != node.GetCoord())
                    {
                        continue;
                    }

                    if (i.f <= node.f)
                    {
                        skipFlag = true;
                        break;
                    }
                }

                if (skipFlag)
                    continue;

                skipFlag = false;
                foreach (Node i in closedList)
                {
                    if (i.GetCoord() != node.GetCoord())
                    {
                        continue;
                    }

                    if (i.f <= node.f)
                    {
                        skipFlag = true;
                        break;
                    }
                }

                if (skipFlag)
                    continue;

                openList.Add(node);
                node.parent = q;
            }

            closedList.Add(q);

        }

        Node currentNode = closedList[closedList.Count-1];
        List<Node> path = new List<Node>();

        while (currentNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
            //print(currentNode);
        }

        //print("closed: " + closedList.Count + ", open: " + openList.Count);
        return path;
    }

    private static int SortNodesByf(Node a, Node b)
    {
        if (a.f < b.f)
        {
            return -1;
        }
        if (a.f > b.f)
        {
            return 1;
        }

        return 0;
    }

    public static void PopulateGrid(ref Grid grid)
    {
        int width = grid.width;
        int height = grid.height;
        Vector2 minCoords = grid.Index2Coord(new Vector2(0,0));
        Vector2 maxCoords = grid.Index2Coord(new Vector2(width, height));

        for (int i = (int)minCoords.x; i < (int)maxCoords.x; i++)
        {
            for (int j = (int)minCoords.y; j < (int)maxCoords.y; j++)
            {
                Vector2 pos = new Vector2(i, j);
                Collider2D coll = Physics2D.OverlapCircle(pos, 1f);

                if (coll)
                {
                    pos = grid.Coord2Index(pos);
                    Node node = grid.GetNode(pos);

                    if (coll.gameObject.CompareTag("Wall") || coll.gameObject.CompareTag("Boundary"))
                    {
                        node.value = ObjectStates.Wall;
                    }
                    else if (coll.gameObject.transform.position.Equals(new Vector2(i, j)))
                    {
                        if (coll.gameObject.CompareTag("Target"))
                        {
                            node.value = ObjectStates.End;
                        }
                        else if (coll.gameObject.CompareTag("Player"))
                        {
                            node.value = ObjectStates.Start;
                        }
                    }

                    grid.SetNode(node);
                }
            }
        }

        /*print("Grid Created");
        string sent = "[";
        for(int i=0; i<grid.width; i++)
        {
            sent += "[";
            for(int j=0; j<grid.height; j++)
            {
                int val = (int)grid.GetNode(new Vector2(i, j)).value;
                sent += val;
                sent += ",";
                if (val > 1)
                {
                    print("more than 1");
                }
            }
            sent += "],";
        }
        sent += "]";
        print(sent);*/
    }

    public static List<Vector2> GetNodeCoordinates(List<Node> nodes, in Grid grid)
    {
        List<Vector2> path = new List<Vector2>();

        foreach (Node node in nodes)
        {
            Vector2 pos = node.GetCoord();
            pos = grid.Index2Coord(pos);
            path.Add(pos);
        }

        return path;
    }

    /*private void GetMap(){
        int[,] mapMatrix = new int[(int)mapSize.x, (int)mapSize.y];

        for(int i=0; i<mapSize.x; i++){
            for(int j=0; j<mapSize.y; i++){
                if(Physics.CheckSphere(new Vector2(i,j), 1)){
                    mapMatrix[i,j] = 1;
                }
                else{
                    mapMatrix[i,j] = 0;
                }
            }
        }
    }*/

}
