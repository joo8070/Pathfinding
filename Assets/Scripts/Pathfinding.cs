using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public int _xPos;
    public int _yPos;
    public int _gValue;
    public int _hValue;
    public PathNode _parent;

    public int FValue
    {
        get
        {
            return _gValue + _hValue;
        }
    }

    public PathNode(int xPos, int yPos)
    {
        _xPos = xPos;
        _yPos = yPos;
    }
}

[RequireComponent(typeof(GridMap))]
public class Pathfinding : MonoBehaviour
{
    GridMap _tile;
    PathNode[,] _pathNodes;

    void Start()
    {
        Init();
    }

    void Init()
    {
        if (_tile == null)
            _tile = GetComponent<GridMap>();

        _pathNodes = new PathNode[_tile.SizeX, _tile.SizeY];
        for (int y = 0; y < _tile.SizeY; y++)
        {
            for (int x = 0; x < _tile.SizeX; x++)
                _pathNodes[x, y] = new PathNode(x, y);
        }
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = _pathNodes[startX, startY];
        PathNode endNode = _pathNodes[endX, endY];

        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            PathNode currentNode = openList[0];
            for (int i = 0; i < openList.Count; i++)
            {
                if(currentNode.FValue > openList[i].FValue)
                    currentNode = openList[i];

                if(currentNode.FValue == openList[i].FValue && currentNode._hValue > openList[i]._hValue)
                    currentNode = openList[i];

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                if(currentNode == endNode)
                    return RetracePath(startNode, endNode);

                List<PathNode> neighborNodes = new List<PathNode>();
                for (int y = -1; y < 2; y++)
                {
                    for (int x = -1; x < 2; x++)
                    {
                        if (x == 0 && y == 0)
                            continue;

                        if (_tile.CheckOutOfBounds(currentNode._xPos + x, currentNode._yPos + y) == false)
                            continue;

                        neighborNodes.Add(_pathNodes[currentNode._xPos + x, currentNode._yPos + y]);
                    }
                }

                for (int jj = 0; jj < neighborNodes.Count; jj++)
                {
                    if (closedList.Contains(neighborNodes[jj]))
                        continue;

                    if (_tile.CanGo(neighborNodes[jj]._xPos, neighborNodes[jj]._yPos) == false)
                        continue;

                    int movementCost = currentNode._gValue + CalcDistance(currentNode, neighborNodes[jj]);
                    if(openList.Contains(neighborNodes[jj]) == false || movementCost < neighborNodes[jj]._gValue)
                    {
                        neighborNodes[jj]._gValue = movementCost;
                        neighborNodes[jj]._hValue = CalcDistance(neighborNodes[jj], endNode);
                        neighborNodes[jj]._parent = currentNode;

                        if (openList.Contains(neighborNodes[jj]) == false)
                            openList.Add(neighborNodes[jj]);
                    }
                }
            }
        }

        return null;
    }

    List<PathNode> RetracePath(PathNode startNode, PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        PathNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode._parent;
        }
        path.Reverse();

        return path;
    }

    int CalcDistance(PathNode current, PathNode target)
    {
        int distX = Mathf.Abs(current._xPos - target._xPos);
        int distY = Mathf.Abs(current._yPos - target._yPos);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);

        return 14 * distX + 10 * (distY - distX);
    }
}
