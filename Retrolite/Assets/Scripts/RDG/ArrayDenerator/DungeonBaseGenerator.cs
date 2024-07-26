using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonBaseGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _roomPrefab;
    [SerializeField] private Tile[] _tiles;
    [SerializeField] private RuleTile _wall;
    [SerializeField] private RuleTile _wallTop;
    [SerializeField] private Tilemap _floor;
    [SerializeField] private Tilemap _walls;
    [SerializeField] private Tilemap _top;
    [SerializeField] private int _maxRoomSize;
    [SerializeField] private int _minRoomSize;
    [SerializeField] private int _maxRooms;
    [SerializeField] private int _minRooms;
    [SerializeField] private int _maxCorridor;
    [SerializeField] private int _minCorridor;

    private int _roomsCount;
    private Room[] _rooms;
    private short[,] _dungeon = new short[500, 500];
    private Vector3Int _position = new Vector3Int(250,250);
    
    private void Start()
    {
        MakeDungeon();
        CleanUpDungeon();
        CleanUpDungeon();
        CleanUpDungeon();
        SpawnDungeon();
    }

    protected Vector3Int GetRandomVector()
    {
        switch (Random.Range(0, 4))
        {
            case 0:
                if(_position.y > 799) return Vector3Int.down;
                else return Vector3Int.up;
            case 1:
                if(_position.y < 1) return Vector3Int.up;
                else return Vector3Int.down;
            case 2:
                if(_position.x > 799) return Vector3Int.left;
                else return Vector3Int.right;
            case 3:
                if(_position.x < 1) return Vector3Int.right;
                else return Vector3Int.left;
        }
        return Vector3Int.zero;
    }

    protected Vector3Int GetVector3Int(int x, int y)
    {
        return new Vector3Int(x - 250, y - 250, 0);
    }

    protected virtual void SetTile(short[,] layer, short i = 2)
    {
        layer[_position.x, _position.y] = 1;
        if(layer[_position.x + 1, _position.y] == 0)layer[_position.x + 1, _position.y] = i;
        if(layer[_position.x - 1, _position.y] == 0)layer[_position.x - 1, _position.y] = i;
        if(layer[_position.x, _position.y + 1] == 0)layer[_position.x, _position.y + 1] = i;
        if(layer[_position.x, _position.y - 1] == 0)layer[_position.x, _position.y - 1] = i;
        if(layer[_position.x + 1, _position.y + 1] == 0)layer[_position.x + 1, _position.y + 1] = i;
        if(layer[_position.x + 1, _position.y - 1] == 0)layer[_position.x + 1, _position.y - 1] = i;
        if(layer[_position.x - 1, _position.y + 1] == 0)layer[_position.x - 1, _position.y + 1] = i;
        if(layer[_position.x - 1, _position.y - 1] == 0)layer[_position.x - 1, _position.y - 1] = i;
    }

    protected virtual void MakeRoom(short id)
    {
        int r = Random.Range(_minRoomSize, _maxRoomSize);
        int roomSize;
        Room currentRoom = Instantiate(_roomPrefab,transform.GetChild(0)).GetComponent<Room>();
        _rooms[id] = currentRoom;

        Vector3Int startPosition = _position;

        for (roomSize = 0; roomSize < r;)
        {
            _position += GetRandomVector();

            if(_dungeon[_position.x, _position.y] == 0 || _dungeon[_position.x, _position.y] == id + 3 || _dungeon[_position.x, _position.y] == 2)
            {
                roomSize++;
                SetTile(_dungeon, (short)(id + 3));
            }
            currentRoom.Trigger.SetTile(GetVector3Int(_position.x,_position.y), _tiles[0]); 
            if(roomSize % 25 == 24 && id != 0) 
            {
                roomSize++;
                _dungeon[_position.x, _position.y] = (short)(-id - 1);
            }
            if(roomSize % 10 == 9) 
            {
                roomSize++;
                _position = startPosition;
            }
        }
    }

    protected virtual void MakeCorridor()
    {
        int steps = Random.Range(_minCorridor, _maxCorridor);

        Vector3Int direction = GetRandomVector();

        for(int u = 0; u < steps; u++)
        {
            if(_position.x + direction.x > 498 || _position.y + direction.y > 498 || _position.x + direction.x < 2 || _position.y + direction.y < 2) break; 
            _position += direction;
            SetTile(_dungeon);
        }
        _position += direction;
    }

    protected virtual void MakeDungeon()
    {
        _roomsCount = Random.Range(_minRooms, _maxRooms);
        _rooms = new Room[_roomsCount];
        MakeRoom(0);    

        for (short i = 1; i < _roomsCount; i++)
        {
            MakeCorridor();
            while(CheckTiles() > 6) 
            {
                MakeCorridor();
            }
            MakeRoom(i);
            if(i % 15 == 14) 
            {
                i++;
                _position = new Vector3Int(250,250);
            }
        }
    }

    private int CheckTiles()
    {
        int i = 0;

        if (_dungeon[_position.x + 1, _position.y] != 0) i++;
        if (_dungeon[_position.x - 1, _position.y] != 0) i++;
        if (_dungeon[_position.x, _position.y + 1] != 0) i++;
        if (_dungeon[_position.x, _position.y - 1] != 0) i++;
        if (_dungeon[_position.x + 1, _position.y + 1] != 0) i++;
        if (_dungeon[_position.x + 1, _position.y - 1] != 0) i++;
        if (_dungeon[_position.x - 1, _position.y + 1] != 0) i++;
        if (_dungeon[_position.x - 1, _position.y - 1] != 0) i++;

        return i;
    }

    protected virtual void CleanUpDungeon()
    {
        for (int x = 1; x < 499; x++)  
        {
            for (int y = 1; y < 499; y++)
            {
                int i = 0;

                if (_dungeon[x, y] > 1)  
                {
                    if (_dungeon[x + 1, y] == 1) i++;
                    if (_dungeon[x - 1, y] == 1) i++;
                    if (_dungeon[x, y + 1] == 1) i++;
                    if (_dungeon[x, y - 1] == 1) i++;
                }

                if (i > 2)
                {
                    _dungeon[x, y] = 1;
                }
            }
        }
    }
    protected virtual void SpawnDungeon()
    {
        for (int x = 0; x < 500; x++)
        {
            for (int y = 0; y < 500; y++)
            {
                if(_dungeon[x, y] < 0)
                {
                    _floor.SetTile(GetVector3Int(x,y), _tiles[0]);
                    _rooms[-_dungeon[x, y] - 1].AddEnemy(x - 250, y - 250);
                }
                else
                {
                    switch (_dungeon[x, y])
                    {
                        case 0:
                            //_top.SetTile(GetVector3Int(x,y), _wallTop);
                            break;
                        case 1:
                            _floor.SetTile(GetVector3Int(x,y), _tiles[0]);
                            break;
                        default:
                            _floor.SetTile(GetVector3Int(x,y), _tiles[0]);
                            _walls.SetTile(GetVector3Int(x,y), _wall);
                            _top.SetTile(GetVector3Int(x,y), _wallTop);
                            break;
                    }
                }
            }
        }
    }
}
