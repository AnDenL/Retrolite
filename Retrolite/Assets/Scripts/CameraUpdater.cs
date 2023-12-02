using UnityEngine;

public class CameraUpdater : MonoBehaviour
{
    public int x,y;
    public RoomUpdater opt;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player")
        {
            GameObject.Find("map").GetComponent<Minimap>().UpdateMap(x, y);
            opt.UpdateDungeon(x,y);
        }
    }
}
