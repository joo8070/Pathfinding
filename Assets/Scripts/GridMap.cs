using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileColor
{
    None = -1,
    Black,
    White,
    Blue,
    Red
}

public class GridMap : MonoBehaviour
{
    TileColor[,] _tile;

    public int SizeX { get; private set; }
    public int SizeY { get; private set; }
    
    public void Init(int x, int y)
    {
        _tile = new TileColor[x, y];
        SizeY = y;
        SizeX = x;
    }

    public void Set(int x, int y, TileColor tileSet)
    {
        if (CheckOutOfBounds(x, y) == false)
            return;

        _tile[x, y] = tileSet;
    }

    public TileColor Get(int x,int y)
    {
        if (CheckOutOfBounds(x, y) == false)
            return TileColor.None;

        return _tile[x, y];
    }

    public bool CheckOutOfBounds(int x, int y)
    {
        if (y < 0 || y >= SizeY || x < 0 || x >= SizeX)
            return false;

        return true;
    }

    public bool CanGo(int xPos, int yPos)
    {
        return _tile[xPos, yPos] == TileColor.Black;
    }
}
