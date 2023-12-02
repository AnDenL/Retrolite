using UnityEngine;

public class Background : MonoBehaviour
{
    public Transform player;

    private void Update()
    {
        transform.position = new Vector2(Mathf.Round(player.transform.position.x), Mathf.Round(player.transform.position.y));
    }
}