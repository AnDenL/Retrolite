using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int maxRooms = 10;
    public int roomSpawns = 3;
    public Minimap minimap;
    public GameObject roomPrefab;
    public GameObject[] wallPrefabs;
    public GameObject[] roomFillPrefabs;
    public RoomUpdater opt;
    private int maxX = 15, maxY = 15;
    private int[,] dungeon = new int[30, 30];
    private void Awake()
    {
        while (roomSpawns > 0)
        {
            maxRooms++;
            GenerateDungeon(15, 15);
            roomSpawns--;
        }
        PlaceRooms();
    }
    
    private void GenerateDungeon(int startX, int startY)
    {
        int rooms = 0;
        while (rooms < maxRooms)
        {
            if (dungeon[startX, startY] == 0)
            {
                dungeon[startX, startY] = 1;
                rooms++;
                if (Mathf.Abs(maxX - 15) + Mathf.Abs(maxY - 15) < Mathf.Abs(startX - 15) + Mathf.Abs(startY - 15))
                {
                    maxX = startX;
                    maxY = startY;
                }
            }
            switch (Random.Range(0, 4))
            {
                case 0:
                    startX++;
                    break;
                case 1:
                    startX--;
                    break;
                case 2:
                    startY++;
                    break;
                case 3:
                    startY--;
                    break;
            }
            startX = Mathf.Clamp(startX, 1, 29);
            startY = Mathf.Clamp(startY, 1, 29);
        }
        if (roomSpawns == 1) dungeon[startX, startY] = 3;
        else dungeon[startX, startY] = 2;
    }
    
    private void PlaceRooms()
    {
        while (dungeon[maxX,maxY] == 0)
        {
            switch (Random.Range(0,4)){
                case 0:
                    maxX++;
                    break;
                case 1:
                    maxX--;
                    break;
                case 2:
                    maxY++;
                    break;
                case 3:
                    maxY--;
                    break;
            }
        }
        dungeon[maxX, maxY] = 4;
        for (int i = 0; i < 30; i++)
        {
            for (int j = 0; j < 30; j++)
            {
                if (dungeon[i, j] != 0)
                {
                    GameObject currentRoom = Instantiate(roomPrefab, new Vector2((i - 15) * 30, (j - 15) * 16), Quaternion.identity);
                    Transform troom = currentRoom.GetComponent<Transform>();
                    CameraUpdater set = currentRoom.GetComponent<CameraUpdater>();
                    set.x = i;
                    set.y = j;
                    set.opt = opt;
                    for (int h = 0; h < 4; h++)
                    {
                        Vector3 wallPos = new Vector3((i - 15) * 30, (j - 15) * 16, -1);
                        switch (h)
                        {
                            case 0:
                                if (i + 1 < 30 && dungeon[i + 1, j] == 0)
                                    Instantiate(wallPrefabs[h],troom.GetChild(0));
                                break;
                            case 1:
                                if (i - 1 >= 0 && dungeon[i - 1, j] == 0)
                                    Instantiate(wallPrefabs[h],troom.GetChild(0));
                                break;
                            case 2:
                                if (j + 1 < 30 && dungeon[i, j + 1] == 0)
                                    Instantiate(wallPrefabs[h],troom.GetChild(0));
                                break;
                            case 3:
                                if (j - 1 >= 0 && dungeon[i, j - 1] == 0)
                                    Instantiate(wallPrefabs[h],troom.GetChild(0));
                                break;
                        }
                    }
                    if(i != 15 || j != 15){
                        switch (dungeon[i, j])
                        {
                            case 1 :
                                Instantiate(roomFillPrefabs[Random.Range(0,roomFillPrefabs.Length - 6)], troom.GetChild(0));
                                break;
                            case 2 :    
                                Instantiate(roomFillPrefabs[Random.Range(roomFillPrefabs.Length - 2, roomFillPrefabs.Length - 4)], troom.GetChild(0));
                                break;
                            case 3 :
                                Instantiate(roomFillPrefabs[Random.Range(roomFillPrefabs.Length - 4, roomFillPrefabs.Length - 7)], troom.GetChild(0));
                                break;
                            case 4 :
                                Instantiate(roomFillPrefabs[roomFillPrefabs.Length - 1], troom.GetChild(0));
                                break;
                        }
                    }
                    opt.dungeon[i,j] = currentRoom;
                    currentRoom.SetActive(false);
                }
            }
        }
        minimap.dungeon = dungeon;
    }
}
