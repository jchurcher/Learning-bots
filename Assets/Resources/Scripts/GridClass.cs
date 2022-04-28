using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int x;
    public int y;
    private float g, h, f = 0;
    private int value;
    private Node parent = null;
    public Node(int x, int y, int value)
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

/*public class Grid
{

}*/

public class Grid
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

    // Takes in object name as string, sets value as integer
    public void SetCell(Vector2 coordinate, Objects objectName)
    {
        Vector2 index = Coord2Index(coordinate);
        this.gridMap[(int)index.x, (int)index.y] = (int)objectName;
    }

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
}
