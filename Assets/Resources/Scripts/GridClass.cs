using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectStates
{
    Empty,
    Wall,
    Start,
    End
}

public class Node
{
    public int x { get; }
    public int y { get; }
    public float g, h, f = 0;
    public ObjectStates value;
    public Node parent = null;
    public Node(int x, int y, ObjectStates value=0)
    {
        this.x = x;
        this.y = y;
        this.value = value;
    }

    public Vector2 GetCoord()
    {
        return new Vector2(x, y);
    }
}

public class Grid
{
    public int width { get; }
    public int height { get; }

    public Node start;
    public Node end;

    private Node[,] grid;

    public Grid(int width, int height)
    {
        this.width = width + 2;
        this.height = height + 2;

        start = null;
        end = null;

        grid = new Node[this.width, this.height];

        InitGrid();
    }

    private void InitGrid()
    {
        for(int i=0; i<width; i++)
        {
            for(int j=0; j<height; j++)
            {
                grid[i, j] = new Node(i, j);
            }
        }
    }

    public void SetNode(Node node)
    {
        grid[node.x, node.y] = node;
        if (node.value.Equals(ObjectStates.Start))
        {
            start = node;
            start.g = 0;
            start.h = 0;
            start.f = 0;
        }
        else if (node.value.Equals(ObjectStates.End))
        {
            end = node;
        }
    }

    public void SetNodeValueAtCoord(Vector2 position, ObjectStates value)
    {
        position = Coord2Index(position);

        int x = (int)position.x;
        int y = (int)position.y;

        Node node = grid[x, y];
        node.value = value;

        switch (value)
        {
            case ObjectStates.Start:
                start = node;
                break;
            case ObjectStates.End:
                end = node;
                break;
        }
    }

    public Node GetNode(Vector2 coord)
    {
        int x = (int)coord.x;
        int y = (int)coord.y;
        return grid[x,y];
    }

    public Vector2 Coord2Index(Vector2 coordinate)
    {
        return coordinate + new Vector2((width - 1) / 2, (height - 1) / 2);
    }

    public Vector2 Index2Coord(Vector2 index)
    {
        return index - new Vector2((width-1) / 2, (height-1) / 2);
    }
}

/*public class Grid
{
    private int width;
    private int height;
    private Vector2 origin;

    private int[,] gridMap;

    public enum Objects
    {
        Empty,
        Wall,
        Start,
        End
    }


    // Object initalisation
    public Grid(int width, int height, Vector2 origin)
    {
        this.width = width;
        this.height = height;
        this.origin = origin;

        this.gridMap = new int[width + 1, height + 1];
    }

    *//*// Takes in object name as string, sets value as integer
    public void SetCell(Vector2 coordinate, Objects objectName)
    {
        Vector2 index = Coord2Index(coordinate);
        this.gridMap[(int)index.x, (int)index.y] = (int)objectName;
    }*//*

    public void SetWall(Vector2 coordinate)
    {
        Vector2 index = Coord2Index(coordinate);
        this.gridMap[(int)index.x, (int)index.y] = (int)Objects.Wall;
    }

    public void SetStart(Vector2 coordinate)
    {
        Vector2 index = Coord2Index(coordinate);
        this.gridMap[(int)index.x, (int)index.y] = (int)Objects.Start;
    }

    public void SetEnd(Vector2 coordinate)
    {
        Vector2 index = Coord2Index(coordinate);
        this.gridMap[(int)index.x, (int)index.y] = (int)Objects.End;
    }

    private Vector2 Coord2Index(Vector2 coordinate)
    {
        coordinate -= origin;
        coordinate += new Vector2(width, height);
        return coordinate;
    }

    private Vector2 Index2Coord(Vector2 index)
    {
        index -= new Vector2(width, height);
        index += origin;
        return index;
    }
}*/
