using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject bullet;
    public float rechargeTime;
    public AudioClip shootSound;
    public LayerMask raycastLayer;

    private AudioSource sound;
    private Transform player;
    private float shootTime;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        sound = GetComponent<AudioSource>();
        shootTime = Time.time + 3;
        transform.parent = null;
    }

    private void Update()
    {
        Vector2 direction = player.position - transform.position;
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, direction, 12f, raycastLayer);

        if(rayHit.collider != null)
        {
            if (rayHit.collider.tag == "Player")
            {
                if(shootTime < Time.time)
                {
                    sound.pitch = Random.Range(0.8f,1.2f);
                    sound.PlayOneShot(shootSound);
                    Instantiate(bullet, transform.GetChild(0));
                    shootTime = Time.time + rechargeTime;
                }
            }
        }    
    }
}
