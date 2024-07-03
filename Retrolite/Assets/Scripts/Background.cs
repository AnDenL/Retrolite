using UnityEngine;

public class Background : MonoBehaviour
{
    private void Update()
    {
        transform.position = new Vector2(Mathf.Round(Game.Player.transform.position.x), Mathf.Round(Game.Player.transform.position.y));
    }
}
