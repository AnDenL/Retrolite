using UnityEngine;

public class Bounce : MonoBehaviour
{
    public float directionX = 1f;
    public float directionY = 1f;
    public float speed = 1f;
    public float rayCastDistance = 1f;
    public LayerMask wallLayer;
    public GameObject Effect;

    private int bounces = 2;

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, transform.position + new Vector3(directionX,directionY,0),speed * Time.deltaTime);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Tilemap")){
            if(bounces == 0)BulletDisappear();

            else if(bounces > 0) bounces--;

            RaycastHit2D hitX = Physics2D.Raycast(transform.position,new Vector2(directionX,0),rayCastDistance,wallLayer);

            if (hitX.collider != null)
            {
                directionX *= -1;
            }
            RaycastHit2D hitY = Physics2D.Raycast(transform.position,new Vector2(0,directionY),rayCastDistance,wallLayer);
            if (hitY.collider != null)
            {
                directionY *= -1;
            }
        }
    }
    void BulletDisappear()
    {
            GameObject effects = Instantiate(Effect, transform);
            ParticleSystem particle = effects.GetComponent<ParticleSystem>();

            var mainModule = particle.main;
            mainModule.startColor = GetComponent<SpriteRenderer>().color;

            effects.transform.parent = null;

            Destroy(effects, 1);
            Destroy(gameObject);
    }
}
