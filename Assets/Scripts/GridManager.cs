using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(GridMap))]
[RequireComponent(typeof(Tilemap))]
public class GridManager : MonoBehaviour
{
    int _currentX = 0;
    int _currentY = 0;
    int _targetX = 0;
    int _targetY = 0;

    Pathfinding _pathfinding;
    [SerializeField] TileSet _tileset;
    [SerializeField] Tilemap _collision;

    List<PathNode> _currentNodes = new List<PathNode>();
    Tilemap _tilemap;
    GridMap _tile;

    void Awake()
    {
        _tilemap = GetComponent<Tilemap>();
        _tile= GetComponent<GridMap>();
        _pathfinding = GetComponent<Pathfinding>();
        //_tile.Init(13, 11);
        //RefreshTilemap();
        LoadCollisionMap();
    }

    //void RefreshTilemap()
    //{
    //    for (int y = 0; y < _tile.SizeY; y++)
    //    {
    //        for (int x = 0; x < _tile.SizeX; x++)
    //            _tilemap.SetTile(new Vector3Int(x, y, 0), _tileset.tiles[(int)_tile.Get(x,y)]);
    //    }
    //}

    void LoadCollisionMap()
    {
        int sizeX = _tilemap.cellBounds.xMax - _tilemap.cellBounds.xMin + 1;
        int sizeY = _tilemap.cellBounds.yMax - _tilemap.cellBounds.yMin + 1;
        _tile.Init(sizeX, sizeY);

        for (int y = _tilemap.cellBounds.yMax; y >= _tilemap.cellBounds.yMin; y--)
        {
            for (int x = _tilemap.cellBounds.xMin; x <= _tilemap.cellBounds.xMax; x++)
            {
                Debug.Log($"x({x}), y({y})");
                TileBase tile = _collision.GetTile(new Vector3Int(x, y, 0));
                if(tile != null)
                    _tile.Set(x, y, TileColor.White);
            }
        }
    }

    public void SetTile(int x, int y, TileColor color)
    {
        if (_tile.CheckOutOfBounds(x, y) == false)
            return;

        _tile.Set(x, y, color);
        _tilemap.SetTile(new Vector3Int(x, y, 0), _tileset.tiles[(int)_tile.Get(x, y)]);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ClearCurrentNodes();
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int tilePos = _tilemap.WorldToCell(mousePos);

            _targetX = tilePos.x;
            _targetY = tilePos.y;

            List<PathNode> path = _pathfinding.FindPath(_currentX, _currentY, _targetX, _targetY);
            _currentNodes = path;
            StartCoroutine(CoSetTile());

            _currentX = _targetX;
            _currentY = _targetY;
        }
    }

    IEnumerator CoSetTile()
    {
        if (_currentNodes != null)
        {
            for (int i = 0; i < _currentNodes.Count; i++)
            {
                SetTile(_currentNodes[i]._xPos, _currentNodes[i]._yPos, TileColor.Red);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    void ClearCurrentNodes()
    {
        if (_currentNodes != null)
        {
            for (int i = 0; i < _currentNodes.Count; i++)
                SetTile(_currentNodes[i]._xPos, _currentNodes[i]._yPos, TileColor.Black);
        }
    }
}
