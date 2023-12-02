using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    public GameObject miniRoom;
    public Transform miniMap, map, frame;
    public Vector2 minimapPos;
    public GameObject[] icons;
    public Animator Text;
    public int[,] dungeon = new int[30, 30];
    public float DefaultScaleX, DefaultScaleY;
    private float ScaleX, ScaleY;
    private bool[,] visibleDungeon = new bool[30, 30];
    private Image[,] Rooms = new Image[30, 30];
    private GameObject player;
    private Transform roomsParent;
    private void Start()
    {   
        minimapPos = new Vector2(frame.position.x,frame.position.y);
        player = GameObject.FindGameObjectWithTag("Player");
        roomsParent = new GameObject("RoomsParent").transform;
        roomsParent.SetParent(miniMap);
        roomsParent.localPosition = Vector2.zero;
        roomsParent.localScale = Vector2.one;
        SetScale();
    }

    private void PlaceRoom(int x, int y)
    {
        visibleDungeon[x, y] = true;

        if (dungeon[x, y] != 0)
        {
            GameObject room = Instantiate(miniRoom, roomsParent);
            room.transform.localPosition = new Vector2((x - 15) * 70, (y - 15) * 50);
            Rooms[x,y] = room.GetComponent<Image>();

            if (x != 15 || y != 15)
            {
                if (dungeon[x, y] == 4)
                {
                    GameObject icon = Instantiate(icons[0], room.transform);
                    icon.transform.localPosition = Vector3.zero;
                }
                else if (dungeon[x, y] == 2)
                {
                    GameObject icon = Instantiate(icons[1], room.transform);
                    icon.transform.localPosition = Vector3.zero;
                }
                else if (dungeon[x, y] == 3)
                {
                    GameObject icon = Instantiate(icons[2], room.transform);
                    icon.transform.localPosition = Vector3.zero;
                }
            }
        }
    }

    public void UpdateMap(int x, int y)
    {
        if (!visibleDungeon[x, y])PlaceRoom(x, y);  
        Rooms[x,y].color = new Color(1, 1, 1, 0.2f);

        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 0: 
                    if(dungeon[x+1, y] != 0){
                        if (!visibleDungeon[x+1, y]){PlaceRoom(x+1, y);
                        Rooms[x+1,y].color = new Color(1, 1, 1, 0.1f);}
                    }
                    break;
                case 1: 
                    if(dungeon[x-1, y] != 0){
                        if (!visibleDungeon[x-1, y]){PlaceRoom(x-1, y);
                        Rooms[x-1,y].color = new Color(1, 1, 1, 0.1f);}
                    }
                    break;
                case 2: 
                    if(dungeon[x, y+1] != 0){
                        if (!visibleDungeon[x, y+1]){PlaceRoom(x, y+1);
                        Rooms[x,y+1].color = new Color(1, 1, 1, 0.1f);}
                    }
                    break;
                case 3: 
                    if(dungeon[x, y-1] != 0){
                        if (!visibleDungeon[x, y-1]){PlaceRoom(x, y-1);
                        Rooms[x,y-1].color = new Color(1, 1, 1, 0.1f);}
                    }
                    break;
            }
        }
    }
    public void SetScale()
    {
        ScaleX = DefaultScaleX * ((float)Screen.width / 1920f);
        ScaleY =  DefaultScaleY * ((float)Screen.height / 1080f);
    }
    private void FixedUpdate()
    {
        SetScale();
        float miniMapX = (player.transform.position.x * ScaleX);
        float miniMapY = (player.transform.position.y * ScaleY);
        if(Input.GetKey(KeyCode.Tab))
        {
            map.localScale = new Vector2(0.5f,0.5f);
            frame.localScale = new Vector2(2,2);
            frame.position = minimapPos + new Vector2(-80,-80);
            if(Text.GetBool("Map") != true)Text.SetBool("Map",true);
        }
        else {
            map.localScale = Vector2.one;
            frame.localScale = new Vector2(1,1);
            frame.position = minimapPos;
            Text.SetBool("Map", false);
        }
        miniMap.position = new Vector2(transform.position.x - miniMapX,transform.position.y - miniMapY);
    }
}
