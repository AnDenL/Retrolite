using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomUpdater : MonoBehaviour
{
    public GameObject[,] dungeon = new GameObject[30, 30];
    private int lastX,lastY;
    private void Start()
    {
        UpdateDungeon();
    }
    public void UpdateDungeon(int x = 15,int y = 15)
    {
        
        if(lastX != 0)
        {
            if(dungeon[lastX + 1,lastY] != null) dungeon[lastX + 1,lastY].SetActive(false);
            if(dungeon[lastX - 1,lastY] != null) dungeon[lastX - 1,lastY].SetActive(false);
            if(dungeon[lastX,lastY + 1] != null) dungeon[lastX,lastY + 1].SetActive(false);
            if(dungeon[lastX,lastY - 1] != null) dungeon[lastX,lastY - 1].SetActive(false);
        }
        dungeon[x,y].SetActive(true);
        if(dungeon[x + 1,y] != null) dungeon[x + 1,y].SetActive(true);
        if(dungeon[x - 1,y] != null) dungeon[x - 1,y].SetActive(true);
        if(dungeon[x,y + 1] != null) dungeon[x,y + 1].SetActive(true);
        if(dungeon[x,y - 1] != null) dungeon[x,y - 1].SetActive(true);
        lastX = x;
        lastY = y;
    }
}
